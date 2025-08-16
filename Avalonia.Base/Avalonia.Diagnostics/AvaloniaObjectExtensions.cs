namespace Avalonia.Diagnostics;

public static class AvaloniaObjectExtensions
{
	public static AvaloniaPropertyValue GetDiagnostic(this AvaloniaObject o, AvaloniaProperty property)
	{
		return o.GetDiagnosticInternal(property);
	}
}
