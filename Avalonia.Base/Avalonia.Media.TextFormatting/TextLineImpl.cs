using System;
using System.Collections.Generic;
using Avalonia.Utilities;

namespace Avalonia.Media.TextFormatting;

internal class TextLineImpl : TextLine
{
	internal IReadOnlyList<IndexedTextRun>? _indexedTextRuns;

	private readonly TextRun[] _textRuns;

	private readonly double _paragraphWidth;

	private readonly TextParagraphProperties _paragraphProperties;

	private TextLineMetrics _textLineMetrics;

	private TextLineBreak? _textLineBreak;

	private readonly FlowDirection _resolvedFlowDirection;

	internal static Comparer<TextBounds> TextBoundsComparer { get; } = Comparer<TextBounds>.Create((TextBounds x, TextBounds y) => x.Rectangle.Left.CompareTo(y.Rectangle.Left));

	public override IReadOnlyList<TextRun> TextRuns => _textRuns;

	public override int FirstTextSourceIndex { get; }

	public override int Length { get; }

	public override TextLineBreak? TextLineBreak => _textLineBreak;

	public override bool HasCollapsed { get; }

	public override bool HasOverflowed => _textLineMetrics.HasOverflowed;

	public override double Baseline => _textLineMetrics.TextBaseline;

	public override double Extent => _textLineMetrics.Extent;

	public override double Height => _textLineMetrics.Height;

	public override int NewLineLength => _textLineMetrics.NewlineLength;

	public override double OverhangAfter => _textLineMetrics.OverhangAfter;

	public override double OverhangLeading => _textLineMetrics.OverhangLeading;

	public override double OverhangTrailing => _textLineMetrics.OverhangTrailing;

	public override int TrailingWhitespaceLength => _textLineMetrics.TrailingWhitespaceLength;

	public override double Start => _textLineMetrics.Start;

	public override double Width => _textLineMetrics.Width;

	public override double WidthIncludingTrailingWhitespace => _textLineMetrics.WidthIncludingTrailingWhitespace;

	public TextLineImpl(TextRun[] textRuns, int firstTextSourceIndex, int length, double paragraphWidth, TextParagraphProperties paragraphProperties, FlowDirection resolvedFlowDirection = FlowDirection.LeftToRight, TextLineBreak? lineBreak = null, bool hasCollapsed = false)
	{
		FirstTextSourceIndex = firstTextSourceIndex;
		Length = length;
		_textLineBreak = lineBreak;
		HasCollapsed = hasCollapsed;
		_textRuns = textRuns;
		_paragraphWidth = paragraphWidth;
		_paragraphProperties = paragraphProperties;
		_resolvedFlowDirection = resolvedFlowDirection;
	}

	public override void Draw(DrawingContext drawingContext, Point lineOrigin)
	{
		(lineOrigin + new Point(Start, 0.0)).Deconstruct(out var x, out var y);
		double num = x;
		double num2 = y;
		TextRun[] textRuns = _textRuns;
		for (int i = 0; i < textRuns.Length; i++)
		{
			if (textRuns[i] is DrawableTextRun drawableTextRun)
			{
				double baselineOffset = GetBaselineOffset(this, drawableTextRun);
				drawableTextRun.Draw(drawingContext, new Point(num, num2 + baselineOffset));
				num += drawableTextRun.Size.Width;
			}
		}
	}

	private static double GetBaselineOffset(TextLine textLine, DrawableTextRun textRun)
	{
		double baseline = textRun.Baseline;
		BaselineAlignment? baselineAlignment = textRun.Properties?.BaselineAlignment;
		switch (baselineAlignment)
		{
		case BaselineAlignment.Top:
			return 0.0;
		case BaselineAlignment.Center:
			return textLine.Height / 2.0 - textRun.Size.Height / 2.0;
		case BaselineAlignment.Bottom:
			return textLine.Height - textRun.Size.Height;
		case BaselineAlignment.Baseline:
		case BaselineAlignment.TextTop:
		case BaselineAlignment.TextBottom:
		case BaselineAlignment.Subscript:
		case BaselineAlignment.Superscript:
			return textLine.Baseline - baseline;
		default:
			throw new ArgumentOutOfRangeException("baselineAlignment", baselineAlignment, null);
		}
	}

