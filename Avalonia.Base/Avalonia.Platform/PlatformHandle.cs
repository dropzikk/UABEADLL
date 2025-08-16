using System;

namespace Avalonia.Platform;

public class PlatformHandle : IPlatformHandle
{
	public IntPtr Handle { get; }

	public string? HandleDescriptor { get; }

	public PlatformHandle(IntPtr handle, string? descriptor)
	{
		Handle = handle;
		HandleDescriptor = descriptor;
	}
}
