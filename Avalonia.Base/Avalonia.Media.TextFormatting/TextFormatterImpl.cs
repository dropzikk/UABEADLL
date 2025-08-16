using System;
using System.Buffers;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using Avalonia.Media.TextFormatting.Unicode;
using Avalonia.Utilities;

namespace Avalonia.Media.TextFormatting;

internal sealed class TextFormatterImpl : TextFormatter
{
	private struct TextRunEnumerator
	{
		private readonly ITextSource _textSource;

		private int _pos;

		public TextRun? Current { get; private set; }

		public TextRunEnumerator(ITextSource textSource, int firstTextSourceIndex)
		{
			_textSource = textSource;
			_pos = firstTextSourceIndex;
			Current = null;
		}

		public bool MoveNext()
		{
			Current = _textSource.GetTextRun(_pos);
			if (Current == null)
			{
				return false;
			}
			if (Current.Length == 0)
			{
				return false;
			}
			_pos += Current.Length;
			return true;
		}
	}

	private static readonly char[] s_empty = new char[1] { ' ' };

	private static readonly char[] s_defaultText = new char[1];

	[ThreadStatic]
	private static BidiData? t_bidiData;

	[ThreadStatic]
	private static BidiAlgorithm? t_bidiAlgorithm;

	public override TextLine? FormatLine(ITextSource textSource, int firstTextSourceIndex, double paragraphWidth, TextParagraphProperties paragraphProperties, TextLineBreak? previousLineBreak = null)
	{
		TextLineBreak textLineBreak = null;
		FormattingObjectPool instance = FormattingObjectPool.Instance;
		FontManager current = FontManager.Current;
		if (previousLineBreak is WrappingTextLineBreak wrappingTextLineBreak)
		{
			List<TextRun> list = wrappingTextLineBreak.AcquireRemainingRuns();
			if (list != null && paragraphProperties.TextWrapping != 0)
			{
				return PerformTextWrapping(list, canReuseTextRunList: true, firstTextSourceIndex, paragraphWidth, paragraphProperties, previousLineBreak.FlowDirection, previousLineBreak, instance);
			}
		}
		FormattingObjectPool.RentedList<TextRun> rentedList = null;
		FormattingObjectPool.RentedList<TextRun> rentedList2 = null;
		try
		{
			rentedList = FetchTextRuns(textSource, firstTextSourceIndex, instance, out TextEndOfLine endOfLine, out int textSourceLength);
			if (rentedList.Count == 0)
			{
				return null;
			}
			rentedList2 = ShapeTextRuns(rentedList, paragraphProperties, instance, current, out var resolvedFlowDirection);
			if (textLineBreak == null && endOfLine != null)
			{
				textLineBreak = new TextLineBreak(endOfLine, resolvedFlowDirection);
			}
			switch (paragraphProperties.TextWrapping)
			{
			case TextWrapping.NoWrap:
			{
				TextLineImpl textLineImpl = new TextLineImpl(rentedList2.ToArray(), firstTextSourceIndex, textSourceLength, paragraphWidth, paragraphProperties, resolvedFlowDirection, textLineBreak);
				textLineImpl.FinalizeLine();
				return textLineImpl;
			}
			case TextWrapping.Wrap:
			case TextWrapping.WrapWithOverflow:
				return PerformTextWrapping(rentedList2, canReuseTextRunList: false, firstTextSourceIndex, paragraphWidth, paragraphProperties, resolvedFlowDirection, textLineBreak, instance);
			default:
				throw new ArgumentOutOfRangeException("TextWrapping");
			}
		}
		finally
		{
			instance.TextRunLists.Return(ref rentedList2);
			instance.TextRunLists.Return(ref rentedList);
		}
	}

