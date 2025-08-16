namespace SixLabors.ImageSharp.Formats.Webp.Lossy;

internal class Vp8PictureHeader
{
	public uint Width { get; set; }

	public uint Height { get; set; }

	public sbyte XScale { get; set; }

	public sbyte YScale { get; set; }

	public sbyte ColorSpace { get; set; }

	public sbyte ClampType { get; set; }
}
