using System.Collections.Generic;

namespace Avalonia.Diagnostics;

public class StyleDiagnostics
{
	public IReadOnlyList<AppliedStyle> AppliedStyles { get; }

	public StyleDiagnostics(IReadOnlyList<AppliedStyle> appliedStyles)
	{
		AppliedStyles = appliedStyles;
	}
}
