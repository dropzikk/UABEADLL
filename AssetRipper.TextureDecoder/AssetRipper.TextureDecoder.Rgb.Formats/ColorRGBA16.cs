namespace AssetRipper.TextureDecoder.Rgb.Formats;

public struct ColorRGBA16 : IColor<byte>
{
	private byte ba;

	private byte rg;

	public byte B
	{
		get
		{
			return (byte)(ba & 0xF0);
		}
		set
		{
			ba = (byte)((value & 0xF0) | (ba & 0xF));
		}
	}

	public byte G
	{
		get
		{
			return (byte)(rg << 4);
		}
		set
		{
			rg = (byte)((value >>> 4) | (rg & 0xF0));
		}
	}

	public byte R
	{
		get
		{
			return (byte)(rg & 0xF0);
		}
		set
		{
			rg = (byte)((value & 0xF0) | (rg & 0xF));
		}
	}

	public byte A
	{
		get
		{
			return (byte)(ba << 4);
		}
		set
		{
			ba = (byte)((value >>> 4) | (ba & 0xF0));
		}
	}

	public void GetChannels(out byte r, out byte g, out byte b, out byte a)
	{
		DefaultColorMethods.GetChannels<ColorRGBA16, byte>(this, out r, out g, out b, out a);
	}

	public void SetChannels(byte r, byte g, byte b, byte a)
	{
		DefaultColorMethods.SetChannels(ref this, r, g, b, a);
	}
}
