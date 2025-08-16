using System;
using System.IO;
using Avalonia.Metadata;
using Avalonia.Platform;
using Avalonia.Utilities;

namespace Avalonia.Media.Imaging;

[NotClientImplementable]
internal interface IBitmap : IImage, IDisposable
{
	Vector Dpi { get; }

	PixelSize PixelSize { get; }

	IRef<IBitmapImpl> PlatformImpl { get; }

	void Save(string fileName, int? quality = null);

	void Save(Stream stream, int? quality = null);
}
