namespace SixLabors.ImageSharp.Formats.Tga;

public enum TgaImageType : byte
{
	NoImageData = 0,
	ColorMapped = 1,
	TrueColor = 2,
	BlackAndWhite = 3,
	RleColorMapped = 9,
	RleTrueColor = 10,
	RleBlackAndWhite = 11
}
