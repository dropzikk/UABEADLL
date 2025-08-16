using System;
using System.IO;
using Avalonia.Metadata;

namespace Avalonia.Platform;

[PrivateApi]
public interface IBitmapImpl : IDisposable
{
	Vector Dpi { get; }

	PixelSize PixelSize { get; }

	int Version { get; }

	void Save(string fileName, int? quality = null);

	void Save(Stream stream, int? quality = null);
}
