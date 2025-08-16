using System;
using Avalonia.Interactivity;

namespace AvaloniaEdit.Rendering;

public sealed class OpenUriRoutedEventArgs : RoutedEventArgs
{
	public Uri Uri { get; }

	public OpenUriRoutedEventArgs(Uri uri)
	{
		Uri = uri ?? throw new ArgumentNullException("uri");
	}
}
