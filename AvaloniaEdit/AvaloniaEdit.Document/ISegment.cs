namespace AvaloniaEdit.Document;

public interface ISegment
{
	int Offset { get; }

	int Length { get; }

	int EndOffset { get; }
}