	public override TextLine Collapse(params TextCollapsingProperties?[] collapsingPropertiesList)
	{
		if (collapsingPropertiesList.Length == 0)
		{
			return this;
		}
		TextCollapsingProperties textCollapsingProperties = collapsingPropertiesList[0];
		if (textCollapsingProperties == null)
		{
			return this;
		}
		TextRun[] array = textCollapsingProperties.Collapse(this);
		if (array == null)
		{
			return this;
		}
		TextLineImpl textLineImpl = new TextLineImpl(array, FirstTextSourceIndex, Length, _paragraphWidth, _paragraphProperties, _resolvedFlowDirection, TextLineBreak, hasCollapsed: true);
		if (array.Length != 0)
		{
			textLineImpl.FinalizeLine();
		}
		return textLineImpl;
	}

	public override void Justify(JustificationProperties justificationProperties)
	{
		justificationProperties.Justify(this);
		_textLineMetrics = CreateLineMetrics();
	}

	public override CharacterHit GetCharacterHitFromDistance(double distance)
	{
		if (_textRuns.Length == 0)
		{
			return new CharacterHit(FirstTextSourceIndex);
		}
		distance -= Start;
		int num = _textRuns.Length - 1;
		int num2 = Length;
		if (_textRuns[num] is TextEndOfLine textEndOfLine)
		{
			num--;
			num2 -= textEndOfLine.Length;
		}
		int num3 = FirstTextSourceIndex;
		if (num < 0)
		{
			return new CharacterHit(num3);
		}
		if (distance <= 0.0)
		{
			TextRun textRun = _textRuns[0];
			if (_paragraphProperties.FlowDirection == FlowDirection.RightToLeft)
			{
				num3 += num2 - textRun.Length;
			}
			return GetRunCharacterHit(textRun, num3, 0.0);
		}
		if (distance >= WidthIncludingTrailingWhitespace)
		{
			TextRun textRun2 = _textRuns[num];
			if (_paragraphProperties.FlowDirection == FlowDirection.LeftToRight)
			{
				num3 += num2 - textRun2.Length;
			}
			return GetRunCharacterHit(textRun2, num3, distance);
		}
		CharacterHit result = default(CharacterHit);
		double num4 = 0.0;
		for (int i = 0; i <= num; i++)
		{
			TextRun textRun3 = _textRuns[i];
			if (textRun3 is ShapedTextRun shapedTextRun && !shapedTextRun.ShapedBuffer.IsLeftToRight)
			{
				int j = i;
				num3 += textRun3.Length;
				for (; j + 1 <= _textRuns.Length - 1; j++)
				{
					if (!(_textRuns[++j] is ShapedTextRun shapedTextRun2))
					{
						break;
					}
					if (shapedTextRun2.ShapedBuffer.IsLeftToRight)
					{
						break;
					}
					num3 += shapedTextRun2.Length;
				}
				int num5 = i;
				while (i <= j && num5 <= _textRuns.Length - 1)
				{
					textRun3 = _textRuns[num5];
					if (textRun3 is ShapedTextRun)
					{
						ShapedTextRun shapedTextRun3 = (ShapedTextRun)textRun3;
						if (!(num4 + shapedTextRun3.Size.Width <= distance))
						{
							return GetRunCharacterHit(textRun3, num3, distance - num4);
						}
						num4 += shapedTextRun3.Size.Width;
						num3 -= textRun3.Length;
					}
					num5++;
				}
			}
			result = GetRunCharacterHit(textRun3, num3, distance - num4);
			if (!(textRun3 is DrawableTextRun drawableTextRun) || i >= _textRuns.Length - 1 || !(num4 + drawableTextRun.Size.Width < distance))
			{
				break;
			}
			num4 += drawableTextRun.Size.Width;
			num3 += textRun3.Length;
		}
		return result;
	}

	private static CharacterHit GetRunCharacterHit(TextRun run, int currentPosition, double distance)
	{
		CharacterHit result;
		if (!(run is ShapedTextRun shapedTextRun))
		{
			result = ((!(run is DrawableTextRun drawableTextRun)) ? new CharacterHit(currentPosition, run.Length) : ((!(distance < drawableTextRun.Size.Width / 2.0)) ? new CharacterHit(currentPosition, run.Length) : new CharacterHit(currentPosition)));
		}
		else
		{
			result = shapedTextRun.GlyphRun.GetCharacterHitFromDistance(distance, out var _);
			int num = 0;
			if (shapedTextRun.GlyphRun.IsLeftToRight)
			{
				num = Math.Max(0, currentPosition - shapedTextRun.GlyphRun.Metrics.FirstCluster);
			}
			result = new CharacterHit(num + result.FirstCharacterIndex, result.TrailingLength);
		}
		return result;
	}

