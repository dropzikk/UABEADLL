using System;
using System.Collections.Generic;

namespace Avalonia.Media.TextFormatting;

public sealed class TextLeadingPrefixCharacterEllipsis : TextCollapsingProperties
{
	private readonly int _prefixLength;

	public override double Width { get; }

	public override TextRun Symbol { get; }

	public override FlowDirection FlowDirection { get; }

	public TextLeadingPrefixCharacterEllipsis(string ellipsis, int prefixLength, double width, TextRunProperties textRunProperties, FlowDirection flowDirection)
	{
		if (_prefixLength < 0)
		{
			throw new ArgumentOutOfRangeException("prefixLength");
		}
		_prefixLength = prefixLength;
		Width = width;
		Symbol = new TextCharacters(ellipsis, textRunProperties);
		FlowDirection = flowDirection;
	}

	public override TextRun[]? Collapse(TextLine textLine)
	{
		IReadOnlyList<TextRun> textRuns = textLine.TextRuns;
		if (textRuns.Count == 0)
		{
			return null;
		}
		int i = 0;
		double num = 0.0;
		ShapedTextRun shapedTextRun = TextFormatterImpl.CreateSymbol(Symbol, FlowDirection.LeftToRight);
		if (Width < shapedTextRun.GlyphRun.Bounds.Width)
		{
			return Array.Empty<TextRun>();
		}
		double num2 = Width - shapedTextRun.Size.Width;
		for (; i < textRuns.Count; i++)
		{
			TextRun textRun = textRuns[i];
			if (!(textRun is ShapedTextRun shapedTextRun2))
			{
				if (textRun is DrawableTextRun drawableTextRun)
				{
					num2 -= drawableTextRun.Size.Width;
				}
				continue;
			}
			num += shapedTextRun2.Size.Width;
			if (num > num2)
			{
				shapedTextRun2.TryMeasureCharacters(num2, out var length);
				if (length > 0)
				{
					FormattingObjectPool instance = FormattingObjectPool.Instance;
					FormattingObjectPool.RentedList<TextRun> rentedList = instance.TextRunLists.Rent();
					FormattingObjectPool.RentedList<TextRun> rentedList2 = null;
					FormattingObjectPool.RentedList<TextRun> rentedList3 = null;
					try
					{
						IReadOnlyList<TextRun> readOnlyList;
						if (_prefixLength > 0)
						{
							TextFormatterImpl.SplitTextRuns(textRuns, Math.Min(_prefixLength, length), instance).Deconstruct(out FormattingObjectPool.RentedList<TextRun> first, out FormattingObjectPool.RentedList<TextRun> second);
							rentedList2 = first;
							rentedList3 = second;
							readOnlyList = rentedList3;
							foreach (TextRun item in rentedList2)
							{
								rentedList.Add(item);
							}
						}
						else
						{
							readOnlyList = textRuns;
						}
						rentedList.Add(shapedTextRun);
						if (length <= _prefixLength || readOnlyList == null)
						{
							return rentedList.ToArray();
						}
						double num3 = num2;
						if (rentedList2 != null)
						{
							foreach (TextRun item2 in rentedList2)
							{
								if (item2 is DrawableTextRun drawableTextRun2)
								{
									num3 -= drawableTextRun2.Size.Width;
								}
							}
						}
						for (int num4 = readOnlyList.Count - 1; num4 >= 0; num4--)
						{
							TextRun textRun2 = readOnlyList[num4];
							if (textRun2 is ShapedTextRun shapedTextRun3 && shapedTextRun3.TryMeasureCharactersBackwards(num3, out var length2, out var width))
							{
								num3 -= width;
								if (length2 > 0)
								{
									SplitResult<ShapedTextRun> splitResult = shapedTextRun3.Split(textRun2.Length - length2);
									rentedList.Add(splitResult.Second);
								}
							}
						}
						return rentedList.ToArray();
					}
					finally
					{
						instance.TextRunLists.Return(ref rentedList2);
						instance.TextRunLists.Return(ref rentedList3);
						instance.TextRunLists.Return(ref rentedList);
					}
				}
				return new TextRun[1] { shapedTextRun };
			}
			num2 -= shapedTextRun2.Size.Width;
		}
		return null;
	}
}
