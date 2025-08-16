using System;
using System.Collections.Generic;

namespace Avalonia.Media.TextFormatting;

public abstract class TextLine : IDisposable
{
	public abstract IReadOnlyList<TextRun> TextRuns { get; }

	public abstract int FirstTextSourceIndex { get; }

	public abstract int Length { get; }

	public abstract TextLineBreak? TextLineBreak { get; }

	public abstract double Baseline { get; }

	public abstract double Extent { get; }

	public abstract bool HasCollapsed { get; }

	public abstract bool HasOverflowed { get; }

	public abstract double Height { get; }

	public abstract int NewLineLength { get; }

	public abstract double OverhangAfter { get; }

	public abstract double OverhangLeading { get; }

	public abstract double OverhangTrailing { get; }

	public abstract double Start { get; }

	public abstract int TrailingWhitespaceLength { get; }

	public abstract double Width { get; }

	public abstract double WidthIncludingTrailingWhitespace { get; }

	public abstract void Draw(DrawingContext drawingContext, Point lineOrigin);

	public abstract TextLine Collapse(params TextCollapsingProperties?[] collapsingPropertiesList);

	public abstract void Justify(JustificationProperties justificationProperties);

	public abstract CharacterHit GetCharacterHitFromDistance(double distance);

	public abstract double GetDistanceFromCharacterHit(CharacterHit characterHit);

	public abstract CharacterHit GetNextCaretCharacterHit(CharacterHit characterHit);

	public abstract CharacterHit GetPreviousCaretCharacterHit(CharacterHit characterHit);

	public abstract CharacterHit GetBackspaceCaretCharacterHit(CharacterHit characterHit);

	public abstract IReadOnlyList<TextBounds> GetTextBounds(int firstTextSourceCharacterIndex, int textLength);

	public abstract void Dispose();
}