	public override double GetDistanceFromCharacterHit(CharacterHit characterHit)
	{
		if (_indexedTextRuns == null || _indexedTextRuns.Count == 0)
		{
			return Start;
		}
		int firstTextSourceIndex = Math.Min(characterHit.FirstCharacterIndex + characterHit.TrailingLength, FirstTextSourceIndex + Length);
		int currentPosition = FirstTextSourceIndex;
		TextRun textRun2 = null;
		IndexedTextRun indexedTextRun = FindIndexedRun();
		while (currentPosition < FirstTextSourceIndex + Length)
		{
			textRun2 = indexedTextRun.TextRun;
			if (textRun2 == null || indexedTextRun.TextSourceCharacterIndex + textRun2.Length > characterHit.FirstCharacterIndex || currentPosition + textRun2.Length >= FirstTextSourceIndex + Length)
			{
				break;
			}
			currentPosition += textRun2.Length;
			indexedTextRun = FindIndexedRun();
		}
		if (textRun2 == null)
		{
			return Start;
		}
		double num = 0.0;
		int num2 = indexedTextRun.RunIndex;
		int i = num2;
		FlowDirection flowDirection = GetDirection(textRun2, _resolvedFlowDirection);
		double num3 = Start + GetPreceedingDistance(indexedTextRun.RunIndex);
		if (textRun2 is DrawableTextRun { Size: var size })
		{
			num = size.Width;
		}
		if (!(textRun2 is TextEndOfLine))
		{
			if (flowDirection == FlowDirection.LeftToRight)
			{
				for (; i + 1 < _textRuns.Length; i++)
				{
					TextRun textRun3 = _textRuns[i + 1];
					FlowDirection flowDirection2 = GetDirection(textRun3, flowDirection);
					if (flowDirection != flowDirection2)
					{
						break;
					}
					if (textRun3 is DrawableTextRun drawableTextRun2)
					{
						num += drawableTextRun2.Size.Width;
					}
				}
			}
			else
			{
				while (num2 - 1 > 0)
				{
					TextRun textRun4 = _textRuns[num2 - 1];
					FlowDirection flowDirection3 = GetDirection(textRun4, flowDirection);
					if (flowDirection != flowDirection3)
					{
						break;
					}
					if (textRun4 is DrawableTextRun drawableTextRun3)
					{
						num += drawableTextRun3.Size.Width;
						num3 -= drawableTextRun3.Size.Width;
					}
					num2--;
				}
			}
		}
		int newPosition;
		int coveredLength;
		if (flowDirection == FlowDirection.RightToLeft)
		{
			return GetTextRunBoundsRightToLeft(num2, i, num3 + num, firstTextSourceIndex, currentPosition, 1, out newPosition, out coveredLength).Rectangle.Right;
		}
		return GetTextBoundsLeftToRight(num2, i, num3, firstTextSourceIndex, currentPosition, 1, out coveredLength, out newPosition).Rectangle.Left;
		IndexedTextRun FindIndexedRun()
		{
			int num4 = 0;
			IndexedTextRun indexedTextRun2 = _indexedTextRuns[num4];
			while (indexedTextRun2.TextSourceCharacterIndex != currentPosition && num4 + 1 != _indexedTextRuns.Count)
			{
				num4++;
				indexedTextRun2 = _indexedTextRuns[num4];
			}
			return indexedTextRun2;
		}
		static FlowDirection GetDirection(TextRun textRun, FlowDirection currentDirection)
		{
			if (textRun is ShapedTextRun shapedTextRun)
			{
				if (!shapedTextRun.ShapedBuffer.IsLeftToRight)
				{
					return FlowDirection.RightToLeft;
				}
				return FlowDirection.LeftToRight;
			}
			return currentDirection;
		}
		double GetPreceedingDistance(int firstIndex)
		{
			double num5 = 0.0;
			for (int j = 0; j < firstIndex; j++)
			{
				if (_textRuns[j] is DrawableTextRun drawableTextRun4)
				{
					num5 += drawableTextRun4.Size.Width;
				}
			}
			return num5;
		}
	}

	public override CharacterHit GetNextCaretCharacterHit(CharacterHit characterHit)
	{
		if (_textRuns.Length == 0 || _indexedTextRuns == null)
		{
			return default(CharacterHit);
		}
		CharacterHit characterHit2 = characterHit;
		int num = characterHit.FirstCharacterIndex + characterHit.TrailingLength;
		int textPosition;
		TextRun runAtCharacterIndex = GetRunAtCharacterIndex(num, LogicalDirection.Forward, out textPosition);
		CharacterHit result = characterHit;
		if (!(runAtCharacterIndex is ShapedTextRun shapedTextRun))
		{
			if (runAtCharacterIndex != null)
			{
				result = new CharacterHit(textPosition + runAtCharacterIndex.Length);
			}
		}
		else
		{
			int num2 = Math.Max(0, textPosition - shapedTextRun.GlyphRun.Metrics.FirstCluster - characterHit.TrailingLength);
			if (num2 > 0)
			{
				characterHit2 = new CharacterHit(Math.Max(0, characterHit.FirstCharacterIndex - num2), characterHit.TrailingLength);
			}
			result = shapedTextRun.GlyphRun.GetNextCaretCharacterHit(characterHit2);
			if (num2 > 0)
			{
				result = new CharacterHit(result.FirstCharacterIndex + num2, result.TrailingLength);
			}
		}
		if (num == result.FirstCharacterIndex + result.TrailingLength)
		{
			return characterHit;
		}
		return result;
	}

