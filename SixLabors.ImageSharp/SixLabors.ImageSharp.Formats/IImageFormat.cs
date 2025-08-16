using System.Collections.Generic;

namespace SixLabors.ImageSharp.Formats;

public interface IImageFormat
{
	string Name { get; }

	string DefaultMimeType { get; }

	IEnumerable<string> MimeTypes { get; }

	IEnumerable<string> FileExtensions { get; }
}
public interface IImageFormat<out TFormatMetadata> : IImageFormat where TFormatMetadata : class
{
	TFormatMetadata CreateDefaultFormatMetadata();
}
public interface IImageFormat<out TFormatMetadata, out TFormatFrameMetadata> : IImageFormat<TFormatMetadata>, IImageFormat where TFormatMetadata : class where TFormatFrameMetadata : class
{
	TFormatFrameMetadata CreateDefaultFormatFrameMetadata();
}
