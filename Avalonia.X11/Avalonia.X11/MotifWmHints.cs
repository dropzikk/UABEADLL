using System;

namespace Avalonia.X11;

internal struct MotifWmHints
{
	internal IntPtr flags;

	internal IntPtr functions;

	internal IntPtr decorations;

	internal IntPtr input_mode;

	internal IntPtr status;

	public override string ToString()
	{
		return $"MotifWmHints <flags={flags.ToInt32()}, functions={functions.ToInt32()}, decorations={decorations.ToInt32()}, input_mode={input_mode.ToInt32()}, status={status.ToInt32()}";
	}
}
