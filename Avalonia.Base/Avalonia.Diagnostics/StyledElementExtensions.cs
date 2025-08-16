namespace Avalonia.Diagnostics;

public static class StyledElementExtensions
{
	public static StyleDiagnostics GetStyleDiagnostics(this StyledElement styledElement)
	{
		return styledElement.GetStyleDiagnosticsInternal();
	}
}
