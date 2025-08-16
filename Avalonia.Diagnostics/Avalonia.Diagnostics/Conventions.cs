using System;
using System.IO;
using Avalonia.Diagnostics.Screenshots;

namespace Avalonia.Diagnostics;

internal static class Conventions
{
	public static string DefaultScreenshotsRoot
	{
		get
		{
			string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "Screenshots");
			Directory.CreateDirectory(text);
			return text;
		}
	}

	public static IScreenshotHandler DefaultScreenshotHandler { get; } = new FilePickerHandler();
}
