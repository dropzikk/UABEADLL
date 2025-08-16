namespace AssetRipper.TextureDecoder.Rgb.Formats;

public struct ColorRG32 : IColor<ushort>
{
	public ushort R { get; set; }

	public ushort G { get; set; }

	public ushort B
	{
		get
		{
			return 0;
		}
		set
		{
		}
	}

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
		DefaultColorMethods.GetChannels<ColorRG32, ushort>(this, out r, out g, out b, out a);
	}

	public void SetChannels(ushort r, ushort g, ushort b, ushort a)
	{
		DefaultColorMethods.SetChannels(ref this, r, g, b, a);
	}
}
