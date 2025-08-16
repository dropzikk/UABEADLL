using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.IO;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.Metadata;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp;

public abstract class Image : IDisposable, IConfigurationProvider
{
	private class EncodeVisitor : IImageVisitor, IImageVisitorAsync
	{
		private readonly IImageEncoder encoder;

		private readonly Stream stream;

		public EncodeVisitor(IImageEncoder encoder, Stream stream)
		{
			this.encoder = encoder;
			this.stream = stream;
		}

		public void Visit<TPixel>(Image<TPixel> image) where TPixel : unmanaged, IPixel<TPixel>
		{
			encoder.Encode(image, stream);
		}

		public Task VisitAsync<TPixel>(Image<TPixel> image, CancellationToken cancellationToken) where TPixel : unmanaged, IPixel<TPixel>
		{
			return encoder.EncodeAsync(image, stream, cancellationToken);
		}
	}

	private bool isDisposed;

	public Configuration Configuration { get; }

	public PixelTypeInfo PixelType { get; }

	public int Width => Size.Width;

	public int Height => Size.Height;

	public ImageMetadata Metadata { get; }

	public Size Size { get; internal set; }

	public Rectangle Bounds => new Rectangle(0, 0, Width, Height);

	protected abstract ImageFrameCollection NonGenericFrameCollection { get; }

	public ImageFrameCollection Frames => NonGenericFrameCollection;

	protected Image(Configuration configuration, PixelTypeInfo pixelType, ImageMetadata metadata, Size size)
	{
		Configuration = configuration;
		PixelType = pixelType;
		Size = size;
		Metadata = metadata;
	}

