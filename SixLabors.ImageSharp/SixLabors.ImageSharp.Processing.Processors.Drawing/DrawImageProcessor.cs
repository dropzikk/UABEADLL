using System;
using System.Runtime.CompilerServices;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Processing.Processors.Drawing;

public class DrawImageProcessor : IImageProcessor
{
	private class ProcessorFactoryVisitor<TPixelBg> : IImageVisitor where TPixelBg : unmanaged, IPixel<TPixelBg>
	{
		private readonly Configuration configuration;

		private readonly DrawImageProcessor definition;

		private readonly Image<TPixelBg> source;

		public IImageProcessor<TPixelBg>? Result { get; private set; }

		public ProcessorFactoryVisitor(Configuration configuration, DrawImageProcessor definition, Image<TPixelBg> source)
		{
			this.configuration = configuration;
			this.definition = definition;
			this.source = source;
		}

		public void Visit<TPixelFg>(Image<TPixelFg> image) where TPixelFg : unmanaged, IPixel<TPixelFg>
		{
			Result = new DrawImageProcessor<TPixelBg, TPixelFg>(configuration, image, source, definition.BackgroundLocation, definition.ForegroundRectangle, definition.ColorBlendingMode, definition.AlphaCompositionMode, definition.Opacity);
		}
	}

	public Image ForeGround { get; }

	public Point BackgroundLocation { get; }

	public Rectangle ForegroundRectangle { get; }

	public PixelColorBlendingMode ColorBlendingMode { get; }

	public PixelAlphaCompositionMode AlphaCompositionMode { get; }

	public float Opacity { get; }

	public DrawImageProcessor(Image foreground, Point backgroundLocation, PixelColorBlendingMode colorBlendingMode, PixelAlphaCompositionMode alphaCompositionMode, float opacity)
		: this(foreground, backgroundLocation, foreground.Bounds, colorBlendingMode, alphaCompositionMode, opacity)
	{
	}

	public DrawImageProcessor(Image foreground, Point backgroundLocation, Rectangle foregroundRectangle, PixelColorBlendingMode colorBlendingMode, PixelAlphaCompositionMode alphaCompositionMode, float opacity)
	{
		ForeGround = foreground;
		BackgroundLocation = backgroundLocation;
		ForegroundRectangle = foregroundRectangle;
		ColorBlendingMode = colorBlendingMode;
		AlphaCompositionMode = alphaCompositionMode;
		Opacity = opacity;
	}

	public IImageProcessor<TPixelBg> CreatePixelSpecificProcessor<TPixelBg>(Configuration configuration, Image<TPixelBg> source, Rectangle sourceRectangle) where TPixelBg : unmanaged, IPixel<TPixelBg>
	{
		ProcessorFactoryVisitor<TPixelBg> processorFactoryVisitor = new ProcessorFactoryVisitor<TPixelBg>(configuration, this, source);
		ForeGround.AcceptVisitor(processorFactoryVisitor);
		return processorFactoryVisitor.Result;
	}
}
internal class DrawImageProcessor<TPixelBg, TPixelFg> : ImageProcessor<TPixelBg> where TPixelBg : unmanaged, IPixel<TPixelBg> where TPixelFg : unmanaged, IPixel<TPixelFg>
{
	private readonly struct RowOperation : IRowOperation
	{
		private readonly Buffer2D<TPixelBg> background;

		private readonly Buffer2D<TPixelFg> foreground;

		private readonly PixelBlender<TPixelBg> blender;

		private readonly Configuration configuration;

		private readonly Rectangle foregroundRectangle;

		private readonly Rectangle backgroundRectangle;

		private readonly float opacity;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public RowOperation(Configuration configuration, Buffer2D<TPixelBg> background, Buffer2D<TPixelFg> foreground, Rectangle backgroundRectangle, Rectangle foregroundRectangle, PixelBlender<TPixelBg> blender, float opacity)
		{
			this.configuration = configuration;
			this.background = background;
			this.foreground = foreground;
			this.backgroundRectangle = backgroundRectangle;
			this.foregroundRectangle = foregroundRectangle;
			this.blender = blender;
			this.opacity = opacity;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Invoke(int y)
		{
			Span<TPixelBg> span = background.DangerousGetRowSpan(y + backgroundRectangle.Top).Slice(backgroundRectangle.Left, backgroundRectangle.Width);
			Span<TPixelFg> span2 = foreground.DangerousGetRowSpan(y + foregroundRectangle.Top).Slice(foregroundRectangle.Left, foregroundRectangle.Width);
			blender.Blend<TPixelFg>(configuration, span, span, span2, opacity);
		}
	}

	public Image<TPixelFg> ForegroundImage { get; }

	public Rectangle ForegroundRectangle { get; }

	public float Opacity { get; }

	public PixelBlender<TPixelBg> Blender { get; }

	public Point BackgroundLocation { get; }

	public DrawImageProcessor(Configuration configuration, Image<TPixelFg> foregroundImage, Image<TPixelBg> backgroundImage, Point backgroundLocation, Rectangle foregroundRectangle, PixelColorBlendingMode colorBlendingMode, PixelAlphaCompositionMode alphaCompositionMode, float opacity)
		: base(configuration, backgroundImage, backgroundImage.Bounds)
	{
		Guard.MustBeBetweenOrEqualTo(opacity, 0f, 1f, "opacity");
		ForegroundImage = foregroundImage;
		ForegroundRectangle = foregroundRectangle;
		Opacity = opacity;
		Blender = PixelOperations<TPixelBg>.Instance.GetPixelBlender(colorBlendingMode, alphaCompositionMode);
		BackgroundLocation = backgroundLocation;
	}

	protected override void OnFrameApply(ImageFrame<TPixelBg> source)
	{
		Rectangle foregroundRectangle = ForegroundRectangle;
		int num = BackgroundLocation.X;
		int num2 = BackgroundLocation.Y;
		if (BackgroundLocation.X < 0)
		{
			foregroundRectangle.Width += BackgroundLocation.X;
			foregroundRectangle.X -= BackgroundLocation.X;
			num = 0;
		}
		if (BackgroundLocation.Y < 0)
		{
			foregroundRectangle.Height += BackgroundLocation.Y;
			foregroundRectangle.Y -= BackgroundLocation.Y;
			num2 = 0;
		}
		foregroundRectangle.Width = Math.Min(source.Width - num, foregroundRectangle.Width);
		foregroundRectangle.Height = Math.Min(source.Height - num2, foregroundRectangle.Height);
		foregroundRectangle = Rectangle.Intersect(foregroundRectangle, ForegroundImage.Bounds);
		int width = foregroundRectangle.Width;
		int height = foregroundRectangle.Height;
		if (width > 0 && height > 0)
		{
			Rectangle backgroundRectangle = Rectangle.Intersect(new Rectangle(num, num2, width, height), base.SourceRectangle);
			Configuration configuration = base.Configuration;
			RowOperation operation = new RowOperation(configuration, source.PixelBuffer, ForegroundImage.Frames.RootFrame.PixelBuffer, backgroundRectangle, foregroundRectangle, Blender, Opacity);
			ParallelRowIterator.IterateRows(configuration, new Rectangle(0, 0, foregroundRectangle.Width, foregroundRectangle.Height), in operation);
		}
	}
}
