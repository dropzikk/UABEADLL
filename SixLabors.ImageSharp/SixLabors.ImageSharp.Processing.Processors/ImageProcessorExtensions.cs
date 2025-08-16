using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Processing.Processors;

internal static class ImageProcessorExtensions
{
	private class ExecuteVisitor : IImageVisitor
	{
		private readonly Configuration configuration;

		private readonly IImageProcessor processor;

		private readonly Rectangle sourceRectangle;

		public ExecuteVisitor(Configuration configuration, IImageProcessor processor, Rectangle sourceRectangle)
		{
			this.configuration = configuration;
			this.processor = processor;
			this.sourceRectangle = sourceRectangle;
		}

		public void Visit<TPixel>(Image<TPixel> image) where TPixel : unmanaged, IPixel<TPixel>
		{
			using IImageProcessor<TPixel> imageProcessor = processor.CreatePixelSpecificProcessor(configuration, image, sourceRectangle);
			imageProcessor.Execute();
		}
	}

	public static void Execute(this IImageProcessor processor, Configuration configuration, Image source, Rectangle sourceRectangle)
	{
		source.AcceptVisitor(new ExecuteVisitor(configuration, processor, sourceRectangle));
	}
}
