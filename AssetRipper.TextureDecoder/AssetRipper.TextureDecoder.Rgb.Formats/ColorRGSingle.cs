namespace AssetRipper.TextureDecoder.Rgb.Formats;

public struct ColorRGSingle : IColor<float>
{
	public float R { get; set; }

	public float G { get; set; }

	public float B
	{
		get
		{
			return 0f;
		}
		set
		{
		}
	}

	public float A
	{
		get
		{
			return 1f;
		}
		set
		{
		}
	}

	public void GetChannels(out float r, out float g, out float b, out float a)
	{
		DefaultColorMethods.GetChannels<ColorRGSingle, float>(this, out r, out g, out b, out a);
	}

	public void SetChannels(float r, float g, float b, float a)
	{
		DefaultColorMethods.SetChannels(ref this, r, g, b, a);
	}
}
