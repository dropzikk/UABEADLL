using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.Metadata;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp;

public abstract class ImageFrame : IConfigurationProvider, IDisposable
{
	public int Width { get; private set; }

	public int Height { get; private set; }

	public ImageFrameMetadata Metadata { get; }

	public Configuration Configuration { get; }

	protected ImageFrame(Configuration configuration, int width, int height, ImageFrameMetadata metadata)
	{
		Configuration = configuration;
		Width = width;
		Height = height;
		Metadata = metadata;
	}

	public Size Size()
	{
		return new Size(Width, Height);
	}

	public Rectangle Bounds()
	{
		return new Rectangle(0, 0, Width, Height);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected abstract void Dispose(bool disposing);

	internal abstract void CopyPixelsTo<TDestinationPixel>(MemoryGroup<TDestinationPixel> destination) where TDestinationPixel : unmanaged, IPixel<TDestinationPixel>;

	internal void UpdateSize(Size size)
	{
		Width = size.Width;
		Height = size.Height;
	}

	internal static ImageFrame<TPixel> LoadPixelData<TPixel>(Configuration configuration, ReadOnlySpan<byte> data, int width, int height) where TPixel : unmanaged, IPixel<TPixel>
	{
		return LoadPixelData(configuration, MemoryMarshal.Cast<byte, TPixel>(data), width, height);
	}

	internal static ImageFrame<TPixel> LoadPixelData<TPixel>(Configuration configuration, ReadOnlySpan<TPixel> data, int width, int height) where TPixel : unmanaged, IPixel<TPixel>
	{
		int num = width * height;
		Guard.MustBeGreaterThanOrEqualTo(data.Length, num, "data");
		ImageFrame<TPixel> imageFrame = new ImageFrame<TPixel>(configuration, width, height);
		data = data.Slice(0, num);
		data.CopyTo(imageFrame.PixelBuffer.FastMemoryGroup);
		return imageFrame;
	}
}
public sealed class ImageFrame<TPixel> : ImageFrame, IPixelSource<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
	private readonly struct RowIntervalOperation<TPixel2> : IRowIntervalOperation where TPixel2 : unmanaged, IPixel<TPixel2>
	{
		private readonly Buffer2D<TPixel> source;

		private readonly Buffer2D<TPixel2> target;

		private readonly Configuration configuration;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public RowIntervalOperation(Buffer2D<TPixel> source, Buffer2D<TPixel2> target, Configuration configuration)
		{
			this.source = source;
			this.target = target;
			this.configuration = configuration;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Invoke(in RowInterval rows)
		{
			for (int i = rows.Min; i < rows.Max; i++)
			{
				Span<TPixel> span = source.DangerousGetRowSpan(i);
				Span<TPixel2> destinationPixels = target.DangerousGetRowSpan(i);
				PixelOperations<TPixel>.Instance.To(configuration, span, destinationPixels);
			}
		}

		void IRowIntervalOperation.Invoke(in RowInterval rows)
		{
			Invoke(in rows);
		}
	}

	private bool isDisposed;

	public Buffer2D<TPixel> PixelBuffer { get; }

	public TPixel this[int x, int y]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			VerifyCoords(x, y);
			return PixelBuffer.GetElementUnsafe(x, y);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			VerifyCoords(x, y);
			PixelBuffer.GetElementUnsafe(x, y) = value;
		}
	}

	internal ImageFrame(Configuration configuration, Size size)
		: this(configuration, size.Width, size.Height, new ImageFrameMetadata())
	{
	}

	internal ImageFrame(Configuration configuration, int width, int height)
		: this(configuration, width, height, new ImageFrameMetadata())
	{
	}

	internal ImageFrame(Configuration configuration, Size size, ImageFrameMetadata metadata)
		: this(configuration, size.Width, size.Height, metadata)
	{
	}

	internal ImageFrame(Configuration configuration, int width, int height, ImageFrameMetadata metadata)
		: base(configuration, width, height, metadata)
	{
		Guard.MustBeGreaterThan(width, 0, "width");
		Guard.MustBeGreaterThan(height, 0, "height");
		PixelBuffer = base.Configuration.MemoryAllocator.Allocate2D<TPixel>(width, height, configuration.PreferContiguousImageBuffers, AllocationOptions.Clean);
	}

	internal ImageFrame(Configuration configuration, int width, int height, TPixel backgroundColor)
		: this(configuration, width, height, backgroundColor, new ImageFrameMetadata())
	{
	}

	internal ImageFrame(Configuration configuration, int width, int height, TPixel backgroundColor, ImageFrameMetadata metadata)
		: base(configuration, width, height, metadata)
	{
		Guard.MustBeGreaterThan(width, 0, "width");
		Guard.MustBeGreaterThan(height, 0, "height");
		PixelBuffer = base.Configuration.MemoryAllocator.Allocate2D<TPixel>(width, height, configuration.PreferContiguousImageBuffers);
		Clear(backgroundColor);
	}

	internal ImageFrame(Configuration configuration, int width, int height, MemoryGroup<TPixel> memorySource)
		: this(configuration, width, height, memorySource, new ImageFrameMetadata())
	{
	}

	internal ImageFrame(Configuration configuration, int width, int height, MemoryGroup<TPixel> memorySource, ImageFrameMetadata metadata)
		: base(configuration, width, height, metadata)
	{
		Guard.MustBeGreaterThan(width, 0, "width");
		Guard.MustBeGreaterThan(height, 0, "height");
		PixelBuffer = new Buffer2D<TPixel>(memorySource, width, height);
	}

	internal ImageFrame(Configuration configuration, ImageFrame<TPixel> source)
		: base(configuration, source.Width, source.Height, source.Metadata.DeepClone())
	{
		Guard.NotNull(configuration, "configuration");
		Guard.NotNull(source, "source");
		PixelBuffer = base.Configuration.MemoryAllocator.Allocate2D<TPixel>(source.PixelBuffer.Width, source.PixelBuffer.Height, configuration.PreferContiguousImageBuffers);
		source.PixelBuffer.FastMemoryGroup.CopyTo(PixelBuffer.FastMemoryGroup);
	}

	public void ProcessPixelRows(PixelAccessorAction<TPixel> processPixels)
	{
		Guard.NotNull(processPixels, "processPixels");
		PixelBuffer.FastMemoryGroup.IncreaseRefCounts();
		try
		{
			PixelAccessor<TPixel> pixelAccessor = new PixelAccessor<TPixel>(PixelBuffer);
			processPixels(pixelAccessor);
		}
		finally
		{
			PixelBuffer.FastMemoryGroup.DecreaseRefCounts();
		}
	}

	public void ProcessPixelRows<TPixel2>(ImageFrame<TPixel2> frame2, PixelAccessorAction<TPixel, TPixel2> processPixels) where TPixel2 : unmanaged, IPixel<TPixel2>
	{
		Guard.NotNull(frame2, "frame2");
		Guard.NotNull(processPixels, "processPixels");
		PixelBuffer.FastMemoryGroup.IncreaseRefCounts();
		frame2.PixelBuffer.FastMemoryGroup.IncreaseRefCounts();
		try
		{
			PixelAccessor<TPixel> pixelAccessor = new PixelAccessor<TPixel>(PixelBuffer);
			PixelAccessor<TPixel2> pixelAccessor2 = new PixelAccessor<TPixel2>(frame2.PixelBuffer);
			processPixels(pixelAccessor, pixelAccessor2);
		}
		finally
		{
			frame2.PixelBuffer.FastMemoryGroup.DecreaseRefCounts();
			PixelBuffer.FastMemoryGroup.DecreaseRefCounts();
		}
	}

	public void ProcessPixelRows<TPixel2, TPixel3>(ImageFrame<TPixel2> frame2, ImageFrame<TPixel3> frame3, PixelAccessorAction<TPixel, TPixel2, TPixel3> processPixels) where TPixel2 : unmanaged, IPixel<TPixel2> where TPixel3 : unmanaged, IPixel<TPixel3>
	{
		Guard.NotNull(frame2, "frame2");
		Guard.NotNull(frame3, "frame3");
		Guard.NotNull(processPixels, "processPixels");
		PixelBuffer.FastMemoryGroup.IncreaseRefCounts();
		frame2.PixelBuffer.FastMemoryGroup.IncreaseRefCounts();
		frame3.PixelBuffer.FastMemoryGroup.IncreaseRefCounts();
		try
		{
			PixelAccessor<TPixel> pixelAccessor = new PixelAccessor<TPixel>(PixelBuffer);
			PixelAccessor<TPixel2> pixelAccessor2 = new PixelAccessor<TPixel2>(frame2.PixelBuffer);
			PixelAccessor<TPixel3> pixelAccessor3 = new PixelAccessor<TPixel3>(frame3.PixelBuffer);
			processPixels(pixelAccessor, pixelAccessor2, pixelAccessor3);
		}
		finally
		{
			frame3.PixelBuffer.FastMemoryGroup.DecreaseRefCounts();
			frame2.PixelBuffer.FastMemoryGroup.DecreaseRefCounts();
			PixelBuffer.FastMemoryGroup.DecreaseRefCounts();
		}
	}

	public void CopyPixelDataTo(Span<TPixel> destination)
	{
		this.GetPixelMemoryGroup().CopyTo(destination);
	}

	public void CopyPixelDataTo(Span<byte> destination)
	{
		this.GetPixelMemoryGroup().CopyTo(MemoryMarshal.Cast<byte, TPixel>(destination));
	}

	public bool DangerousTryGetSinglePixelMemory(out Memory<TPixel> memory)
	{
		IMemoryGroup<TPixel> pixelMemoryGroup = this.GetPixelMemoryGroup();
		if (pixelMemoryGroup.Count > 1)
		{
			memory = default(Memory<TPixel>);
			return false;
		}
		memory = pixelMemoryGroup.Single();
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal ref TPixel GetPixelReference(int x, int y)
	{
		return ref PixelBuffer[x, y];
	}

	internal void CopyTo(Buffer2D<TPixel> target)
	{
		if (Size() != target.Size())
		{
			throw new ArgumentException("ImageFrame<TPixel>.CopyTo(): target must be of the same size!", "target");
		}
		PixelBuffer.FastMemoryGroup.CopyTo(target.FastMemoryGroup);
	}

	internal void SwapOrCopyPixelsBufferFrom(ImageFrame<TPixel> pixelSource)
	{
		Guard.NotNull(pixelSource, "pixelSource");
		Buffer2D<TPixel>.SwapOrCopyContent(PixelBuffer, pixelSource.PixelBuffer);
		UpdateSize(PixelBuffer.Size());
	}

	protected override void Dispose(bool disposing)
	{
		if (!isDisposed)
		{
			if (disposing)
			{
				PixelBuffer.Dispose();
			}
			isDisposed = true;
		}
	}

	internal override void CopyPixelsTo<TDestinationPixel>(MemoryGroup<TDestinationPixel> destination)
	{
		if (typeof(TPixel) == typeof(TDestinationPixel))
		{
			PixelBuffer.FastMemoryGroup.TransformTo(destination, delegate(ReadOnlySpan<TPixel> s, Span<TDestinationPixel> d)
			{
				Span<TPixel> destination2 = MemoryMarshal.Cast<TDestinationPixel, TPixel>(d);
				s.CopyTo(destination2);
			});
		}
		else
		{
			PixelBuffer.FastMemoryGroup.TransformTo(destination, delegate(ReadOnlySpan<TPixel> s, Span<TDestinationPixel> d)
			{
				PixelOperations<TPixel>.Instance.To(base.Configuration, s, d);
			});
		}
	}

	public override string ToString()
	{
		return $"ImageFrame<{typeof(TPixel).Name}>({base.Width}x{base.Height})";
	}

	internal ImageFrame<TPixel> Clone()
	{
		return Clone(base.Configuration);
	}

	internal ImageFrame<TPixel> Clone(Configuration configuration)
	{
		return new ImageFrame<TPixel>(configuration, this);
	}

	internal ImageFrame<TPixel2>? CloneAs<TPixel2>() where TPixel2 : unmanaged, IPixel<TPixel2>
	{
		return CloneAs<TPixel2>(base.Configuration);
	}

	internal ImageFrame<TPixel2> CloneAs<TPixel2>(Configuration configuration) where TPixel2 : unmanaged, IPixel<TPixel2>
	{
		if (typeof(TPixel2) == typeof(TPixel))
		{
			return Clone(configuration) as ImageFrame<TPixel2>;
		}
		ImageFrame<TPixel2> imageFrame = new ImageFrame<TPixel2>(configuration, base.Width, base.Height, base.Metadata.DeepClone());
		RowIntervalOperation<TPixel2> operation = new RowIntervalOperation<TPixel2>(PixelBuffer, imageFrame.PixelBuffer, configuration);
		ParallelRowIterator.IterateRowIntervals(configuration, Bounds(), in operation);
		return imageFrame;
	}

	internal void Clear(TPixel value)
	{
		MemoryGroup<TPixel> fastMemoryGroup = PixelBuffer.FastMemoryGroup;
		if (value.Equals(default(TPixel)))
		{
			fastMemoryGroup.Clear();
		}
		else
		{
			fastMemoryGroup.Fill(value);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void VerifyCoords(int x, int y)
	{
		if ((uint)x >= (uint)base.Width)
		{
			ThrowArgumentOutOfRangeException("x");
		}
		if ((uint)y >= (uint)base.Height)
		{
			ThrowArgumentOutOfRangeException("y");
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static void ThrowArgumentOutOfRangeException(string paramName)
	{
		throw new ArgumentOutOfRangeException(paramName);
	}
}