	public override CharacterHit GetPreviousCaretCharacterHit(CharacterHit characterHit)
	{
		if (_textRuns.Length == 0 || _indexedTextRuns == null)
		{
			return default(CharacterHit);
		}
		if (characterHit.TrailingLength > 0 && characterHit.FirstCharacterIndex <= FirstTextSourceIndex)
		{
			return new CharacterHit(FirstTextSourceIndex);
		}
		int num = characterHit.FirstCharacterIndex + characterHit.TrailingLength;
		if (num <= FirstTextSourceIndex)
		{
			return new CharacterHit(FirstTextSourceIndex);
		}
		CharacterHit characterHit2 = characterHit;
		int textPosition;
		TextRun runAtCharacterIndex = GetRunAtCharacterIndex(num, LogicalDirection.Backward, out textPosition);
		if (textPosition == characterHit.FirstCharacterIndex)
		{
			runAtCharacterIndex = GetRunAtCharacterIndex(characterHit.FirstCharacterIndex, LogicalDirection.Backward, out textPosition);
		}
		CharacterHit result = characterHit;
		if (!(runAtCharacterIndex is ShapedTextRun shapedTextRun))
		{
			if (runAtCharacterIndex != null)
			{
				result = ((characterHit.TrailingLength <= 0) ? new CharacterHit(textPosition + runAtCharacterIndex.Length) : new CharacterHit(textPosition, runAtCharacterIndex.Length));
			}
		}
		else
		{
			int num2 = Math.Max(0, textPosition - shapedTextRun.GlyphRun.Metrics.FirstCluster);
			if (num2 > 0)
			{
				characterHit2 = new CharacterHit(Math.Max(0, characterHit.FirstCharacterIndex - num2), characterHit.TrailingLength);
			}
			result = shapedTextRun.GlyphRun.GetPreviousCaretCharacterHit(characterHit2);
			if (num2 > 0)
			{
				result = new CharacterHit(result.FirstCharacterIndex + num2, result.TrailingLength);
			}
		}
		if (num == result.FirstCharacterIndex + result.TrailingLength)
		{
			return characterHit;
		}
		return result;
	}

	public override CharacterHit GetBackspaceCaretCharacterHit(CharacterHit characterHit)
	{
		return GetPreviousCaretCharacterHit(characterHit);
	}

