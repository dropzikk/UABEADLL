using System.Text.RegularExpressions;

namespace AvaloniaEdit.Highlighting;

public class HighlightingRule
{
	public Regex Regex { get; set; }

	public HighlightingColor Color { get; set; }

	public override string ToString()
	{
		return "[" + GetType().Name + " " + Regex?.ToString() + "]";
	}
}