	internal static SplitResult<FormattingObjectPool.RentedList<TextRun>> SplitTextRuns(IReadOnlyList<TextRun> textRuns, int length, FormattingObjectPool objectPool)
	{
		FormattingObjectPool.RentedList<TextRun> rentedList = objectPool.TextRunLists.Rent();
		int num = 0;
		for (int i = 0; i < textRuns.Count; i++)
		{
			TextRun textRun = textRuns[i];
			int length2 = textRun.Length;
			if (num + length2 < length)
			{
				num += length2;
				continue;
			}
			int num2 = ((length2 >= 1) ? (i + 1) : i);
			if (num2 > 1)
			{
				for (int j = 0; j < i; j++)
				{
					rentedList.Add(textRuns[j]);
				}
			}
			int num3 = textRuns.Count - num2;
			if (num + length2 == length)
			{
				FormattingObjectPool.RentedList<TextRun> rentedList2 = ((num3 > 0) ? objectPool.TextRunLists.Rent() : null);
				if (rentedList2 != null)
				{
					int num4 = ((length2 >= 1) ? 1 : 0);
					for (int k = 0; k < num3; k++)
					{
						rentedList2.Add(textRuns[i + k + num4]);
					}
				}
				rentedList.Add(textRun);
				return new SplitResult<FormattingObjectPool.RentedList<TextRun>>(rentedList, rentedList2);
			}
			num3++;
			FormattingObjectPool.RentedList<TextRun> rentedList3 = objectPool.TextRunLists.Rent();
			if (textRun is ShapedTextRun shapedTextRun)
			{
				SplitResult<ShapedTextRun> splitResult = shapedTextRun.Split(length - num);
				rentedList.Add(splitResult.First);
				rentedList3.Add(splitResult.Second);
			}
			for (int l = 1; l < num3; l++)
			{
				rentedList3.Add(textRuns[i + l]);
			}
			return new SplitResult<FormattingObjectPool.RentedList<TextRun>>(rentedList, rentedList3);
		}
		for (int m = 0; m < textRuns.Count; m++)
		{
			rentedList.Add(textRuns[m]);
		}
		return new SplitResult<FormattingObjectPool.RentedList<TextRun>>(rentedList, null);
	}

	private static FormattingObjectPool.RentedList<TextRun> ShapeTextRuns(IReadOnlyList<TextRun> textRuns, TextParagraphProperties paragraphProperties, FormattingObjectPool objectPool, FontManager fontManager, out FlowDirection resolvedFlowDirection)
	{
		FlowDirection flowDirection = paragraphProperties.FlowDirection;
		FormattingObjectPool.RentedList<TextRun> rentedList = objectPool.TextRunLists.Rent();
		if (textRuns.Count == 0)
		{
			resolvedFlowDirection = flowDirection;
			return rentedList;
		}
		BidiData bidiData = t_bidiData ?? (t_bidiData = new BidiData());
		bidiData.Reset();
		bidiData.ParagraphEmbeddingLevel = (sbyte)flowDirection;
		for (int i = 0; i < textRuns.Count; i++)
		{
			TextRun textRun = textRuns[i];
			ReadOnlyMemory<char> text = textRun.Text;
			ReadOnlySpan<char> text2;
			if (!text.IsEmpty)
			{
				text = textRun.Text;
				text2 = text.Span;
			}
			else
			{
				text2 = ((textRun.Length != 1) ? ((ReadOnlySpan<char>)new char[textRun.Length]) : ((ReadOnlySpan<char>)s_defaultText));
			}
			bidiData.Append(text2);
		}
		BidiAlgorithm bidiAlgorithm = t_bidiAlgorithm ?? (t_bidiAlgorithm = new BidiAlgorithm());
		bidiAlgorithm.Process(bidiData);
		sbyte b = bidiAlgorithm.ResolveEmbeddingLevel(bidiData.Classes);
		resolvedFlowDirection = (((b & 1) != 0) ? FlowDirection.RightToLeft : FlowDirection.LeftToRight);
		FormattingObjectPool.RentedList<TextRun> rentedList2 = objectPool.TextRunLists.Rent();
		FormattingObjectPool.RentedList<UnshapedTextRun> rentedList3 = objectPool.UnshapedTextRunLists.Rent();
		try
		{
			CoalesceLevels(textRuns, bidiAlgorithm.ResolvedLevels.Span, fontManager, rentedList2);
			bidiData.Reset();
			bidiAlgorithm.Reset();
			TextShaper current = TextShaper.Current;
			for (int j = 0; j < rentedList2.Count; j++)
			{
				TextRun textRun2 = rentedList2[j];
				UnshapedTextRun unshapedTextRun = textRun2 as UnshapedTextRun;
				if (unshapedTextRun != null)
				{
					rentedList3.Clear();
					rentedList3.Add(unshapedTextRun);
					ReadOnlyMemory<char> readOnlyMemory = unshapedTextRun.Text;
					TextRunProperties properties = unshapedTextRun.Properties;
					ReadOnlyMemory<char> joinedMemory;
					while (j + 1 < rentedList2.Count && rentedList2[j + 1] is UnshapedTextRun unshapedTextRun2 && unshapedTextRun.BidiLevel == unshapedTextRun2.BidiLevel && TryJoinContiguousMemories(readOnlyMemory, unshapedTextRun2.Text, out joinedMemory) && CanShapeTogether(properties, unshapedTextRun2.Properties))
					{
						rentedList3.Add(unshapedTextRun2);
						j++;
						unshapedTextRun = unshapedTextRun2;
						readOnlyMemory = joinedMemory;
					}
					ShapeTogether(options: new TextShaperOptions(properties.CachedGlyphTypeface, properties.FontRenderingEmSize, unshapedTextRun.BidiLevel, properties.CultureInfo, paragraphProperties.DefaultIncrementalTab, paragraphProperties.LetterSpacing), textRuns: rentedList3, text: readOnlyMemory, textShaper: current, results: rentedList);
				}
				else
				{
					rentedList.Add(textRun2);
				}
			}
			return rentedList;
		}
		finally
		{
			objectPool.TextRunLists.Return(ref rentedList2);
			objectPool.UnshapedTextRunLists.Return(ref rentedList3);
		}
	}

