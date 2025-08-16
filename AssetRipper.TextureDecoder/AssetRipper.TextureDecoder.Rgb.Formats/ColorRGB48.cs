namespace AssetRipper.TextureDecoder.Rgb.Formats;

public struct ColorRGB48 : IColor<ushort>
{
	public ushort R { get; set; }

	public ushort G { get; set; }

	public ushort B { get; set; }

	public ushort A
	{
		get
		{
			return ushort.MaxValue;
		}
		set
		{
		}
	}

	public void GetChannels(out ushort r, out ushort g, out ushort b, out ushort a)
	{
		DefaultColorMethods.GetChannels<ColorRGB48, ushort>(this, out r, out g, out b, out a);
	}

	public void SetChannels(ushort r, ushort g, ushort b, ushort a)
	{
		DefaultColorMethods.SetChannels(ref this, r, g, b, a);
	}
}
