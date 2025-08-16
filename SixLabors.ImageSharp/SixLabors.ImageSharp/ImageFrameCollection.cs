using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp;

public abstract class ImageFrameCollection : IDisposable, IEnumerable<ImageFrame>, IEnumerable
{
	private bool isDisposed;

	public abstract int Count { get; }

	public ImageFrame RootFrame
	{
		get
		{
			EnsureNotDisposed();
			return NonGenericRootFrame;
		}
	}

	protected abstract ImageFrame NonGenericRootFrame { get; }

	public ImageFrame this[int index]
	{
		get
		{
			EnsureNotDisposed();
			return NonGenericGetFrame(index);
		}
	}

	public abstract int IndexOf(ImageFrame frame);

	public ImageFrame InsertFrame(int index, ImageFrame source)
	{
		EnsureNotDisposed();
		return NonGenericInsertFrame(index, source);
	}

	public ImageFrame AddFrame(ImageFrame source)
	{
		EnsureNotDisposed();
		return NonGenericAddFrame(source);
	}

	public abstract void RemoveFrame(int index);

	public abstract bool Contains(ImageFrame frame);

	public abstract void MoveFrame(int sourceIndex, int destinationIndex);

	public Image ExportFrame(int index)
	{
		EnsureNotDisposed();
		return NonGenericExportFrame(index);
	}

	public Image CloneFrame(int index)
	{
		EnsureNotDisposed();
		return NonGenericCloneFrame(index);
	}

	public ImageFrame CreateFrame()
	{
		EnsureNotDisposed();
		return NonGenericCreateFrame();
	}

	public ImageFrame CreateFrame(Color backgroundColor)
	{
		EnsureNotDisposed();
		return NonGenericCreateFrame(backgroundColor);
	}

	public void Dispose()
	{
		if (!isDisposed)
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
			isDisposed = true;
		}
	}

	IEnumerator<ImageFrame> IEnumerable<ImageFrame>.GetEnumerator()
	{
		EnsureNotDisposed();
		return NonGenericGetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return ((IEnumerable<ImageFrame>)this).GetEnumerator();
	}

	protected void EnsureNotDisposed()
	{
		if (isDisposed)
		{
			ThrowObjectDisposedException(GetType());
		}
	}

	protected abstract void Dispose(bool disposing);

	protected abstract IEnumerator<ImageFrame> NonGenericGetEnumerator();

	protected abstract ImageFrame NonGenericGetFrame(int index);

	protected abstract ImageFrame NonGenericInsertFrame(int index, ImageFrame source);

	protected abstract ImageFrame NonGenericAddFrame(ImageFrame source);

	protected abstract Image NonGenericExportFrame(int index);

	protected abstract Image NonGenericCloneFrame(int index);

	protected abstract ImageFrame NonGenericCreateFrame();

	protected abstract ImageFrame NonGenericCreateFrame(Color backgroundColor);

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static void ThrowObjectDisposedException(Type type)
	{
		throw new ObjectDisposedException(type.Name);
	}
}
public sealed class ImageFrameCollection<TPixel> : ImageFrameCollection, IEnumerable<ImageFrame<TPixel>>, IEnumerable where TPixel : unmanaged, IPixel<TPixel>
{
	private readonly IList<ImageFrame<TPixel>> frames = new List<ImageFrame<TPixel>>();

	private readonly Image<TPixel> parent;

	public override int Count => frames.Count;

	public new ImageFrame<TPixel> RootFrame
	{
		get
		{
			EnsureNotDisposed();
			return frames[0];
		}
	}

	internal ImageFrame<TPixel> RootFrameUnsafe => frames[0];

	protected override ImageFrame NonGenericRootFrame => RootFrame;

	public new ImageFrame<TPixel> this[int index]
	{
		get
		{
			EnsureNotDisposed();
			return frames[index];
		}
	}

	internal ImageFrameCollection(Image<TPixel> parent, int width, int height, TPixel backgroundColor)
	{
		this.parent = parent ?? throw new ArgumentNullException("parent");
		frames.Add(new ImageFrame<TPixel>(parent.Configuration, width, height, backgroundColor));
	}

	internal ImageFrameCollection(Image<TPixel> parent, int width, int height, MemoryGroup<TPixel> memorySource)
	{
		this.parent = parent ?? throw new ArgumentNullException("parent");
		frames.Add(new ImageFrame<TPixel>(parent.Configuration, width, height, memorySource));
	}

	internal ImageFrameCollection(Image<TPixel> parent, IEnumerable<ImageFrame<TPixel>> frames)
	{
		Guard.NotNull(parent, "parent");
		Guard.NotNull(frames, "frames");
		this.parent = parent;
		foreach (ImageFrame<TPixel> frame in frames)
		{
			ValidateFrame(frame);
			this.frames.Add(frame);
		}
		if (this.frames.Count == 0)
		{
			throw new ArgumentException("Must not be empty.", "frames");
		}
	}

	public override int IndexOf(ImageFrame frame)
	{
		EnsureNotDisposed();
		if (!(frame is ImageFrame<TPixel> item))
		{
			return -1;
		}
		return frames.IndexOf(item);
	}

	public int IndexOf(ImageFrame<TPixel> frame)
	{
		EnsureNotDisposed();
		return frames.IndexOf(frame);
	}

	public ImageFrame<TPixel> InsertFrame(int index, ImageFrame<TPixel> source)
	{
		EnsureNotDisposed();
		ValidateFrame(source);
		ImageFrame<TPixel> imageFrame = source.Clone(parent.Configuration);
		frames.Insert(index, imageFrame);
		return imageFrame;
	}