	internal Image(Configuration configuration, PixelTypeInfo pixelType, ImageMetadata metadata, int width, int height)
		: this(configuration, pixelType, metadata, new Size(width, height))
	{
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

	public void Save(Stream stream, IImageEncoder encoder)
	{
		Guard.NotNull(stream, "stream");
		Guard.NotNull(encoder, "encoder");
		EnsureNotDisposed();
		this.AcceptVisitor(new EncodeVisitor(encoder, stream));
	}

	public Task SaveAsync(Stream stream, IImageEncoder encoder, CancellationToken cancellationToken = default(CancellationToken))
	{
		Guard.NotNull(stream, "stream");
		Guard.NotNull(encoder, "encoder");
		EnsureNotDisposed();
		return this.AcceptVisitorAsync(new EncodeVisitor(encoder, stream), cancellationToken);
	}

	public Image<TPixel2> CloneAs<TPixel2>() where TPixel2 : unmanaged, IPixel<TPixel2>
	{
		return CloneAs<TPixel2>(Configuration);
	}

	public abstract Image<TPixel2> CloneAs<TPixel2>(Configuration configuration) where TPixel2 : unmanaged, IPixel<TPixel2>;

	protected void UpdateSize(Size size)
	{
		Size = size;
	}

	protected abstract void Dispose(bool disposing);

	internal void EnsureNotDisposed()
	{
		if (isDisposed)
		{
			ThrowObjectDisposedException(GetType());
		}
	}

	internal abstract void Accept(IImageVisitor visitor);

	internal abstract Task AcceptAsync(IImageVisitorAsync visitor, CancellationToken cancellationToken);

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static void ThrowObjectDisposedException(Type type)
	{
		throw new ObjectDisposedException(type.Name);
	}

	internal static Image<TPixel> CreateUninitialized<TPixel>(Configuration configuration, int width, int height, ImageMetadata metadata) where TPixel : unmanaged, IPixel<TPixel>
	{
		Buffer2D<TPixel> buffer2D = configuration.MemoryAllocator.Allocate2D<TPixel>(width, height, configuration.PreferContiguousImageBuffers);
		return new Image<TPixel>(configuration, buffer2D.FastMemoryGroup, width, height, metadata);
	}

	private static IImageFormat InternalDetectFormat(Configuration configuration, Stream stream)
	{
		int num = (int)Math.Min(configuration.MaxHeaderSize, stream.Length);
		if (num <= 0)
		{
			ImageFormatManager.ThrowInvalidDecoder(configuration.ImageFormatsManager);
		}
		Span<byte> span = ((num <= 512) ? stackalloc byte[num] : ((Span<byte>)new byte[num]));
		Span<byte> span2 = span;
		long position = stream.Position;
		int num2 = 0;
		int num3;
		do
		{
			num3 = stream.Read(span2, num2, num - num2);
			num2 += num3;
		}
		while (num2 < num && num3 > 0);
		stream.Position = position;
		IImageFormat imageFormat = null;
		foreach (IImageFormatDetector formatDetector in configuration.ImageFormatsManager.FormatDetectors)
		{
			if (formatDetector.HeaderSize <= num && formatDetector.TryDetectFormat(span2, out IImageFormat format))
			{
				imageFormat = format;
			}
		}
		if (imageFormat == null)
		{
			ImageFormatManager.ThrowInvalidDecoder(configuration.ImageFormatsManager);
		}
		return imageFormat;
	}

	private static IImageDecoder DiscoverDecoder(DecoderOptions options, Stream stream)
	{
		IImageFormat format = InternalDetectFormat(options.Configuration, stream);
		return options.Configuration.ImageFormatsManager.GetDecoder(format);
	}

	private static Image<TPixel> Decode<TPixel>(DecoderOptions options, Stream stream) where TPixel : unmanaged, IPixel<TPixel>
	{
		return DiscoverDecoder(options, stream).Decode<TPixel>(options, stream);
	}

	private static Task<Image<TPixel>> DecodeAsync<TPixel>(DecoderOptions options, Stream stream, CancellationToken cancellationToken) where TPixel : unmanaged, IPixel<TPixel>
	{
		return DiscoverDecoder(options, stream).DecodeAsync<TPixel>(options, stream, cancellationToken);
	}

	private static Image Decode(DecoderOptions options, Stream stream)
	{
		return DiscoverDecoder(options, stream).Decode(options, stream);
	}

	private static Task<Image> DecodeAsync(DecoderOptions options, Stream stream, CancellationToken cancellationToken)
	{
		return DiscoverDecoder(options, stream).DecodeAsync(options, stream, cancellationToken);
	}

	private static ImageInfo InternalIdentify(DecoderOptions options, Stream stream)
	{
		return DiscoverDecoder(options, stream).Identify(options, stream);
	}

	private static Task<ImageInfo> InternalIdentifyAsync(DecoderOptions options, Stream stream, CancellationToken cancellationToken)
	{
		return DiscoverDecoder(options, stream).IdentifyAsync(options, stream, cancellationToken);
	}

	public static IImageFormat DetectFormat(ReadOnlySpan<byte> buffer)
	{
		return DetectFormat(DecoderOptions.Default, buffer);
	}

	public unsafe static IImageFormat DetectFormat(DecoderOptions options, ReadOnlySpan<byte> buffer)
	{
		Guard.NotNull(options, "Configuration");
		fixed (byte* pointer = buffer)
		{
			using UnmanagedMemoryStream stream = new UnmanagedMemoryStream(pointer, buffer.Length);
			return DetectFormat(options, stream);
		}
	}

	public static ImageInfo Identify(ReadOnlySpan<byte> buffer)
	{
		return Identify(DecoderOptions.Default, buffer);
	}

	public unsafe static ImageInfo Identify(DecoderOptions options, ReadOnlySpan<byte> buffer)
	{
		fixed (byte* pointer = buffer)
		{
			using UnmanagedMemoryStream stream = new UnmanagedMemoryStream(pointer, buffer.Length);
			return Identify(options, stream);
		}
	}

	public static Image Load(ReadOnlySpan<byte> buffer)
	{
		return Load(DecoderOptions.Default, buffer);
	}

	public unsafe static Image Load(DecoderOptions options, ReadOnlySpan<byte> buffer)
	{
		fixed (byte* pointer = buffer)
		{
			using UnmanagedMemoryStream stream = new UnmanagedMemoryStream(pointer, buffer.Length);
			return Load(options, stream);
		}
	}

	public static Image<TPixel> Load<TPixel>(ReadOnlySpan<byte> data) where TPixel : unmanaged, IPixel<TPixel>
	{
		return Load<TPixel>(DecoderOptions.Default, data);
	}

	public unsafe static Image<TPixel> Load<TPixel>(DecoderOptions options, ReadOnlySpan<byte> data) where TPixel : unmanaged, IPixel<TPixel>
	{
		fixed (byte* pointer = data)
		{
			using UnmanagedMemoryStream stream = new UnmanagedMemoryStream(pointer, data.Length);
			return Load<TPixel>(options, stream);
		}
	}

	public static IImageFormat DetectFormat(string path)
	{
		return DetectFormat(DecoderOptions.Default, path);
	}

	public static IImageFormat DetectFormat(DecoderOptions options, string path)
	{
		Guard.NotNull(options, "options");
		using Stream stream = options.Configuration.FileSystem.OpenRead(path);
		return DetectFormat(options, stream);
	}

	public static Task<IImageFormat> DetectFormatAsync(string path, CancellationToken cancellationToken = default(CancellationToken))
	{
		return DetectFormatAsync(DecoderOptions.Default, path, cancellationToken);
	}

	public static async Task<IImageFormat> DetectFormatAsync(DecoderOptions options, string path, CancellationToken cancellationToken = default(CancellationToken))
	{
		Guard.NotNull(options, "options");
		IImageFormat result;
		await using (Stream stream = options.Configuration.FileSystem.OpenReadAsynchronous(path))
		{
			result = await DetectFormatAsync(options, stream, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		}
		return result;
	}

	public static ImageInfo Identify(string path)
	{
		return Identify(DecoderOptions.Default, path);
	}

	public static ImageInfo Identify(DecoderOptions options, string path)
	{
		Guard.NotNull(options, "options");
		using Stream stream = options.Configuration.FileSystem.OpenRead(path);
		return Identify(options, stream);
	}

	public static Task<ImageInfo> IdentifyAsync(string path, CancellationToken cancellationToken = default(CancellationToken))
	{
		return IdentifyAsync(DecoderOptions.Default, path, cancellationToken);
	}

	public static async Task<ImageInfo> IdentifyAsync(DecoderOptions options, string path, CancellationToken cancellationToken = default(CancellationToken))
	{
		Guard.NotNull(options, "options");
		ImageInfo result;
		await using (Stream stream = options.Configuration.FileSystem.OpenReadAsynchronous(path))
		{
			result = await IdentifyAsync(options, stream, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		}
		return result;
	}

	public static Image Load(string path)
	{
		return Load(DecoderOptions.Default, path);
	}

	public static Image Load(DecoderOptions options, string path)
	{
		Guard.NotNull(options, "options");
		Guard.NotNull(path, "path");
		using Stream stream = options.Configuration.FileSystem.OpenRead(path);
		return Load(options, stream);
	}

	public static Task<Image> LoadAsync(string path, CancellationToken cancellationToken = default(CancellationToken))
	{
		return LoadAsync(DecoderOptions.Default, path, cancellationToken);
	}

	public static async Task<Image> LoadAsync(DecoderOptions options, string path, CancellationToken cancellationToken = default(CancellationToken))
	{
		Image result;
		await using (Stream stream = options.Configuration.FileSystem.OpenReadAsynchronous(path))
		{
			result = await LoadAsync(options, stream, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		}
		return result;
	}

	public static Image<TPixel> Load<TPixel>(string path) where TPixel : unmanaged, IPixel<TPixel>
	{
		return Load<TPixel>(DecoderOptions.Default, path);
	}

	public static Image<TPixel> Load<TPixel>(DecoderOptions options, string path) where TPixel : unmanaged, IPixel<TPixel>
	{
		Guard.NotNull(options, "options");
		Guard.NotNull(path, "path");
		using Stream stream = options.Configuration.FileSystem.OpenRead(path);
		return Load<TPixel>(options, stream);
	}

	public static Task<Image<TPixel>> LoadAsync<TPixel>(string path, CancellationToken cancellationToken = default(CancellationToken)) where TPixel : unmanaged, IPixel<TPixel>
	{
		return LoadAsync<TPixel>(DecoderOptions.Default, path, cancellationToken);
	}

	public static async Task<Image<TPixel>> LoadAsync<TPixel>(DecoderOptions options, string path, CancellationToken cancellationToken = default(CancellationToken)) where TPixel : unmanaged, IPixel<TPixel>
	{
		Guard.NotNull(options, "options");
		Guard.NotNull(path, "path");
		Image<TPixel> result;
		await using (Stream stream = options.Configuration.FileSystem.OpenReadAsynchronous(path))
		{
			result = await LoadAsync<TPixel>(options, stream, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		}
		return result;
	}

	public static IImageFormat DetectFormat(Stream stream)
	{
		return DetectFormat(DecoderOptions.Default, stream);
	}

	public static IImageFormat DetectFormat(DecoderOptions options, Stream stream)
	{
		return WithSeekableStream(options, stream, (Stream s) => InternalDetectFormat(options.Configuration, s));
	}

	public static Task<IImageFormat> DetectFormatAsync(Stream stream, CancellationToken cancellationToken = default(CancellationToken))
	{
		return DetectFormatAsync(DecoderOptions.Default, stream, cancellationToken);
	}

	public static Task<IImageFormat> DetectFormatAsync(DecoderOptions options, Stream stream, CancellationToken cancellationToken = default(CancellationToken))
	{
		return WithSeekableStreamAsync(options, stream, (Stream s, CancellationToken _) => Task.FromResult(InternalDetectFormat(options.Configuration, s)), cancellationToken);
	}

	public static ImageInfo Identify(Stream stream)
	{
		return Identify(DecoderOptions.Default, stream);
	}

	public static ImageInfo Identify(DecoderOptions options, Stream stream)
	{
		return WithSeekableStream(options, stream, (Stream s) => InternalIdentify(options, s));
	}

	public static Task<ImageInfo> IdentifyAsync(Stream stream, CancellationToken cancellationToken = default(CancellationToken))
	{
		return IdentifyAsync(DecoderOptions.Default, stream, cancellationToken);
	}

	public static Task<ImageInfo> IdentifyAsync(DecoderOptions options, Stream stream, CancellationToken cancellationToken = default(CancellationToken))
	{
		return WithSeekableStreamAsync(options, stream, (Stream s, CancellationToken ct) => InternalIdentifyAsync(options, s, ct), cancellationToken);
	}

	public static Image Load(Stream stream)
	{
		return Load(DecoderOptions.Default, stream);
	}

	public static Image Load(DecoderOptions options, Stream stream)
	{
		return WithSeekableStream(options, stream, (Stream s) => Decode(options, s));
	}

	public static Task<Image> LoadAsync(Stream stream, CancellationToken cancellationToken = default(CancellationToken))
	{
		return LoadAsync(DecoderOptions.Default, stream, cancellationToken);
	}

	public static Task<Image> LoadAsync(DecoderOptions options, Stream stream, CancellationToken cancellationToken = default(CancellationToken))
	{
		return WithSeekableStreamAsync(options, stream, (Stream s, CancellationToken ct) => DecodeAsync(options, s, ct), cancellationToken);
	}

	public static Image<TPixel> Load<TPixel>(Stream stream) where TPixel : unmanaged, IPixel<TPixel>
	{
		return Load<TPixel>(DecoderOptions.Default, stream);
	}

	public static Image<TPixel> Load<TPixel>(DecoderOptions options, Stream stream) where TPixel : unmanaged, IPixel<TPixel>
	{
		return WithSeekableStream(options, stream, (Stream s) => Decode<TPixel>(options, s));
	}

	public static Task<Image<TPixel>> LoadAsync<TPixel>(Stream stream, CancellationToken cancellationToken = default(CancellationToken)) where TPixel : unmanaged, IPixel<TPixel>
	{
		return LoadAsync<TPixel>(DecoderOptions.Default, stream, cancellationToken);
	}

	public static Task<Image<TPixel>> LoadAsync<TPixel>(DecoderOptions options, Stream stream, CancellationToken cancellationToken = default(CancellationToken)) where TPixel : unmanaged, IPixel<TPixel>
	{
		return WithSeekableStreamAsync(options, stream, (Stream s, CancellationToken ct) => DecodeAsync<TPixel>(options, s, ct), cancellationToken);
	}

	internal static T WithSeekableStream<T>(DecoderOptions options, Stream stream, Func<Stream, T> action)
	{
		Guard.NotNull(options, "options");
		Guard.NotNull(stream, "stream");
		if (!stream.CanRead)
		{
			throw new NotSupportedException("Cannot read from the stream.");
		}
		Configuration configuration = options.Configuration;
		if (stream.CanSeek)
		{
			if (configuration.ReadOrigin == ReadOrigin.Begin)
			{
				stream.Position = 0L;
			}
			return action(stream);
		}
		using ChunkedMemoryStream chunkedMemoryStream = new ChunkedMemoryStream(configuration.MemoryAllocator);
		stream.CopyTo(chunkedMemoryStream, configuration.StreamProcessingBufferSize);
		chunkedMemoryStream.Position = 0L;
		return action(chunkedMemoryStream);
	}

	internal static async Task<T> WithSeekableStreamAsync<T>(DecoderOptions options, Stream stream, Func<Stream, CancellationToken, Task<T>> action, CancellationToken cancellationToken)
	{
		Guard.NotNull(options, "options");
		Guard.NotNull(stream, "stream");
		if (!stream.CanRead)
		{
			throw new NotSupportedException("Cannot read from the stream.");
		}
		Configuration configuration = options.Configuration;
		if (stream.CanSeek)
		{
			if (configuration.ReadOrigin == ReadOrigin.Begin)
			{
				stream.Position = 0L;
			}
			return await action(stream, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		}
		using ChunkedMemoryStream memoryStream = new ChunkedMemoryStream(configuration.MemoryAllocator);
		await stream.CopyToAsync(memoryStream, configuration.StreamProcessingBufferSize, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		memoryStream.Position = 0L;
		return await action(memoryStream, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
	}

	public static Image<TPixel> LoadPixelData<TPixel>(ReadOnlySpan<TPixel> data, int width, int height) where TPixel : unmanaged, IPixel<TPixel>
	{
		return LoadPixelData(SixLabors.ImageSharp.Configuration.Default, data, width, height);
	}

	public static Image<TPixel> LoadPixelData<TPixel>(ReadOnlySpan<byte> data, int width, int height) where TPixel : unmanaged, IPixel<TPixel>
	{
		return LoadPixelData<TPixel>(SixLabors.ImageSharp.Configuration.Default, data, width, height);
	}

	public static Image<TPixel> LoadPixelData<TPixel>(Configuration configuration, ReadOnlySpan<byte> data, int width, int height) where TPixel : unmanaged, IPixel<TPixel>
	{
		return LoadPixelData(configuration, MemoryMarshal.Cast<byte, TPixel>(data), width, height);
	}

	public static Image<TPixel> LoadPixelData<TPixel>(Configuration configuration, ReadOnlySpan<TPixel> data, int width, int height) where TPixel : unmanaged, IPixel<TPixel>
	{
		Guard.NotNull(configuration, "configuration");
		int num = width * height;
		Guard.MustBeGreaterThanOrEqualTo(data.Length, num, "data");
		Image<TPixel> image = new Image<TPixel>(configuration, width, height);
		data = data.Slice(0, num);
		data.CopyTo(image.Frames.RootFrame.PixelBuffer.FastMemoryGroup);
		return image;
	}

	public static Image<TPixel> WrapMemory<TPixel>(Configuration configuration, Memory<TPixel> pixelMemory, int width, int height, ImageMetadata metadata) where TPixel : unmanaged, IPixel<TPixel>
	{
		Guard.NotNull(configuration, "configuration");
		Guard.NotNull(metadata, "metadata");
		Guard.IsTrue(pixelMemory.Length >= width * height, "pixelMemory", "The length of the input memory is less than the specified image size");
		MemoryGroup<TPixel> memoryGroup = MemoryGroup<TPixel>.Wrap(pixelMemory);
		return new Image<TPixel>(configuration, memoryGroup, width, height, metadata);
	}

	public static Image<TPixel> WrapMemory<TPixel>(Configuration configuration, Memory<TPixel> pixelMemory, int width, int height) where TPixel : unmanaged, IPixel<TPixel>
	{
		return WrapMemory(configuration, pixelMemory, width, height, new ImageMetadata());
	}

	public static Image<TPixel> WrapMemory<TPixel>(Memory<TPixel> pixelMemory, int width, int height) where TPixel : unmanaged, IPixel<TPixel>
	{
		return WrapMemory(SixLabors.ImageSharp.Configuration.Default, pixelMemory, width, height);
	}

	public static Image<TPixel> WrapMemory<TPixel>(Configuration configuration, IMemoryOwner<TPixel> pixelMemoryOwner, int width, int height, ImageMetadata metadata) where TPixel : unmanaged, IPixel<TPixel>
	{
		Guard.NotNull(configuration, "configuration");
		Guard.NotNull(metadata, "metadata");
		Guard.IsTrue(pixelMemoryOwner.Memory.Length >= width * height, "pixelMemoryOwner", "The length of the input memory is less than the specified image size");
		MemoryGroup<TPixel> memoryGroup = MemoryGroup<TPixel>.Wrap(pixelMemoryOwner);
		return new Image<TPixel>(configuration, memoryGroup, width, height, metadata);
	}

	public static Image<TPixel> WrapMemory<TPixel>(Configuration configuration, IMemoryOwner<TPixel> pixelMemoryOwner, int width, int height) where TPixel : unmanaged, IPixel<TPixel>
	{
		return WrapMemory(configuration, pixelMemoryOwner, width, height, new ImageMetadata());
	}

	public static Image<TPixel> WrapMemory<TPixel>(IMemoryOwner<TPixel> pixelMemoryOwner, int width, int height) where TPixel : unmanaged, IPixel<TPixel>
	{
		return WrapMemory(SixLabors.ImageSharp.Configuration.Default, pixelMemoryOwner, width, height);
	}

	public static Image<TPixel> WrapMemory<TPixel>(Configuration configuration, Memory<byte> byteMemory, int width, int height, ImageMetadata metadata) where TPixel : unmanaged, IPixel<TPixel>
	{
		Guard.NotNull(configuration, "configuration");
		Guard.NotNull(metadata, "metadata");
		ByteMemoryManager<TPixel> byteMemoryManager = new ByteMemoryManager<TPixel>(byteMemory);
		Guard.IsTrue(byteMemoryManager.Memory.Length >= width * height, "byteMemory", "The length of the input memory is less than the specified image size");
		MemoryGroup<TPixel> memoryGroup = MemoryGroup<TPixel>.Wrap(byteMemoryManager.Memory);
		return new Image<TPixel>(configuration, memoryGroup, width, height, metadata);
	}

	public static Image<TPixel> WrapMemory<TPixel>(Configuration configuration, Memory<byte> byteMemory, int width, int height) where TPixel : unmanaged, IPixel<TPixel>
	{
		return WrapMemory<TPixel>(configuration, byteMemory, width, height, new ImageMetadata());
	}

	public static Image<TPixel> WrapMemory<TPixel>(Memory<byte> byteMemory, int width, int height) where TPixel : unmanaged, IPixel<TPixel>
	{
		return WrapMemory<TPixel>(SixLabors.ImageSharp.Configuration.Default, byteMemory, width, height);
	}

	public static Image<TPixel> WrapMemory<TPixel>(Configuration configuration, IMemoryOwner<byte> byteMemoryOwner, int width, int height, ImageMetadata metadata) where TPixel : unmanaged, IPixel<TPixel>
	{
		Guard.NotNull(configuration, "configuration");
		Guard.NotNull(metadata, "metadata");
		ByteMemoryOwner<TPixel> byteMemoryOwner2 = new ByteMemoryOwner<TPixel>(byteMemoryOwner);
		Guard.IsTrue(byteMemoryOwner2.Memory.Length >= (long)width * (long)height, "pixelMemoryOwner", "The length of the input memory is less than the specified image size");
		MemoryGroup<TPixel> memoryGroup = MemoryGroup<TPixel>.Wrap(byteMemoryOwner2);
		return new Image<TPixel>(configuration, memoryGroup, width, height, metadata);
	}

	public static Image<TPixel> WrapMemory<TPixel>(Configuration configuration, IMemoryOwner<byte> byteMemoryOwner, int width, int height) where TPixel : unmanaged, IPixel<TPixel>
	{
		return WrapMemory<TPixel>(configuration, byteMemoryOwner, width, height, new ImageMetadata());
	}

	public static Image<TPixel> WrapMemory<TPixel>(IMemoryOwner<byte> byteMemoryOwner, int width, int height) where TPixel : unmanaged, IPixel<TPixel>
	{
		return WrapMemory<TPixel>(SixLabors.ImageSharp.Configuration.Default, byteMemoryOwner, width, height);
	}

	public unsafe static Image<TPixel> WrapMemory<TPixel>(Configuration configuration, void* pointer, int bufferSizeInBytes, int width, int height, ImageMetadata metadata) where TPixel : unmanaged, IPixel<TPixel>
	{
		Guard.IsFalse(pointer == null, "pointer", "Pointer must be not null");
		Guard.NotNull(configuration, "configuration");
		Guard.NotNull(metadata, "metadata");
		UnmanagedMemoryManager<TPixel> unmanagedMemoryManager = new UnmanagedMemoryManager<TPixel>(pointer, width * height);
		Guard.MustBeGreaterThanOrEqualTo(bufferSizeInBytes, unmanagedMemoryManager.Memory.Span.Length, "bufferSizeInBytes");
		MemoryGroup<TPixel> memoryGroup = MemoryGroup<TPixel>.Wrap(unmanagedMemoryManager.Memory);
		return new Image<TPixel>(configuration, memoryGroup, width, height, metadata);
	}

	public unsafe static Image<TPixel> WrapMemory<TPixel>(Configuration configuration, void* pointer, int bufferSizeInBytes, int width, int height) where TPixel : unmanaged, IPixel<TPixel>
	{
		return WrapMemory<TPixel>(configuration, pointer, bufferSizeInBytes, width, height, new ImageMetadata());
	}

	public unsafe static Image<TPixel> WrapMemory<TPixel>(void* pointer, int bufferSizeInBytes, int width, int height) where TPixel : unmanaged, IPixel<TPixel>
	{
		return WrapMemory<TPixel>(SixLabors.ImageSharp.Configuration.Default, pointer, bufferSizeInBytes, width, height);
	}
}
public sealed class Image<TPixel> : Image where TPixel : unmanaged, IPixel<TPixel>
{
	private readonly ImageFrameCollection<TPixel> frames;

	protected override ImageFrameCollection NonGenericFrameCollection => Frames;

	public new ImageFrameCollection<TPixel> Frames
	{
		get
		{
			EnsureNotDisposed();
			return frames;
		}
	}

	private IPixelSource<TPixel> PixelSourceUnsafe => frames.RootFrameUnsafe;

	public TPixel this[int x, int y]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			EnsureNotDisposed();
			VerifyCoords(x, y);
			return PixelSourceUnsafe.PixelBuffer.GetElementUnsafe(x, y);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			EnsureNotDisposed();
			VerifyCoords(x, y);
			PixelSourceUnsafe.PixelBuffer.GetElementUnsafe(x, y) = value;
		}
	}

	public Image(Configuration configuration, int width, int height)
		: this(configuration, width, height, new ImageMetadata())
	{
	}

	public Image(Configuration configuration, int width, int height, TPixel backgroundColor)
		: this(configuration, width, height, backgroundColor, new ImageMetadata())
	{
	}

	public Image(int width, int height, TPixel backgroundColor)
		: this(SixLabors.ImageSharp.Configuration.Default, width, height, backgroundColor, new ImageMetadata())
	{
	}

	public Image(int width, int height)
		: this(SixLabors.ImageSharp.Configuration.Default, width, height)
	{
	}

	internal Image(Configuration configuration, int width, int height, ImageMetadata? metadata)
		: base(configuration, PixelTypeInfo.Create<TPixel>(), metadata ?? new ImageMetadata(), width, height)
	{
		frames = new ImageFrameCollection<TPixel>(this, width, height, default(TPixel));
	}

	internal Image(Configuration configuration, Buffer2D<TPixel> pixelBuffer, ImageMetadata metadata)
		: this(configuration, pixelBuffer.FastMemoryGroup, pixelBuffer.Width, pixelBuffer.Height, metadata)
	{
	}

	internal Image(Configuration configuration, MemoryGroup<TPixel> memoryGroup, int width, int height, ImageMetadata metadata)
		: base(configuration, PixelTypeInfo.Create<TPixel>(), metadata, width, height)
	{
		frames = new ImageFrameCollection<TPixel>(this, width, height, memoryGroup);
	}

	internal Image(Configuration configuration, int width, int height, TPixel backgroundColor, ImageMetadata? metadata)
		: base(configuration, PixelTypeInfo.Create<TPixel>(), metadata ?? new ImageMetadata(), width, height)
	{
		frames = new ImageFrameCollection<TPixel>(this, width, height, backgroundColor);
	}

	internal Image(Configuration configuration, ImageMetadata metadata, IEnumerable<ImageFrame<TPixel>> frames)
		: base(configuration, PixelTypeInfo.Create<TPixel>(), metadata, ValidateFramesAndGetSize(frames))
	{
		this.frames = new ImageFrameCollection<TPixel>(this, frames);
	}

	public void ProcessPixelRows(PixelAccessorAction<TPixel> processPixels)
	{
		Guard.NotNull(processPixels, "processPixels");
		Buffer2D<TPixel> pixelBuffer = Frames.RootFrame.PixelBuffer;
		pixelBuffer.FastMemoryGroup.IncreaseRefCounts();
		try
		{
			PixelAccessor<TPixel> pixelAccessor = new PixelAccessor<TPixel>(pixelBuffer);
			processPixels(pixelAccessor);
		}
		finally
		{
			pixelBuffer.FastMemoryGroup.DecreaseRefCounts();
		}
	}

	public void ProcessPixelRows<TPixel2>(Image<TPixel2> image2, PixelAccessorAction<TPixel, TPixel2> processPixels) where TPixel2 : unmanaged, IPixel<TPixel2>
	{
		Guard.NotNull(image2, "image2");
		Guard.NotNull(processPixels, "processPixels");
		Buffer2D<TPixel> pixelBuffer = Frames.RootFrame.PixelBuffer;
		Buffer2D<TPixel2> pixelBuffer2 = image2.Frames.RootFrame.PixelBuffer;
		pixelBuffer.FastMemoryGroup.IncreaseRefCounts();
		pixelBuffer2.FastMemoryGroup.IncreaseRefCounts();
		try
		{
			PixelAccessor<TPixel> pixelAccessor = new PixelAccessor<TPixel>(pixelBuffer);
			PixelAccessor<TPixel2> pixelAccessor2 = new PixelAccessor<TPixel2>(pixelBuffer2);
			processPixels(pixelAccessor, pixelAccessor2);
		}
		finally
		{
			pixelBuffer2.FastMemoryGroup.DecreaseRefCounts();
			pixelBuffer.FastMemoryGroup.DecreaseRefCounts();
		}
	}

	public void ProcessPixelRows<TPixel2, TPixel3>(Image<TPixel2> image2, Image<TPixel3> image3, PixelAccessorAction<TPixel, TPixel2, TPixel3> processPixels) where TPixel2 : unmanaged, IPixel<TPixel2> where TPixel3 : unmanaged, IPixel<TPixel3>
	{
		Guard.NotNull(image2, "image2");
		Guard.NotNull(image3, "image3");
		Guard.NotNull(processPixels, "processPixels");
		Buffer2D<TPixel> pixelBuffer = Frames.RootFrame.PixelBuffer;
		Buffer2D<TPixel2> pixelBuffer2 = image2.Frames.RootFrame.PixelBuffer;
		Buffer2D<TPixel3> pixelBuffer3 = image3.Frames.RootFrame.PixelBuffer;
		pixelBuffer.FastMemoryGroup.IncreaseRefCounts();
		pixelBuffer2.FastMemoryGroup.IncreaseRefCounts();
		pixelBuffer3.FastMemoryGroup.IncreaseRefCounts();
		try
		{
			PixelAccessor<TPixel> pixelAccessor = new PixelAccessor<TPixel>(pixelBuffer);
			PixelAccessor<TPixel2> pixelAccessor2 = new PixelAccessor<TPixel2>(pixelBuffer2);
			PixelAccessor<TPixel3> pixelAccessor3 = new PixelAccessor<TPixel3>(pixelBuffer3);
			processPixels(pixelAccessor, pixelAccessor2, pixelAccessor3);
		}
		finally
		{
			pixelBuffer3.FastMemoryGroup.DecreaseRefCounts();
			pixelBuffer2.FastMemoryGroup.DecreaseRefCounts();
			pixelBuffer.FastMemoryGroup.DecreaseRefCounts();
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

	public Image<TPixel> Clone()
	{
		return Clone(base.Configuration);
	}

	public Image<TPixel> Clone(Configuration configuration)
	{
		EnsureNotDisposed();
		ImageFrame<TPixel>[] array = new ImageFrame<TPixel>[frames.Count];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = frames[i].Clone(configuration);
		}
		return new Image<TPixel>(configuration, base.Metadata.DeepClone(), array);
	}

	public override Image<TPixel2> CloneAs<TPixel2>(Configuration configuration)
	{
		EnsureNotDisposed();
		ImageFrame<TPixel2>[] array = new ImageFrame<TPixel2>[frames.Count];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = frames[i].CloneAs<TPixel2>(configuration);
		}
		return new Image<TPixel2>(configuration, base.Metadata.DeepClone(), array);
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			frames.Dispose();
		}
	}

	public override string ToString()
	{
		return $"Image<{typeof(TPixel).Name}>: {base.Width}x{base.Height}";
	}

	internal override void Accept(IImageVisitor visitor)
	{
		EnsureNotDisposed();
		visitor.Visit(this);
	}

	internal override Task AcceptAsync(IImageVisitorAsync visitor, CancellationToken cancellationToken)
	{
		EnsureNotDisposed();
		return visitor.VisitAsync(this, cancellationToken);
	}

	internal void SwapOrCopyPixelsBuffersFrom(Image<TPixel> pixelSource)
	{
		Guard.NotNull(pixelSource, "pixelSource");
		EnsureNotDisposed();
		ImageFrameCollection<TPixel> imageFrameCollection = pixelSource.Frames;
		for (int i = 0; i < frames.Count; i++)
		{
			frames[i].SwapOrCopyPixelsBufferFrom(imageFrameCollection[i]);
		}
		UpdateSize(pixelSource.Size);
	}

	private static Size ValidateFramesAndGetSize(IEnumerable<ImageFrame<TPixel>> frames)
	{
		Guard.NotNull(frames, "frames");
		ImageFrame<TPixel> imageFrame = frames.FirstOrDefault() ?? throw new ArgumentException("Must not be empty.", "frames");
		Size rootSize = imageFrame.Size();
		if (frames.Any((ImageFrame<TPixel> f) => f.Size() != rootSize))
		{
			throw new ArgumentException("The provided frames must be of the same size.", "frames");
		}
		return rootSize;
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

	private static void ThrowArgumentOutOfRangeException(string paramName)
	{
		throw new ArgumentOutOfRangeException(paramName);
	}
}
