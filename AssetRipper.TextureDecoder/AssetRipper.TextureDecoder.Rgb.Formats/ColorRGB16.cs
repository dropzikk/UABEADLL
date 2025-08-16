namespace AssetRipper.TextureDecoder.Rgb.Formats;

public struct ColorRGB16 : IColor<byte>
{
	private ushort bits;

	public byte R
	{
		get
		{
			return (byte)((bits >>> 8) & 0xF8);
		}
		set
		{
			bits = (ushort)(((value << 8) & 0xF800) | (bits & -63489));
		}
	}

	public byte G
	{
		get
		{
			return (byte)((bits >>> 3) & 0xFC);
		}
		set
		{
			bits = (ushort)(((value << 3) & 0x7E0) | (bits & -2017));
		}
	}

	public byte B
	{
		get
		{
			return (byte)((bits << 3) & 0xF8);
		}
		set
		{
			bits = (ushort)(((value >>> 3) & 0x1F) | (bits & -32));
		}
	}

	public byte A
	{
		get
		{
			return byte.MaxValue;
		}
		set
		{
		}
	}

	public void GetChannels(out byte r, out byte g, out byte b, out byte a)
	{
		DefaultColorMethods.GetChannels<ColorRGB16, byte>(this, out r, out g, out b, out a);
	}

	public void SetChannels(byte r, byte g, byte b, byte a)
	{
		bits = (ushort)(((r & 0xF8) << 8) | ((g & 0xFC) << 3) | ((b & 0xF8) >>> 3));
	}
}