	public ImageFrame<TPixel> AddFrame(ImageFrame<TPixel> source)
	{
		EnsureNotDisposed();
		ValidateFrame(source);
		ImageFrame<TPixel> imageFrame = source.Clone(parent.Configuration);
		frames.Add(imageFrame);
		return imageFrame;
	}

	public ImageFrame<TPixel> AddFrame(ReadOnlySpan<TPixel> source)
	{
		EnsureNotDisposed();
		ImageFrame<TPixel> imageFrame = ImageFrame.LoadPixelData(parent.Configuration, source, RootFrame.Width, RootFrame.Height);
		frames.Add(imageFrame);
		return imageFrame;
	}

	public ImageFrame<TPixel> AddFrame(TPixel[] source)
	{
		Guard.NotNull(source, "source");
		return AddFrame(source.AsSpan());
	}

	public override void RemoveFrame(int index)
	{
		EnsureNotDisposed();
		if (index == 0 && Count == 1)
		{
			throw new InvalidOperationException("Cannot remove last frame.");
		}
		ImageFrame<TPixel> imageFrame = frames[index];
		frames.RemoveAt(index);
		imageFrame.Dispose();
	}

	public override bool Contains(ImageFrame frame)
	{
		EnsureNotDisposed();
		if (frame is ImageFrame<TPixel> item)
		{
			return frames.Contains(item);
		}
		return false;
	}

	public bool Contains(ImageFrame<TPixel> frame)
	{
		EnsureNotDisposed();
		return frames.Contains(frame);
	}

	public override void MoveFrame(int sourceIndex, int destinationIndex)
	{
		EnsureNotDisposed();
		if (sourceIndex != destinationIndex)
		{
			ImageFrame<TPixel> item = frames[sourceIndex];
			frames.RemoveAt(sourceIndex);
			frames.Insert(destinationIndex, item);
		}
	}

	public new Image<TPixel> ExportFrame(int index)
	{
		EnsureNotDisposed();
		ImageFrame<TPixel> imageFrame = this[index];
		if (Count == 1 && frames.Contains(imageFrame))
		{
			throw new InvalidOperationException("Cannot remove last frame.");
		}
		frames.Remove(imageFrame);
		return new Image<TPixel>(parent.Configuration, parent.Metadata.DeepClone(), new ImageFrame<TPixel>[1] { imageFrame });
	}

	public new Image<TPixel> CloneFrame(int index)
	{
		EnsureNotDisposed();
		ImageFrame<TPixel> imageFrame = this[index].Clone();
		return new Image<TPixel>(parent.Configuration, parent.Metadata.DeepClone(), new ImageFrame<TPixel>[1] { imageFrame });
	}

	public new ImageFrame<TPixel> CreateFrame()
	{
		EnsureNotDisposed();
		ImageFrame<TPixel> imageFrame = new ImageFrame<TPixel>(parent.Configuration, RootFrame.Width, RootFrame.Height);
		frames.Add(imageFrame);
		return imageFrame;
	}

	protected override IEnumerator<ImageFrame> NonGenericGetEnumerator()
	{
		return frames.GetEnumerator();
	}

	protected override ImageFrame NonGenericGetFrame(int index)
	{
		return this[index];
	}

	protected override ImageFrame NonGenericInsertFrame(int index, ImageFrame source)
	{
		Guard.NotNull(source, "source");
		if (source is ImageFrame<TPixel> source2)
		{
			return InsertFrame(index, source2);
		}
		ImageFrame<TPixel> imageFrame = CopyNonCompatibleFrame(source);
		frames.Insert(index, imageFrame);
		return imageFrame;
	}

	protected override ImageFrame NonGenericAddFrame(ImageFrame source)
	{
		Guard.NotNull(source, "source");
		if (source is ImageFrame<TPixel> source2)
		{
			return AddFrame(source2);
		}
		ImageFrame<TPixel> imageFrame = CopyNonCompatibleFrame(source);
		frames.Add(imageFrame);
		return imageFrame;
	}

	protected override Image NonGenericExportFrame(int index)
	{
		return ExportFrame(index);
	}

	protected override Image NonGenericCloneFrame(int index)
	{
		return CloneFrame(index);
	}

	protected override ImageFrame NonGenericCreateFrame(Color backgroundColor)
	{
		return CreateFrame(backgroundColor.ToPixel<TPixel>());
	}

	protected override ImageFrame NonGenericCreateFrame()
	{
		return CreateFrame();
	}

	public ImageFrame<TPixel> CreateFrame(TPixel backgroundColor)
	{
		ImageFrame<TPixel> imageFrame = new ImageFrame<TPixel>(parent.Configuration, RootFrame.Width, RootFrame.Height, backgroundColor);
		frames.Add(imageFrame);
		return imageFrame;
	}

	public IEnumerator<ImageFrame<TPixel>> GetEnumerator()
	{
		EnsureNotDisposed();
		return frames.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	private void ValidateFrame(ImageFrame<TPixel> frame)
	{
		Guard.NotNull(frame, "frame");
		if (Count != 0 && (RootFrame.Width != frame.Width || RootFrame.Height != frame.Height))
		{
			throw new ArgumentException("Frame must have the same dimensions as the image.", "frame");
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (!disposing)
		{
			return;
		}
		foreach (ImageFrame<TPixel> frame in frames)
		{
			frame.Dispose();
		}
		frames.Clear();
	}

	private ImageFrame<TPixel> CopyNonCompatibleFrame(ImageFrame source)
	{
		ImageFrame<TPixel> imageFrame = new ImageFrame<TPixel>(parent.Configuration, source.Size(), source.Metadata.DeepClone());
		source.CopyPixelsTo(imageFrame.PixelBuffer.FastMemoryGroup);
		return imageFrame;
	}
}
