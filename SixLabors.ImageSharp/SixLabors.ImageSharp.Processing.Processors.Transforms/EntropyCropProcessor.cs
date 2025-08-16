using System;
using System.Runtime.CompilerServices;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing.Processors.Binarization;
using SixLabors.ImageSharp.Processing.Processors.Convolution;

namespace SixLabors.ImageSharp.Processing.Processors.Transforms;

public sealed class EntropyCropProcessor : IImageProcessor
{
	public float Threshold { get; }

	public EntropyCropProcessor()
		: this(0.5f)
	{
	}

	public EntropyCropProcessor(float threshold)
	{
		Guard.MustBeBetweenOrEqualTo(threshold, 0f, 1f, "threshold");
		Threshold = threshold;
	}

	public IImageProcessor<TPixel> CreatePixelSpecificProcessor<TPixel>(Configuration configuration, Image<TPixel> source, Rectangle sourceRectangle) where TPixel : unmanaged, IPixel<TPixel>
	{
		return new EntropyCropProcessor<TPixel>(configuration, this, source, sourceRectangle);
	}
}
internal class EntropyCropProcessor<TPixel> : ImageProcessor<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
	private readonly EntropyCropProcessor definition;

	public EntropyCropProcessor(Configuration configuration, EntropyCropProcessor definition, Image<TPixel> source, Rectangle sourceRectangle)
		: base(configuration, source, sourceRectangle)
	{
		this.definition = definition;
	}

	protected override void BeforeImageApply()
	{
		Rectangle filteredBoundingRectangle;
		using (Image<TPixel> image = new Image<TPixel>(base.Configuration, base.Source.Metadata.DeepClone(), new ImageFrame<TPixel>[1] { base.Source.Frames.RootFrame.Clone() }))
		{
			_ = base.Source.Configuration;
			new EdgeDetector2DProcessor(KnownEdgeDetectorKernels.Sobel, grayscale: false).Execute(base.Configuration, image, base.SourceRectangle);
			new BinaryThresholdProcessor(definition.Threshold).Execute(base.Configuration, image, base.SourceRectangle);
			filteredBoundingRectangle = GetFilteredBoundingRectangle(image.Frames.RootFrame, 0f);
		}
		new CropProcessor(filteredBoundingRectangle, base.Source.Size).Execute(base.Configuration, base.Source, base.SourceRectangle);
		base.BeforeImageApply();
	}

	protected override void OnFrameApply(ImageFrame<TPixel> source)
	{
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static Rectangle GetBoundingRectangle(Point topLeft, Point bottomRight)
	{
		return new Rectangle(topLeft.X, topLeft.Y, bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y);
	}

	private static Rectangle GetFilteredBoundingRectangle(ImageFrame<TPixel> bitmap, float componentValue, RgbaComponent channel = RgbaComponent.B)
	{
		int width = bitmap.Width;
		int height = bitmap.Height;
		Point topLeft = default(Point);
		Point bottomRight = default(Point);
		Func<ImageFrame<TPixel>, int, int, float, bool> delegateFunc = channel switch
		{
			RgbaComponent.R => (ImageFrame<TPixel> pixels, int x, int y, float b) => MathF.Abs(pixels[x, y].ToVector4().X - b) > Constants.Epsilon, 
			RgbaComponent.G => (ImageFrame<TPixel> pixels, int x, int y, float b) => MathF.Abs(pixels[x, y].ToVector4().Y - b) > Constants.Epsilon, 
			RgbaComponent.B => (ImageFrame<TPixel> pixels, int x, int y, float b) => MathF.Abs(pixels[x, y].ToVector4().Z - b) > Constants.Epsilon, 
			_ => (ImageFrame<TPixel> pixels, int x, int y, float b) => MathF.Abs(pixels[x, y].ToVector4().W - b) > Constants.Epsilon, 
		};
		topLeft.Y = GetMinY(bitmap);
		topLeft.X = GetMinX(bitmap);
		bottomRight.Y = Numerics.Clamp(GetMaxY(bitmap) + 1, 0, height);
		bottomRight.X = Numerics.Clamp(GetMaxX(bitmap) + 1, 0, width);
		return GetBoundingRectangle(topLeft, bottomRight);
		int GetMaxX(ImageFrame<TPixel> pixels)
		{
			for (int num2 = width - 1; num2 > -1; num2--)
			{
				for (int n = 0; n < height; n++)
				{
					if (delegateFunc(pixels, num2, n, componentValue))
					{
						return num2;
					}
				}
			}
			return width;
		}
		int GetMaxY(ImageFrame<TPixel> pixels)
		{
			for (int num = height - 1; num > -1; num--)
			{
				for (int m = 0; m < width; m++)
				{
					if (delegateFunc(pixels, m, num, componentValue))
					{
						return num;
					}
				}
			}
			return height;
		}
		int GetMinX(ImageFrame<TPixel> pixels)
		{
			for (int k = 0; k < width; k++)
			{
				for (int l = 0; l < height; l++)
				{
					if (delegateFunc(pixels, k, l, componentValue))
					{
						return k;
					}
				}
			}
			return 0;
		}
		int GetMinY(ImageFrame<TPixel> pixels)
		{
			for (int i = 0; i < height; i++)
			{
				for (int j = 0; j < width; j++)
				{
					if (delegateFunc(pixels, j, i, componentValue))
					{
						return i;
					}
				}
			}
			return 0;
		}
	}
}
