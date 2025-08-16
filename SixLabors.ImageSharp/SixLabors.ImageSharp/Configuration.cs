using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Pbm;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Qoi;
using SixLabors.ImageSharp.Formats.Tga;
using SixLabors.ImageSharp.Formats.Tiff;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.IO;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.Processing;

namespace SixLabors.ImageSharp;

public sealed class Configuration
{
	private static readonly Lazy<Configuration> Lazy = new Lazy<Configuration>(CreateDefaultInstance);

	private const int DefaultStreamProcessingBufferSize = 8096;

	private int streamProcessingBufferSize = 8096;

	private int maxDegreeOfParallelism = Environment.ProcessorCount;

	private MemoryAllocator memoryAllocator = SixLabors.ImageSharp.Memory.MemoryAllocator.Default;

	public static Configuration Default { get; } = Lazy.Value;

	public int MaxDegreeOfParallelism
	{
		get
		{
			return maxDegreeOfParallelism;
		}
		set
		{
			if ((value < -1 || value == 0) ? true : false)
			{
				throw new ArgumentOutOfRangeException("MaxDegreeOfParallelism");
			}
			maxDegreeOfParallelism = value;
		}
	}

	public int StreamProcessingBufferSize
	{
		get
		{
			return streamProcessingBufferSize;
		}
		set
		{
			if (value <= 0)
			{
				throw new ArgumentOutOfRangeException("StreamProcessingBufferSize");
			}
			streamProcessingBufferSize = value;
		}
	}

	public bool PreferContiguousImageBuffers { get; set; }

	public IDictionary<object, object> Properties { get; } = new ConcurrentDictionary<object, object>();

	public IEnumerable<IImageFormat> ImageFormats => ImageFormatsManager.ImageFormats;

	public ReadOrigin ReadOrigin { get; set; } = ReadOrigin.Current;

	public ImageFormatManager ImageFormatsManager { get; private set; } = new ImageFormatManager();

	public MemoryAllocator MemoryAllocator
	{
		get
		{
			return memoryAllocator;
		}
		set
		{
			Guard.NotNull(value, "MemoryAllocator");
			memoryAllocator = value;
		}
	}

	internal int MaxHeaderSize => ImageFormatsManager.MaxHeaderSize;

	internal IFileSystem FileSystem { get; set; } = new LocalFileSystem();

	internal int WorkingBufferSizeHintInBytes { get; set; } = 1048576;

	internal IImageProcessingContextFactory ImageOperationsProvider { get; set; } = new DefaultImageOperationsProviderFactory();

	public Configuration()
	{
	}

	public Configuration(params IImageFormatConfigurationModule[] configurationModules)
	{
		if (configurationModules != null)
		{
			for (int i = 0; i < configurationModules.Length; i++)
			{
				configurationModules[i].Configure(this);
			}
		}
	}

	public void Configure(IImageFormatConfigurationModule configuration)
	{
		Guard.NotNull(configuration, "configuration");
		configuration.Configure(this);
	}

	public Configuration Clone()
	{
		return new Configuration
		{
			MaxDegreeOfParallelism = MaxDegreeOfParallelism,
			StreamProcessingBufferSize = StreamProcessingBufferSize,
			ImageFormatsManager = ImageFormatsManager,
			memoryAllocator = memoryAllocator,
			ImageOperationsProvider = ImageOperationsProvider,
			ReadOrigin = ReadOrigin,
			FileSystem = FileSystem,
			WorkingBufferSizeHintInBytes = WorkingBufferSizeHintInBytes
		};
	}

	internal static Configuration CreateDefaultInstance()
	{
		return new Configuration(new PngConfigurationModule(), new JpegConfigurationModule(), new GifConfigurationModule(), new BmpConfigurationModule(), new PbmConfigurationModule(), new TgaConfigurationModule(), new TiffConfigurationModule(), new WebpConfigurationModule(), new QoiConfigurationModule());
	}
}
