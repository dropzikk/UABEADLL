using System.Runtime.CompilerServices;
using System.Text;

namespace Avalonia.Platform;

public record struct PixelFormat(PixelFormatEnum format)
{
	public int BitsPerPixel
	{
		get
		{
			if (format == PixelFormatEnum.BlackWhite)
			{
				return 1;
			}
			if (format == PixelFormatEnum.Gray2)
			{
				return 2;
			}
			if (format == PixelFormatEnum.Gray4)
			{
				return 4;
			}
			if (format == PixelFormatEnum.Gray8)
			{
				return 8;
			}
			if (format == PixelFormatEnum.Rgb565 || format == PixelFormatEnum.Gray16)
			{
				return 16;
			}
			PixelFormatEnum formatEnum = format;
			if (formatEnum == PixelFormatEnum.Bgr24 || formatEnum == PixelFormatEnum.Rgb24)
			{
				return 24;
			}
			if (format == PixelFormatEnum.Rgba64)
			{
				return 64;
			}
			return 32;
		}
	}

	internal bool HasAlpha
	{
		get
		{
			if (format != PixelFormatEnum.Rgba8888 && format != PixelFormatEnum.Bgra8888)
			{
				return format == PixelFormatEnum.Rgba64;
			}
			return true;
		}
	}

	public static PixelFormat Rgb565 => PixelFormats.Rgb565;

	public static PixelFormat Rgba8888 => PixelFormats.Rgba8888;

	public static PixelFormat Bgra8888 => PixelFormats.Bgra8888;

	internal PixelFormatEnum FormatEnum = format;

	public override string ToString()
	{
		return format.ToString();
	}

	[CompilerGenerated]
	private bool PrintMembers(StringBuilder builder)
	{
		builder.Append("BitsPerPixel = ");
		builder.Append(BitsPerPixel.ToString());
		return true;
	}
}
