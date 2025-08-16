namespace AssetRipper.TextureDecoder.Rgb.Formats;

public struct ColorA8 : IColor<byte>
{
	public byte R
	{
		get
		{
			return 0;
		}
		set
		{
		}
	}

	public byte G
	{
		get
		{
			return 0;
		}
		set
		{
		}
	}

	public byte B
	{
		get
		{
			return 0;
		}
		set
		{
		}
	}

	public byte A { get; set; }

	public void GetChannels(out byte r, out byte g, out byte b, out byte a)
	{
		DefaultColorMethods.GetChannels<ColorA8, byte>(this, out r, out g, out b, out a);
	}

	public void SetChannels(byte r, byte g, byte b, byte a)
	{
		DefaultColorMethods.SetChannels(ref this, r, g, b, a);
	}
}
