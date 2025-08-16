using SixLabors.ImageSharp.Processing.Processors;
using SixLabors.ImageSharp.Processing.Processors.Filters;

namespace SixLabors.ImageSharp.Processing;

public static class ColorBlindnessExtensions
{
	public static IImageProcessingContext ColorBlindness(this IImageProcessingContext source, ColorBlindnessMode colorBlindness)
	{
		return source.ApplyProcessor(GetProcessor(colorBlindness));
	}

	public static IImageProcessingContext ColorBlindness(this IImageProcessingContext source, ColorBlindnessMode colorBlindnessMode, Rectangle rectangle)
	{
		return source.ApplyProcessor(GetProcessor(colorBlindnessMode), rectangle);
	}

	private static IImageProcessor GetProcessor(ColorBlindnessMode colorBlindness)
	{
		return colorBlindness switch
		{
			ColorBlindnessMode.Achromatomaly => new AchromatomalyProcessor(), 
			ColorBlindnessMode.Achromatopsia => new AchromatopsiaProcessor(), 
			ColorBlindnessMode.Deuteranomaly => new DeuteranomalyProcessor(), 
			ColorBlindnessMode.Deuteranopia => new DeuteranopiaProcessor(), 
			ColorBlindnessMode.Protanomaly => new ProtanomalyProcessor(), 
			ColorBlindnessMode.Protanopia => new ProtanopiaProcessor(), 
			ColorBlindnessMode.Tritanomaly => new TritanomalyProcessor(), 
			_ => new TritanopiaProcessor(), 
		};
	}
}
