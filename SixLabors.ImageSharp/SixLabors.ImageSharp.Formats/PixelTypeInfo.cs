using System.Runtime.CompilerServices;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats;

public class PixelTypeInfo
{
	public int BitsPerPixel { get; }

	public PixelAlphaRepresentation? AlphaRepresentation { get; }

	public PixelTypeInfo(int bitsPerPixel)
	{
		BitsPerPixel = bitsPerPixel;
	}

	public PixelTypeInfo(int bitsPerPixel, PixelAlphaRepresentation alpha)
	{
		BitsPerPixel = bitsPerPixel;
		AlphaRepresentation = alpha;
	}

	internal static PixelTypeInfo Create<TPixel>() where TPixel : unmanaged, IPixel<TPixel>
	{
		return new PixelTypeInfo(Unsafe.SizeOf<TPixel>() * 8);
	}

	internal static PixelTypeInfo Create<TPixel>(PixelAlphaRepresentation alpha) where TPixel : unmanaged, IPixel<TPixel>
	{
		return new PixelTypeInfo(Unsafe.SizeOf<TPixel>() * 8, alpha);
	}
}
