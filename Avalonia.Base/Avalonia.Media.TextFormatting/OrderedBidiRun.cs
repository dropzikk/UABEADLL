namespace Avalonia.Media.TextFormatting;

internal struct OrderedBidiRun
{
	public int RunIndex { get; }

	public sbyte Level { get; }

	public TextRun Run { get; }

	public int NextRunIndex { get; set; }

	public OrderedBidiRun(int runIndex, TextRun run, sbyte level)
	{
		RunIndex = runIndex;
		Run = run;
		Level = level;
		NextRunIndex = -1;
	}
}