	private static bool TryJoinContiguousMemories(ReadOnlyMemory<char> x, ReadOnlyMemory<char> y, out ReadOnlyMemory<char> joinedMemory)
	{
		ArraySegment<char> segment;
		MemoryManager<char> manager;
		MemoryManager<char> manager2;
		int start3;
		int length3;
		int joinedStart4;
		if (MemoryMarshal.TryGetString(x, out string text, out int start, out int length))
		{
			if (MemoryMarshal.TryGetString(y, out string text2, out int start2, out int length2) && (object)text == text2 && TryGetContiguousStart(start, length, start2, length2, out var joinedStart2))
			{
				joinedMemory = text.AsMemory(joinedStart2, length + length2);
				return true;
			}
		}
		else if (MemoryMarshal.TryGetArray(x, out segment))
		{
			if (MemoryMarshal.TryGetArray(y, out var segment2) && segment.Array == segment2.Array && TryGetContiguousStart(segment.Offset, segment.Count, segment2.Offset, segment2.Count, out var joinedStart3))
			{
				joinedMemory = segment.Array.AsMemory(joinedStart3, segment.Count + segment2.Count);
				return true;
			}
		}
		else if (MemoryMarshal.TryGetMemoryManager<char, MemoryManager<char>>(x, out manager, out start, out length) && MemoryMarshal.TryGetMemoryManager<char, MemoryManager<char>>(y, out manager2, out start3, out length3) && manager == manager2 && TryGetContiguousStart(start, length, start3, length3, out joinedStart4))
		{
			joinedMemory = manager.Memory.Slice(joinedStart4, length + length3);
			return true;
		}
		joinedMemory = default(ReadOnlyMemory<char>);
		return false;
		static bool TryGetContiguousStart(int xStart, int xLength, int yStart, int yLength, out int joinedStart)
		{
			(int, int) tuple = (xStart, xLength);
			(int, int) tuple2 = (yStart, yLength);
			(int, int) tuple4;
			(int, int) tuple5;
			if (xStart > yStart)
			{
				(int, int) tuple3 = tuple;
				tuple4 = tuple2;
				tuple5 = tuple3;
			}
			else
			{
				(int, int) tuple3 = tuple2;
				tuple4 = tuple;
				tuple5 = tuple3;
			}
			if (tuple4.Item1 + tuple4.Item2 == tuple5.Item1)
			{
				(joinedStart, _) = tuple4;
				return true;
			}
			joinedStart = 0;
			return false;
		}
	}

	private static bool CanShapeTogether(TextRunProperties x, TextRunProperties y)
	{
		if (MathUtilities.AreClose(x.FontRenderingEmSize, y.FontRenderingEmSize) && x.Typeface == y.Typeface)
		{
			return x.BaselineAlignment == y.BaselineAlignment;
		}
		return false;
	}

