using System;
using Avalonia.Metadata;

namespace Avalonia.Input.Raw;

[PrivateApi]
public class RawInputEventArgs : EventArgs
{
	public IInputDevice Device { get; }

	public IInputRoot Root { get; }

	public bool Handled { get; set; }

	public ulong Timestamp { get; set; }

	public RawInputEventArgs(IInputDevice device, ulong timestamp, IInputRoot root)
	{
		Device = device ?? throw new ArgumentNullException("device");
		Timestamp = timestamp;
		Root = root ?? throw new ArgumentNullException("root");
	}
}
