using System;

namespace AssetRipper.TextureDecoder.Rgb.Formats;

public struct ColorRHalf : IColor<Half>
{
	public Half R { get; set; }

	public Half G
	{
		get
		{
			return default(Half);
		}
		set
		{
		}
	}

	public Half B
	{
		get
		{
			return default(Half);
		}
		set
		{
		}
	}

	public Half A
	{
		get
		{
			return (Half)1f;
		}
		set
		{
		}
	}

	public void GetChannels(out Half r, out Half g, out Half b, out Half a)
	{
		DefaultColorMethods.GetChannels<ColorRHalf, Half>(this, out r, out g, out b, out a);
	}

	public void SetChannels(Half r, Half g, Half b, Half a)
	{
		DefaultColorMethods.SetChannels(ref this, r, g, b, a);
	}
}