	public override IReadOnlyList<TextBounds> GetTextBounds(int firstTextSourceIndex, int textLength)
	{
		if (_indexedTextRuns == null || _indexedTextRuns.Count == 0)
		{
			return Array.Empty<TextBounds>();
		}
		List<TextBounds> list = new List<TextBounds>();
		int currentPosition = FirstTextSourceIndex;
		int num = textLength;
		TextBounds textBounds = null;
		while (num > 0 && currentPosition < FirstTextSourceIndex + Length)
		{
			IndexedTextRun indexedTextRun = FindIndexedRun();
			if (indexedTextRun == null)
			{
				break;
			}
			double num2 = 0.0;
			int runIndex = indexedTextRun.RunIndex;
			int lastRunIndex = runIndex;
			TextRun textRun2 = indexedTextRun.TextRun;
			if (textRun2 == null)
			{
				break;
			}
			FlowDirection flowDirection = GetDirection(textRun2, _resolvedFlowDirection);
			if (indexedTextRun.TextSourceCharacterIndex + textRun2.Length <= firstTextSourceIndex)
			{
				currentPosition += textRun2.Length;
				continue;
			}
			double num3 = Start + GetPreceedingDistance(indexedTextRun.RunIndex);
			if (textRun2 is DrawableTextRun { Size: var size })
			{
				num2 = size.Width;
			}
			int coveredLength;
			TextBounds textBounds2 = ((flowDirection != FlowDirection.RightToLeft) ? GetTextBoundsLeftToRight(runIndex, lastRunIndex, num3, firstTextSourceIndex, currentPosition, num, out coveredLength, out currentPosition) : GetTextRunBoundsRightToLeft(runIndex, lastRunIndex, num3 + num2, firstTextSourceIndex, currentPosition, num, out coveredLength, out currentPosition));
			if (coveredLength > 0)
			{
				if (textBounds != null && TryMergeWithLastBounds(textBounds2, textBounds))
				{
					textBounds2 = textBounds;
					list[list.Count - 1] = textBounds2;
				}
				else
				{
					list.Add(textBounds2);
				}
				textBounds = textBounds2;
				num -= coveredLength;
			}
		}
		list.Sort(TextBoundsComparer);
		return list;
		IndexedTextRun FindIndexedRun()
		{
			int num4 = 0;
			IndexedTextRun indexedTextRun2 = _indexedTextRuns[num4];
			while (indexedTextRun2.TextSourceCharacterIndex != currentPosition && num4 + 1 != _indexedTextRuns.Count)
			{
				num4++;
				indexedTextRun2 = _indexedTextRuns[num4];
			}
			return indexedTextRun2;
		}
		static FlowDirection GetDirection(TextRun textRun, FlowDirection currentDirection)
		{
			if (textRun is ShapedTextRun shapedTextRun)
			{
				if (!shapedTextRun.ShapedBuffer.IsLeftToRight)
				{
					return FlowDirection.RightToLeft;
				}
				return FlowDirection.LeftToRight;
			}
			return currentDirection;
		}
		double GetPreceedingDistance(int firstIndex)
		{
			double num5 = 0.0;
			for (int i = 0; i < firstIndex; i++)
			{
				if (_textRuns[i] is DrawableTextRun drawableTextRun2)
				{
					num5 += drawableTextRun2.Size.Width;
				}
			}
			return num5;
		}
		static bool TryMergeWithLastBounds(TextBounds currentBounds, TextBounds lastBounds)
		{
			if (currentBounds.FlowDirection != lastBounds.FlowDirection)
			{
				return false;
			}
			if (currentBounds.Rectangle.Left == lastBounds.Rectangle.Right)
			{
				foreach (TextRunBounds textRunBound in currentBounds.TextRunBounds)
				{
					lastBounds.TextRunBounds.Add(textRunBound);
				}
				lastBounds.Rectangle = lastBounds.Rectangle.Union(currentBounds.Rectangle);
				return true;
			}
			if (currentBounds.Rectangle.Right == lastBounds.Rectangle.Left)
			{
				for (int j = 0; j < currentBounds.TextRunBounds.Count; j++)
				{
					lastBounds.TextRunBounds.Insert(j, currentBounds.TextRunBounds[j]);
				}
				lastBounds.Rectangle = lastBounds.Rectangle.Union(currentBounds.Rectangle);
				return true;
			}
			return false;
		}
	}

	private TextBounds GetTextRunBoundsRightToLeft(int firstRunIndex, int lastRunIndex, double endX, int firstTextSourceIndex, int currentPosition, int remainingLength, out int coveredLength, out int newPosition)
	{
		coveredLength = 0;
		List<TextRunBounds> list = new List<TextRunBounds>();
		double num = endX;
		for (int num2 = lastRunIndex; num2 >= firstRunIndex; num2--)
		{
			TextRun textRun = _textRuns[num2];
			if (textRun is ShapedTextRun currentRun)
			{
				int offset;
				TextRunBounds runBoundsRightToLeft = GetRunBoundsRightToLeft(currentRun, num, firstTextSourceIndex, remainingLength, currentPosition, out offset);
				list.Insert(0, runBoundsRightToLeft);
				if (offset > 0)
				{
					endX = runBoundsRightToLeft.Rectangle.Right;
					num = endX;
				}
				num -= runBoundsRightToLeft.Rectangle.Width;
				currentPosition += runBoundsRightToLeft.Length + offset;
				coveredLength += runBoundsRightToLeft.Length;
				remainingLength -= runBoundsRightToLeft.Length;
			}
			else
			{
				if (textRun is DrawableTextRun drawableTextRun)
				{
					num -= drawableTextRun.Size.Width;
					list.Insert(0, new TextRunBounds(new Rect(num, 0.0, drawableTextRun.Size.Width, Height), currentPosition, textRun.Length, textRun));
				}
				else
				{
					list.Add(new TextRunBounds(new Rect(endX, 0.0, 0.0, Height), currentPosition, textRun.Length, textRun));
				}
				currentPosition += textRun.Length;
				coveredLength += textRun.Length;
				remainingLength -= textRun.Length;
			}
			if (remainingLength <= 0)
			{
				break;
			}
		}
		newPosition = currentPosition;
		double width = endX - num;
		return new TextBounds(new Rect(num, 0.0, width, Height), FlowDirection.RightToLeft, list);
	}

