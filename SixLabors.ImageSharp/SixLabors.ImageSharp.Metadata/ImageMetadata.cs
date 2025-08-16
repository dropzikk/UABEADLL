using System.Collections.Generic;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Metadata.Profiles.Cicp;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using SixLabors.ImageSharp.Metadata.Profiles.Icc;
using SixLabors.ImageSharp.Metadata.Profiles.Iptc;
using SixLabors.ImageSharp.Metadata.Profiles.Xmp;

namespace SixLabors.ImageSharp.Metadata;

public sealed class ImageMetadata : IDeepCloneable<ImageMetadata>
{
	public const double DefaultHorizontalResolution = 96.0;

	public const double DefaultVerticalResolution = 96.0;

	public const PixelResolutionUnit DefaultPixelResolutionUnits = PixelResolutionUnit.PixelsPerInch;

	private readonly Dictionary<IImageFormat, IDeepCloneable> formatMetadata = new Dictionary<IImageFormat, IDeepCloneable>();

	private double horizontalResolution;

	private double verticalResolution;

	public double HorizontalResolution
	{
		get
		{
			return horizontalResolution;
		}
		set
		{
			if (value > 0.0)
			{
				horizontalResolution = value;
			}
		}
	}

	public double VerticalResolution
	{
		get
		{
			return verticalResolution;
		}
		set
		{
			if (value > 0.0)
			{
				verticalResolution = value;
			}
		}
	}

	public PixelResolutionUnit ResolutionUnits { get; set; }

	public ExifProfile? ExifProfile { get; set; }

	public XmpProfile? XmpProfile { get; set; }

	public IccProfile? IccProfile { get; set; }

	public IptcProfile? IptcProfile { get; set; }

	public CicpProfile? CicpProfile { get; set; }

	public IImageFormat? DecodedImageFormat { get; internal set; }

	public ImageMetadata()
	{
		horizontalResolution = 96.0;
		verticalResolution = 96.0;
		ResolutionUnits = PixelResolutionUnit.PixelsPerInch;
	}

	private ImageMetadata(ImageMetadata other)
	{
		HorizontalResolution = other.HorizontalResolution;
		VerticalResolution = other.VerticalResolution;
		ResolutionUnits = other.ResolutionUnits;
		foreach (KeyValuePair<IImageFormat, IDeepCloneable> formatMetadatum in other.formatMetadata)
		{
			formatMetadata.Add(formatMetadatum.Key, formatMetadatum.Value.DeepClone());
		}
		ExifProfile = other.ExifProfile?.DeepClone();
		IccProfile = other.IccProfile?.DeepClone();
		IptcProfile = other.IptcProfile?.DeepClone();
		XmpProfile = other.XmpProfile?.DeepClone();
		CicpProfile = other.CicpProfile?.DeepClone();
		DecodedImageFormat = other.DecodedImageFormat;
	}

	public TFormatMetadata GetFormatMetadata<TFormatMetadata>(IImageFormat<TFormatMetadata> key) where TFormatMetadata : class, IDeepCloneable
	{
		if (formatMetadata.TryGetValue(key, out IDeepCloneable value))
		{
			return (TFormatMetadata)value;
		}
		TFormatMetadata val = key.CreateDefaultFormatMetadata();
		formatMetadata[key] = val;
		return val;
	}

	public bool TryGetFormatMetadata<TFormatMetadata>(IImageFormat<TFormatMetadata> key, out TFormatMetadata? metadata) where TFormatMetadata : class, IDeepCloneable
	{
		if (formatMetadata.TryGetValue(key, out IDeepCloneable value))
		{
			metadata = (TFormatMetadata)value;
			return true;
		}
		metadata = null;
		return false;
	}

	public ImageMetadata DeepClone()
	{
		return new ImageMetadata(this);
	}

	internal void SyncProfiles()
	{
		ExifProfile?.Sync(this);
	}
}
