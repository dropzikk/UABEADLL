using System;

namespace Avalonia;

public enum AvaloniaNativeRenderingMode
{
	OpenGl = 1,
	Software,
	[Obsolete("Experimental, unstable, not for production usage")]
	Metal
}
