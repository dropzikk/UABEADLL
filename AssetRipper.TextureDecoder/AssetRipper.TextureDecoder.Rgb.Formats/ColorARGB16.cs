namespace AssetRipper.TextureDecoder.Rgb.Formats;

public struct ColorARGB16 : IColor<byte>
{
	private byte gb;

	private byte ar;

	public byte B
	{
		get
		{
			return (byte)(gb << 4);
		}
		set
		{
			gb = (byte)((value >>> 4) | (gb & 0xF0));
		}
	}

	public byte G
	{
		get
		{
			return (byte)(gb & 0xF0);
		}
		set
		{
			gb = (byte)((value & 0xF0) | (gb & 0xF));
		}
	}

	public byte R
	{
		get
		{
			return (byte)(ar << 4);
		}
		set
		{
			ar = (byte)((value >>> 4) | (ar & 0xF0));
		}
	}

	public byte A
	{
		get
		{
			return (byte)(ar & 0xF0);
		}
		set
		{
			ar = (byte)((value & 0xF0) | (ar & 0xF));
		}
	}

	public void GetChannels(out byte r, out byte g, out byte b, out byte a)
	{
		DefaultColorMethods.GetChannels<ColorARGB16, byte>(this, out r, out g, out b, out a);
	}

	public void SetChannels(byte r, byte g, byte b, byte a)
	{
		DefaultColorMethods.SetChannels(ref this, r, g, b, a);
	}
}