	private TextBounds GetTextBoundsLeftToRight(int firstRunIndex, int lastRunIndex, double startX, int firstTextSourceIndex, int currentPosition, int remainingLength, out int coveredLength, out int newPosition)
	{
		coveredLength = 0;
		List<TextRunBounds> list = new List<TextRunBounds>();
		double num = startX;
		for (int i = firstRunIndex; i <= lastRunIndex; i++)
		{
			TextRun textRun = _textRuns[i];
			if (textRun is ShapedTextRun currentRun)
			{
				int offset;
				TextRunBounds runBoundsLeftToRight = GetRunBoundsLeftToRight(currentRun, num, firstTextSourceIndex, remainingLength, currentPosition, out offset);
				list.Add(runBoundsLeftToRight);
				if (offset > 0)
				{
					startX = runBoundsLeftToRight.Rectangle.Left;
					num = startX;
				}
				currentPosition += runBoundsLeftToRight.Length + offset;
				num += runBoundsLeftToRight.Rectangle.Width;
				coveredLength += runBoundsLeftToRight.Length;
				remainingLength -= runBoundsLeftToRight.Length;
			}
			else
			{
				if (textRun is DrawableTextRun drawableTextRun)
				{
					list.Add(new TextRunBounds(new Rect(num, 0.0, drawableTextRun.Size.Width, Height), currentPosition, textRun.Length, textRun));
					num += drawableTextRun.Size.Width;
				}
				else
				{
					list.Add(new TextRunBounds(new Rect(num, 0.0, 0.0, Height), currentPosition, textRun.Length, textRun));
				}
				currentPosition += textRun.Length;
				coveredLength += textRun.Length;
				remainingLength -= textRun.Length;
			}
			if (remainingLength <= 0)
			{
				break;
			}
		}
		newPosition = currentPosition;
		double width = num - startX;
		return new TextBounds(new Rect(startX, 0.0, width, Height), FlowDirection.LeftToRight, list);
	}

	private TextRunBounds GetRunBoundsLeftToRight(ShapedTextRun currentRun, double startX, int firstTextSourceIndex, int remainingLength, int currentPosition, out int offset)
	{
		int num = currentPosition;
		offset = Math.Max(0, firstTextSourceIndex - currentPosition);
		int firstCluster = currentRun.GlyphRun.Metrics.FirstCluster;
		num = ((currentPosition == firstCluster) ? (num + offset) : (firstCluster + offset));
		double distanceFromCharacterHit = currentRun.GlyphRun.GetDistanceFromCharacterHit(new CharacterHit(num));
		double distanceFromCharacterHit2 = currentRun.GlyphRun.GetDistanceFromCharacterHit(new CharacterHit(num + remainingLength));
		double num2 = startX + distanceFromCharacterHit2;
		startX += distanceFromCharacterHit;
		bool isInside;
		CharacterHit characterHitFromDistance = currentRun.GlyphRun.GetCharacterHitFromDistance(distanceFromCharacterHit, out isInside);
		CharacterHit characterHitFromDistance2 = currentRun.GlyphRun.GetCharacterHitFromDistance(distanceFromCharacterHit2, out isInside);
		int num3 = Math.Abs(characterHitFromDistance.FirstCharacterIndex + characterHitFromDistance.TrailingLength - characterHitFromDistance2.FirstCharacterIndex - characterHitFromDistance2.TrailingLength);
		if (num2 < startX)
		{
			double num4 = startX;
			double num5 = num2;
			num2 = num4;
			startX = num5;
		}
		if (num3 == 0)
		{
			num3 = NewLineLength;
		}
		double width = num2 - startX;
		return new TextRunBounds(new Rect(startX, 0.0, width, Height), currentPosition, num3, currentRun);
	}

