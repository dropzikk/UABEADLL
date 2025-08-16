namespace Avalonia.Styling;

internal class NthLastChildSelector : NthChildSelector
{
	public NthLastChildSelector(Selector? previous, int step, int offset)
		: base(previous, step, offset, reversed: true)
	{
	}
}
