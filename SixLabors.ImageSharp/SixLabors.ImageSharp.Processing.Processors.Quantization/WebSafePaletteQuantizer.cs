namespace SixLabors.ImageSharp.Processing.Processors.Quantization;

public class WebSafePaletteQuantizer : PaletteQuantizer
{
	public WebSafePaletteQuantizer()
		: this(new QuantizerOptions())
	{
	}

	public WebSafePaletteQuantizer(QuantizerOptions options)
		: base(Color.WebSafePalette, options)
	{
	}
}
