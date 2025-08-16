using System;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Transforms;

namespace SixLabors.ImageSharp.Formats;

public sealed class DecoderOptions
{
	private static readonly Lazy<DecoderOptions> LazyOptions = new Lazy<DecoderOptions>(() => new DecoderOptions());

	private uint maxFrames = 2147483647u;

	private Configuration configuration = SixLabors.ImageSharp.Configuration.Default;

	internal static DecoderOptions Default { get; } = LazyOptions.Value;

	public Configuration Configuration
	{
		get
		{
			return configuration;
		}
		init
		{
			configuration = value;
		}
	}

	public Size? TargetSize { get; init; }

	public IResampler Sampler { get; init; } = KnownResamplers.Box;

	public bool SkipMetadata { get; init; }

	public uint MaxFrames
	{
		get
		{
			return maxFrames;
		}
		init
		{
			maxFrames = Math.Clamp(value, 1u, 2147483647u);
		}
	}

	internal void SetConfiguration(Configuration configuration)
	{
		this.configuration = configuration;
	}
}
