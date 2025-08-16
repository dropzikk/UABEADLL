using System.IO;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace Avalonia.X11;

internal class X11IconLoader : IPlatformIconLoader
{
	private static IWindowIconImpl LoadIcon(Bitmap bitmap)
	{
		X11IconData result = new X11IconData(bitmap);
		bitmap.Dispose();
		return result;
	}

	public IWindowIconImpl LoadIcon(string fileName)
	{
		return LoadIcon(new Bitmap(fileName));
	}

	public IWindowIconImpl LoadIcon(Stream stream)
	{
		return LoadIcon(new Bitmap(stream));
	}

	public IWindowIconImpl LoadIcon(IBitmapImpl bitmap)
	{
		MemoryStream memoryStream = new MemoryStream();
		bitmap.Save(memoryStream, null);
		memoryStream.Position = 0L;
		return LoadIcon(memoryStream);
	}
}
