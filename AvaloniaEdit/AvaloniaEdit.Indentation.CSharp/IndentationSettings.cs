namespace AvaloniaEdit.Indentation.CSharp;

internal sealed class IndentationSettings
{
	public string IndentString { get; set; } = "\t";

	public bool LeaveEmptyLines { get; set; } = true;
}
