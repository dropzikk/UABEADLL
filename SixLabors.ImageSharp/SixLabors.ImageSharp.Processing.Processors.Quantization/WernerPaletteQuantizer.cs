namespace SixLabors.ImageSharp.Processing.Processors.Quantization;

public class WernerPaletteQuantizer : PaletteQuantizer
{
	public WernerPaletteQuantizer()
		: this(new QuantizerOptions())
	{
	}

	public WernerPaletteQuantizer(QuantizerOptions options)
		: base(Color.WernerPalette, options)
	{
	}
}