	private static void ShapeTogether(IReadOnlyList<UnshapedTextRun> textRuns, ReadOnlyMemory<char> text, TextShaperOptions options, TextShaper textShaper, FormattingObjectPool.RentedList<TextRun> results)
	{
		ShapedBuffer shapedBuffer = textShaper.ShapeText(text, options);
		for (int i = 0; i < textRuns.Count; i++)
		{
			UnshapedTextRun unshapedTextRun = textRuns[i];
			SplitResult<ShapedBuffer> splitResult = shapedBuffer.Split(unshapedTextRun.Length);
			results.Add(new ShapedTextRun(splitResult.First, unshapedTextRun.Properties));
			shapedBuffer = splitResult.Second;
		}
	}

	private static void CoalesceLevels(IReadOnlyList<TextRun> textCharacters, ReadOnlySpan<sbyte> levels, FontManager fontManager, FormattingObjectPool.RentedList<TextRun> processedRuns)
	{
		if (levels.Length == 0)
		{
			return;
		}
		int num = 0;
		sbyte b = levels[0];
		TextRunProperties previousProperties = null;
		TextCharacters textCharacters2 = null;
		ReadOnlyMemory<char> text = default(ReadOnlyMemory<char>);
		for (int i = 0; i < textCharacters.Count; i++)
		{
			int num2 = 0;
			textCharacters2 = textCharacters[i] as TextCharacters;
			if (textCharacters2 == null)
			{
				TextRun textRun = textCharacters[i];
				processedRuns.Add(textRun);
				num += textRun.Length;
				continue;
			}
			text = textCharacters2.Text;
			ReadOnlySpan<char> span = text.Span;
			while (num2 < span.Length)
			{
				Codepoint.ReadAt(span, num2, out var count);
				if (num + 1 == levels.Length)
				{
					break;
				}
				num++;
				num2 += count;
				if (num2 == span.Length)
				{
					textCharacters2.GetShapeableCharacters(text.Slice(0, num2), b, fontManager, ref previousProperties, processedRuns);
					b = levels[num];
				}
				else if (levels[num] != b)
				{
					textCharacters2.GetShapeableCharacters(text.Slice(0, num2), b, fontManager, ref previousProperties, processedRuns);
					text = text.Slice(num2);
					span = text.Span;
					num2 = 0;
					b = levels[num];
				}
			}
		}
		if (textCharacters2 != null && !text.IsEmpty)
		{
			textCharacters2.GetShapeableCharacters(text, b, fontManager, ref previousProperties, processedRuns);
		}
	}

	private static FormattingObjectPool.RentedList<TextRun> FetchTextRuns(ITextSource textSource, int firstTextSourceIndex, FormattingObjectPool objectPool, out TextEndOfLine? endOfLine, out int textSourceLength)
	{
		textSourceLength = 0;
		endOfLine = null;
		FormattingObjectPool.RentedList<TextRun> rentedList = objectPool.TextRunLists.Rent();
		TextRunEnumerator textRunEnumerator = new TextRunEnumerator(textSource, firstTextSourceIndex);
		while (textRunEnumerator.MoveNext())
		{
			TextRun current = textRunEnumerator.Current;
			if (current is TextEndOfLine textEndOfLine)
			{
				endOfLine = textEndOfLine;
				textSourceLength += textEndOfLine.Length;
				rentedList.Add(current);
				break;
			}
			if (current is TextCharacters textCharacters)
			{
				if (TryGetLineBreak(textCharacters, out var lineBreak))
				{
					TextCharacters item = new TextCharacters(textCharacters.Text.Slice(0, lineBreak.PositionWrap), textCharacters.Properties);
					rentedList.Add(item);
					textSourceLength += lineBreak.PositionWrap;
					return rentedList;
				}
				rentedList.Add(textCharacters);
			}
			else
			{
				rentedList.Add(current);
			}
			textSourceLength += current.Length;
		}
		return rentedList;
	}

	private static bool TryGetLineBreak(TextRun textRun, out LineBreak lineBreak)
	{
		lineBreak = default(LineBreak);
		ReadOnlyMemory<char> text = textRun.Text;
		if (text.IsEmpty)
		{
			return false;
		}
		LineBreakEnumerator lineBreakEnumerator = new LineBreakEnumerator(text.Span);
		while (lineBreakEnumerator.MoveNext(out lineBreak))
		{
			if (lineBreak.Required)
			{
				if (lineBreak.PositionWrap < textRun.Length)
				{
					return true;
				}
				return true;
			}
		}
		return false;
	}

