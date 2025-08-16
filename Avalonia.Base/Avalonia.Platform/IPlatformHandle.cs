using System;

namespace Avalonia.Platform;

public interface IPlatformHandle
{
	IntPtr Handle { get; }

	string? HandleDescriptor { get; }
}
