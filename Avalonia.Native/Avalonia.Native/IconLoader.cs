using System.IO;
using Avalonia.Platform;

namespace Avalonia.Native;

internal class IconLoader : IPlatformIconLoader
{
	private class IconStub : IWindowIconImpl
	{
		private readonly IBitmapImpl _bitmap;

		public IconStub(IBitmapImpl bitmap)
		{
			_bitmap = bitmap;
		}

		public void Save(Stream outputStream)
		{
			_bitmap.Save(outputStream, null);
		}
	}

	public IWindowIconImpl LoadIcon(string fileName)
	{
		return new IconStub(AvaloniaLocator.Current.GetService<IPlatformRenderInterface>().LoadBitmap(fileName));
	}

	public IWindowIconImpl LoadIcon(Stream stream)
	{
		return new IconStub(AvaloniaLocator.Current.GetService<IPlatformRenderInterface>().LoadBitmap(stream));
	}

	public IWindowIconImpl LoadIcon(IBitmapImpl bitmap)
	{
		MemoryStream memoryStream = new MemoryStream();
		bitmap.Save(memoryStream, null);
		memoryStream.Seek(0L, SeekOrigin.Begin);
		return LoadIcon(memoryStream);
	}
}