	private TextRunBounds GetRunBoundsRightToLeft(ShapedTextRun currentRun, double endX, int firstTextSourceIndex, int remainingLength, int currentPosition, out int offset)
	{
		double num = endX;
		int num2 = currentPosition;
		offset = Math.Max(0, firstTextSourceIndex - currentPosition);
		int firstCluster = currentRun.GlyphRun.Metrics.FirstCluster;
		num2 = ((currentPosition == firstCluster) ? (num2 + offset) : (firstCluster + offset));
		double distanceFromCharacterHit = currentRun.GlyphRun.GetDistanceFromCharacterHit(new CharacterHit(num2));
		double distanceFromCharacterHit2 = currentRun.GlyphRun.GetDistanceFromCharacterHit(new CharacterHit(num2 + remainingLength));
		num -= currentRun.Size.Width - distanceFromCharacterHit2;
		endX -= currentRun.Size.Width - distanceFromCharacterHit;
		bool isInside;
		CharacterHit characterHitFromDistance = currentRun.GlyphRun.GetCharacterHitFromDistance(distanceFromCharacterHit, out isInside);
		CharacterHit characterHitFromDistance2 = currentRun.GlyphRun.GetCharacterHitFromDistance(distanceFromCharacterHit2, out isInside);
		int num3 = Math.Abs(characterHitFromDistance2.FirstCharacterIndex + characterHitFromDistance2.TrailingLength - characterHitFromDistance.FirstCharacterIndex - characterHitFromDistance.TrailingLength);
		if (endX < num)
		{
			double num4 = num;
			double num5 = endX;
			endX = num4;
			num = num5;
		}
		if (num3 == 0)
		{
			num3 = NewLineLength;
		}
		double width = endX - num;
		return new TextRunBounds(new Rect(num, 0.0, width, Height), currentPosition, num3, currentRun);
	}

	public override void Dispose()
	{
		for (int i = 0; i < _textRuns.Length; i++)
		{
			if (_textRuns[i] is ShapedTextRun shapedTextRun)
			{
				shapedTextRun.Dispose();
			}
		}
	}

	public void FinalizeLine()
	{
		_indexedTextRuns = BidiReorderer.Instance.BidiReorder(_textRuns, _paragraphProperties.FlowDirection, FirstTextSourceIndex);
		_textLineMetrics = CreateLineMetrics();
		if (_textLineBreak == null && _textRuns.Length > 1 && _textRuns[_textRuns.Length - 1] is TextEndOfLine textEndOfLine)
		{
			_textLineBreak = new TextLineBreak(textEndOfLine);
		}
	}

	private TextRun? GetRunAtCharacterIndex(int codepointIndex, LogicalDirection direction, out int textPosition)
	{
		int num = 0;
		textPosition = FirstTextSourceIndex;
		if (_indexedTextRuns == null)
		{
			return null;
		}
		TextRun textRun = null;
		TextRun textRun2 = null;
		while (num < _indexedTextRuns.Count)
		{
			IndexedTextRun indexedTextRun = _indexedTextRuns[num];
			textRun = indexedTextRun.TextRun;
			if (!(textRun is ShapedTextRun shapedTextRun))
			{
				if (textRun != null)
				{
					if (codepointIndex == textPosition)
					{
						return textRun;
					}
					if (num + 1 >= _textRuns.Length)
					{
						return textRun;
					}
					textPosition += textRun.Length;
				}
			}
			else
			{
				int firstCluster = shapedTextRun.GlyphRun.Metrics.FirstCluster;
				firstCluster += Math.Max(0, indexedTextRun.TextSourceCharacterIndex - firstCluster);
				if (direction == LogicalDirection.Forward)
				{
					if (codepointIndex >= firstCluster && codepointIndex < firstCluster + textRun.Length)
					{
						return textRun;
					}
				}
				else
				{
					if (textRun2 != null && !(textRun2 is ShapedTextRun) && codepointIndex == textPosition + firstCluster)
					{
						textPosition -= textRun2.Length;
						return textRun2;
					}
					if (codepointIndex > firstCluster && codepointIndex <= firstCluster + textRun.Length)
					{
						return textRun;
					}
				}
				if (num + 1 >= _textRuns.Length)
				{
					return textRun;
				}
				textPosition += textRun.Length;
			}
			num++;
			textRun2 = textRun;
		}
		return textRun;
	}

