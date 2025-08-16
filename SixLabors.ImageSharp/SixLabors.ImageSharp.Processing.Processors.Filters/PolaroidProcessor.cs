using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing.Processors.Overlays;

namespace SixLabors.ImageSharp.Processing.Processors.Filters;

public sealed class PolaroidProcessor : FilterProcessor
{
	public GraphicsOptions GraphicsOptions { get; }

	public PolaroidProcessor(GraphicsOptions graphicsOptions)
		: base(KnownFilterMatrices.PolaroidFilter)
	{
		GraphicsOptions = graphicsOptions;
	}

	public override IImageProcessor<TPixel> CreatePixelSpecificProcessor<TPixel>(Configuration configuration, Image<TPixel> source, Rectangle sourceRectangle)
	{
		return new PolaroidProcessor<TPixel>(configuration, this, source, sourceRectangle);
	}
}
internal class PolaroidProcessor<TPixel> : FilterProcessor<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
	private static readonly Color LightOrange = Color.FromRgba(byte.MaxValue, 153, 102, 128);

	private static readonly Color VeryDarkOrange = Color.FromRgb(102, 34, 0);

	private readonly PolaroidProcessor definition;

	public PolaroidProcessor(Configuration configuration, PolaroidProcessor definition, Image<TPixel> source, Rectangle sourceRectangle)
		: base(configuration, (FilterProcessor)definition, source, sourceRectangle)
	{
		this.definition = definition;
	}

	protected override void AfterImageApply()
	{
		new VignetteProcessor(definition.GraphicsOptions, VeryDarkOrange).Execute(base.Configuration, base.Source, base.SourceRectangle);
		new GlowProcessor(definition.GraphicsOptions, LightOrange, (float)base.Source.Width / 4f).Execute(base.Configuration, base.Source, base.SourceRectangle);
		base.AfterImageApply();
	}
}
