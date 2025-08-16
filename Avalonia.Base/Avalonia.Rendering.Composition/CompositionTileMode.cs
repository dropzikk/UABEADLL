using System;

namespace Avalonia.Rendering.Composition;

[Flags]
public enum CompositionTileMode
{
	None = 0,
	TileX = 1,
	TileY = 2,
	FlipX = 4,
	FlipY = 8,
	Tile = 3,
	Flip = 0xC
}