	private TextLineMetrics CreateLineMetrics()
	{
		FontMetrics metrics = _paragraphProperties.DefaultTextRunProperties.CachedGlyphTypeface.Metrics;
		double fontRenderingEmSize = _paragraphProperties.DefaultTextRunProperties.FontRenderingEmSize;
		double num = fontRenderingEmSize / (double)metrics.DesignEmHeight;
		double num2 = 0.0;
		int num3 = 0;
		int num4 = 0;
		double num5 = (double)metrics.Ascent * num;
		double num6 = (double)metrics.Descent * num;
		double num7 = (double)metrics.LineGap * num;
		double num8 = num6 - num5 + num7;
		double lineHeight = _paragraphProperties.LineHeight;
		Rect rect = default(Rect);
		for (int i = 0; i < _textRuns.Length; i++)
		{
			TextRun textRun = _textRuns[i];
			if (!(textRun is ShapedTextRun { TextMetrics: var textMetrics } shapedTextRun))
			{
				if (textRun is DrawableTextRun drawableTextRun)
				{
					num2 += drawableTextRun.Size.Width;
					if (drawableTextRun.Size.Height > num8)
					{
						num8 = drawableTextRun.Size.Height;
					}
					if (num5 > 0.0 - drawableTextRun.Baseline)
					{
						num5 = 0.0 - drawableTextRun.Baseline;
					}
					rect = rect.Union(new Rect(new Point(rect.Right, 0.0), drawableTextRun.Size));
				}
				continue;
			}
			GlyphRun glyphRun = shapedTextRun.GlyphRun;
			rect = rect.Union(glyphRun.InkBounds);
			if (fontRenderingEmSize < textMetrics.FontRenderingEmSize)
			{
				fontRenderingEmSize = textMetrics.FontRenderingEmSize;
				if (num5 > textMetrics.Ascent)
				{
					num5 = textMetrics.Ascent;
				}
				if (num6 < textMetrics.Descent)
				{
					num6 = textMetrics.Descent;
				}
				if (num7 < textMetrics.LineGap)
				{
					num7 = textMetrics.LineGap;
				}
				if (num6 - num5 + num7 > num8)
				{
					num8 = num6 - num5 + num7;
				}
			}
			num2 += shapedTextRun.Size.Width;
		}
		double overhangAfter = Math.Max(0.0, rect.Bottom - num8);
		double num9 = num2;
		for (int num10 = _textRuns.Length - 1; num10 >= 0; num10--)
		{
			if (_textRuns[num10] is ShapedTextRun shapedTextRun2)
			{
				GlyphRun glyphRun2 = shapedTextRun2.GlyphRun;
				GlyphRunMetrics metrics2 = glyphRun2.Metrics;
				num4 += metrics2.NewLineLength;
				if (metrics2.TrailingWhitespaceLength == 0)
				{
					break;
				}
				num3 += metrics2.TrailingWhitespaceLength;
				double num11 = glyphRun2.Bounds.Width - metrics2.Width;
				num9 -= num11;
			}
		}
		double paragraphOffsetX = GetParagraphOffsetX(num9, num2);
		double num12 = Math.Max(0.0, rect.Left - paragraphOffsetX);
		double num13 = Math.Max(0.0, rect.Width - num2);
		bool hasOverflowed = num12 + num2 + num13 > _paragraphWidth;
		if (!double.IsNaN(lineHeight) && !MathUtilities.IsZero(lineHeight) && lineHeight > num8)
		{
			num8 = lineHeight;
		}
		TextLineMetrics result = default(TextLineMetrics);
		result.HasOverflowed = hasOverflowed;
		result.Height = num8;
		result.Extent = rect.Height;
		result.NewlineLength = num4;
		result.Start = paragraphOffsetX;
		result.TextBaseline = 0.0 - num5;
		result.TrailingWhitespaceLength = num3;
		result.Width = num9;
		result.WidthIncludingTrailingWhitespace = num2;
		result.OverhangLeading = num12;
		result.OverhangTrailing = num13;
		result.OverhangAfter = overhangAfter;
		return result;
	}

	private double GetParagraphOffsetX(double width, double widthIncludingTrailingWhitespace)
	{
		if (double.IsPositiveInfinity(_paragraphWidth))
		{
			return 0.0;
		}
		TextAlignment textAlignment = _paragraphProperties.TextAlignment;
		FlowDirection flowDirection = _paragraphProperties.FlowDirection;
		if (textAlignment == TextAlignment.Justify)
		{
			textAlignment = TextAlignment.Start;
		}
		switch (textAlignment)
		{
		case TextAlignment.Start:
			textAlignment = ((flowDirection != 0) ? TextAlignment.Right : TextAlignment.Left);
			break;
		case TextAlignment.End:
			textAlignment = ((flowDirection != FlowDirection.RightToLeft) ? TextAlignment.Right : TextAlignment.Left);
			break;
		case TextAlignment.DetectFromContent:
			textAlignment = ((_resolvedFlowDirection != 0) ? TextAlignment.Right : TextAlignment.Left);
			break;
		}
		switch (textAlignment)
		{
		case TextAlignment.Center:
		{
			double num = (_paragraphWidth - width) / 2.0;
			if (flowDirection == FlowDirection.RightToLeft)
			{
				num -= widthIncludingTrailingWhitespace - width;
			}
			return Math.Max(0.0, num);
		}
		case TextAlignment.Right:
			return Math.Max(0.0, _paragraphWidth - widthIncludingTrailingWhitespace);
		default:
			return 0.0;
		}
	}
}
