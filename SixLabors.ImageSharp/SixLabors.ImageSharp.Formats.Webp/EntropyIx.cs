namespace SixLabors.ImageSharp.Formats.Webp;

internal enum EntropyIx : byte
{
	Direct,
	Spatial,
	SubGreen,
	SpatialSubGreen,
	Palette,
	PaletteAndSpatial,
	NumEntropyIx
}
