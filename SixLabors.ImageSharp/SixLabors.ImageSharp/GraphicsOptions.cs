using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp;

public class GraphicsOptions : IDeepCloneable<GraphicsOptions>
{
	private int antialiasSubpixelDepth = 16;

	private float blendPercentage = 1f;

	public bool Antialias { get; set; } = true;

	public int AntialiasSubpixelDepth
	{
		get
		{
			return antialiasSubpixelDepth;
		}
		set
		{
			Guard.MustBeGreaterThanOrEqualTo(value, 0, "AntialiasSubpixelDepth");
			antialiasSubpixelDepth = value;
		}
	}

	public float BlendPercentage
	{
		get
		{
			return blendPercentage;
		}
		set
		{
			Guard.MustBeBetweenOrEqualTo(value, 0f, 1f, "BlendPercentage");
			blendPercentage = value;
		}
	}

	public PixelColorBlendingMode ColorBlendingMode { get; set; }

	public PixelAlphaCompositionMode AlphaCompositionMode { get; set; }

	public GraphicsOptions()
	{
	}

	private GraphicsOptions(GraphicsOptions source)
	{
		AlphaCompositionMode = source.AlphaCompositionMode;
		Antialias = source.Antialias;
		AntialiasSubpixelDepth = source.AntialiasSubpixelDepth;
		BlendPercentage = source.BlendPercentage;
		ColorBlendingMode = source.ColorBlendingMode;
	}

	public GraphicsOptions DeepClone()
	{
		return new GraphicsOptions(this);
	}
}
