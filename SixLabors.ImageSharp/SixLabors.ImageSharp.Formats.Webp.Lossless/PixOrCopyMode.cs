namespace SixLabors.ImageSharp.Formats.Webp.Lossless;

internal enum PixOrCopyMode : byte
{
	Literal,
	CacheIdx,
	Copy,
	None
}
