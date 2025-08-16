using System;

namespace Avalonia.X11;

internal struct XIMText
{
	public ushort Length;

	public IntPtr Feedback;

	public int EncodingIsWChar;

	public IntPtr String;
}