	private static int MeasureLength(IReadOnlyList<TextRun> textRuns, double paragraphWidth)
	{
		int num = 0;
		double num2 = 0.0;
		for (int i = 0; i < textRuns.Count; i++)
		{
			TextRun textRun = textRuns[i];
			if (!(textRun is ShapedTextRun shapedTextRun))
			{
				if (textRun is DrawableTextRun drawableTextRun)
				{
					if (num2 + drawableTextRun.Size.Width >= paragraphWidth)
					{
						return num;
					}
					num += textRun.Length;
					num2 += drawableTextRun.Size.Width;
				}
				else
				{
					num += textRun.Length;
				}
			}
			else
			{
				if (shapedTextRun.ShapedBuffer.Length <= 0)
				{
					continue;
				}
				int num3 = 0;
				for (int j = 0; j < shapedTextRun.ShapedBuffer.Length; j++)
				{
					GlyphInfo glyphInfo = shapedTextRun.ShapedBuffer[j];
					double num4 = glyphInfo.GlyphAdvance;
					GlyphInfo glyphInfo2 = default(GlyphInfo);
					for (; j + 1 < shapedTextRun.ShapedBuffer.Length; j++)
					{
						glyphInfo2 = shapedTextRun.ShapedBuffer[j + 1];
						if (glyphInfo.GlyphCluster != glyphInfo2.GlyphCluster)
						{
							break;
						}
						num4 += glyphInfo2.GlyphAdvance;
					}
					int num5 = Math.Max(0, glyphInfo2.GlyphCluster - glyphInfo.GlyphCluster);
					if (num5 == 0)
					{
						num5 = textRun.Length - num3;
					}
					if (num5 == 0)
					{
						num5 = shapedTextRun.GlyphRun.Metrics.FirstCluster + textRun.Length - glyphInfo.GlyphCluster;
					}
					if (num2 + num4 > paragraphWidth)
					{
						if (num3 == 0 && num == 0)
						{
							num3 = num5;
						}
						return num + num3;
					}
					num2 += num4;
					num3 += num5;
				}
				num += num3;
			}
		}
		return num;
	}

	public static TextLineImpl CreateEmptyTextLine(int firstTextSourceIndex, double paragraphWidth, TextParagraphProperties paragraphProperties)
	{
		FlowDirection flowDirection = paragraphProperties.FlowDirection;
		TextRunProperties defaultTextRunProperties = paragraphProperties.DefaultTextRunProperties;
		IGlyphTypeface cachedGlyphTypeface = defaultTextRunProperties.CachedGlyphTypeface;
		ushort glyph = cachedGlyphTypeface.GetGlyph(s_empty[0]);
		GlyphInfo[] array = new GlyphInfo[1]
		{
			new GlyphInfo(glyph, firstTextSourceIndex, 0.0)
		};
		ShapedBuffer shapedBuffer = new ShapedBuffer(s_empty.AsMemory(), array, cachedGlyphTypeface, defaultTextRunProperties.FontRenderingEmSize, (sbyte)flowDirection);
		TextLineImpl textLineImpl = new TextLineImpl(new TextRun[1]
		{
			new ShapedTextRun(shapedBuffer, defaultTextRunProperties)
		}, firstTextSourceIndex, 0, paragraphWidth, paragraphProperties, flowDirection);
		textLineImpl.FinalizeLine();
		return textLineImpl;
	}

