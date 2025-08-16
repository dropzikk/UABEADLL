using System;
using System.Collections.Generic;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Metadata;

namespace SixLabors.ImageSharp;

public class ImageInfo
{
	public PixelTypeInfo PixelType { get; }

	public int Width => Size.Width;

	public int Height => Size.Height;

	public ImageMetadata Metadata { get; }

	public IReadOnlyList<ImageFrameMetadata> FrameMetadataCollection { get; }

	public Size Size { get; internal set; }

	public Rectangle Bounds => new Rectangle(0, 0, Width, Height);

	public ImageInfo(PixelTypeInfo pixelType, Size size, ImageMetadata? metadata)
		: this(pixelType, size, metadata, null)
	{
	}

	public ImageInfo(PixelTypeInfo pixelType, Size size, ImageMetadata? metadata, IReadOnlyList<ImageFrameMetadata>? frameMetadataCollection)
	{
		PixelType = pixelType;
		Size = size;
		Metadata = metadata ?? new ImageMetadata();
		FrameMetadataCollection = frameMetadataCollection ?? Array.Empty<ImageFrameMetadata>();
	}
}
