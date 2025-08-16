using System.Collections.Generic;

namespace Avalonia.Media.TextFormatting;

internal sealed class WrappingTextLineBreak : TextLineBreak
{
	private List<TextRun>? _remainingRuns;

	public WrappingTextLineBreak(TextEndOfLine? textEndOfLine, FlowDirection flowDirection, List<TextRun> remainingRuns)
		: base(textEndOfLine, flowDirection, isSplit: true)
	{
		_remainingRuns = remainingRuns;
	}

	public List<TextRun>? AcquireRemainingRuns()
	{
		List<TextRun>? remainingRuns = _remainingRuns;
		_remainingRuns = null;
		return remainingRuns;
	}
}