	private static TextLineImpl PerformTextWrapping(List<TextRun> textRuns, bool canReuseTextRunList, int firstTextSourceIndex, double paragraphWidth, TextParagraphProperties paragraphProperties, FlowDirection resolvedFlowDirection, TextLineBreak? currentLineBreak, FormattingObjectPool objectPool)
	{
		if (textRuns.Count == 0)
		{
			return CreateEmptyTextLine(firstTextSourceIndex, paragraphWidth, paragraphProperties);
		}
		int num = MeasureLength(textRuns, paragraphWidth);
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		for (int i = 0; i < textRuns.Count; i++)
		{
			bool flag = false;
			TextRun textRun = textRuns[i];
			if (textRun is ShapedTextRun)
			{
				ReadOnlyMemory<char> text = textRun.Text;
				LineBreakEnumerator lineBreakEnumerator = new LineBreakEnumerator(text.Span);
				LineBreak lineBreak;
				while (lineBreakEnumerator.MoveNext(out lineBreak))
				{
					if (lineBreak.Required && num2 + lineBreak.PositionMeasure <= num)
					{
						flag = true;
						num4 = num2 + lineBreak.PositionWrap;
						break;
					}
					if (num2 + lineBreak.PositionMeasure > num)
					{
						if (paragraphProperties.TextWrapping == TextWrapping.WrapWithOverflow)
						{
							if (num3 > 0)
							{
								num4 = num3;
								flag = true;
								break;
							}
							if (i < textRuns.Count - 1)
							{
								if (lineBreak.PositionWrap != textRun.Length)
								{
									flag = true;
									num4 = num2 + lineBreak.PositionWrap;
									break;
								}
								while (lineBreakEnumerator.MoveNext(out lineBreak))
								{
									num4 += lineBreak.PositionWrap;
									if (lineBreak.PositionWrap != textRun.Length)
									{
										break;
									}
									i++;
									if (i >= textRuns.Count)
									{
										break;
									}
									textRun = textRuns[i];
									text = textRun.Text;
									lineBreakEnumerator = new LineBreakEnumerator(text.Span);
								}
							}
							else
							{
								num4 = num2 + lineBreak.PositionWrap;
							}
							if (num4 == 0 && num > 0)
							{
								num4 = num;
							}
							flag = true;
						}
						else
						{
							num4 = ((num3 == 0) ? num : num3);
							flag = true;
						}
						break;
					}
					if (lineBreak.PositionMeasure != lineBreak.PositionWrap)
					{
						num3 = num2 + lineBreak.PositionWrap;
					}
				}
			}
			if (!flag)
			{
				num2 += textRun.Length;
				continue;
			}
			if (num4 <= num || resolvedFlowDirection != FlowDirection.RightToLeft)
			{
				num = num4;
			}
			break;
		}
		var (rentedList3, rentedList4) = (SplitResult<FormattingObjectPool.RentedList<TextRun>>)(ref SplitTextRuns(textRuns, num, objectPool));
		try
		{
			TextLineBreak lineBreak2;
			if (rentedList4 != null && rentedList4.Count > 0)
			{
				List<TextRun> list;
				if (canReuseTextRunList)
				{
					list = textRuns;
					list.Clear();
				}
				else
				{
					list = new List<TextRun>();
				}
				for (int j = 0; j < rentedList4.Count; j++)
				{
					list.Add(rentedList4[j]);
				}
				lineBreak2 = new WrappingTextLineBreak(null, resolvedFlowDirection, list);
			}
			else
			{
				TextEndOfLine textEndOfLine = currentLineBreak?.TextEndOfLine;
				lineBreak2 = ((textEndOfLine == null) ? null : new TextLineBreak(textEndOfLine, resolvedFlowDirection));
			}
			TextLineImpl textLineImpl = new TextLineImpl(rentedList3.ToArray(), firstTextSourceIndex, num, paragraphWidth, paragraphProperties, resolvedFlowDirection, lineBreak2);
			textLineImpl.FinalizeLine();
			return textLineImpl;
		}
		finally
		{
			objectPool.TextRunLists.Return(ref rentedList3);
			objectPool.TextRunLists.Return(ref rentedList4);
		}
	}

	internal static ShapedTextRun CreateSymbol(TextRun textRun, FlowDirection flowDirection)
	{
		TextShaper current = TextShaper.Current;
		IGlyphTypeface cachedGlyphTypeface = textRun.Properties.CachedGlyphTypeface;
		double fontRenderingEmSize = textRun.Properties.FontRenderingEmSize;
		CultureInfo cultureInfo = textRun.Properties.CultureInfo;
		return new ShapedTextRun(current.ShapeText(options: new TextShaperOptions(cachedGlyphTypeface, fontRenderingEmSize, (sbyte)flowDirection, cultureInfo), text: textRun.Text), textRun.Properties);
	}
}
