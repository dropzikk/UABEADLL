using System;
using Avalonia.Platform;

namespace Avalonia.X11;

internal class CursorImpl : ICursorImpl, IDisposable
{
	public IntPtr Handle { get; protected set; }

	public CursorImpl()
	{
	}

	public CursorImpl(IntPtr handle)
	{
		Handle = handle;
	}

	public virtual void Dispose()
	{
	}
}
