namespace SixLabors.ImageSharp.Formats.Tga;

public static class TgaImageTypeExtensions
{
	public static bool IsRunLengthEncoded(this TgaImageType imageType)
	{
		if (imageType == TgaImageType.RleColorMapped || imageType == TgaImageType.RleBlackAndWhite || imageType == TgaImageType.RleTrueColor)
		{
			return true;
		}
		return false;
	}

	public static bool IsValid(this TgaImageType imageType)
	{
		if (imageType <= TgaImageType.BlackAndWhite || imageType - 9 <= TgaImageType.TrueColor)
		{
			return true;
		}
		return false;
	}
}
