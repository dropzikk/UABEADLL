using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing.Processors.Overlays;

namespace SixLabors.ImageSharp.Processing.Processors.Filters;

public sealed class LomographProcessor : FilterProcessor
{
	public GraphicsOptions GraphicsOptions { get; }

	public LomographProcessor(GraphicsOptions graphicsOptions)
		: base(KnownFilterMatrices.LomographFilter)
	{
		GraphicsOptions = graphicsOptions;
	}

	public override IImageProcessor<TPixel> CreatePixelSpecificProcessor<TPixel>(Configuration configuration, Image<TPixel> source, Rectangle sourceRectangle)
	{
		return new LomographProcessor<TPixel>(configuration, this, source, sourceRectangle);
	}
}
internal class LomographProcessor<TPixel> : FilterProcessor<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
	private static readonly Color VeryDarkGreen = Color.FromRgba(0, 10, 0, byte.MaxValue);

	private readonly LomographProcessor definition;

	public LomographProcessor(Configuration configuration, LomographProcessor definition, Image<TPixel> source, Rectangle sourceRectangle)
		: base(configuration, (FilterProcessor)definition, source, sourceRectangle)
	{
		this.definition = definition;
	}

	protected override void AfterImageApply()
	{
		new VignetteProcessor(definition.GraphicsOptions, VeryDarkGreen).Execute(base.Configuration, base.Source, base.SourceRectangle);
		base.AfterImageApply();
	}
}
