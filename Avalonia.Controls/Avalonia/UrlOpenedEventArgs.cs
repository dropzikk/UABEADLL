using System;

namespace Avalonia;

public class UrlOpenedEventArgs : EventArgs
{
	public string[] Urls { get; }

	public UrlOpenedEventArgs(string[] urls)
	{
		Urls = urls;
	}
}
