using System.Collections.Generic;

namespace Avalonia;

public class AvaloniaNativePlatformOptions
{
	public IReadOnlyList<AvaloniaNativeRenderingMode> RenderingMode { get; set; } = new AvaloniaNativeRenderingMode[2]
	{
		AvaloniaNativeRenderingMode.OpenGl,
		AvaloniaNativeRenderingMode.Software
	};

	public bool OverlayPopups { get; set; }

	public string AvaloniaNativeLibraryPath { get; set; }
}
