using System;
using Avalonia.Native.Interop;
using Avalonia.Platform;

namespace Avalonia.Native;

internal class AvaloniaNativeCursor : ICursorImpl, IDisposable
{
	public IAvnCursor Cursor { get; private set; }

	public IntPtr Handle => IntPtr.Zero;

	public string HandleDescriptor => "<none>";

	public AvaloniaNativeCursor(IAvnCursor cursor)
	{
		Cursor = cursor;
	}

	public void Dispose()
	{
		Cursor.Dispose();
		Cursor = null;
	}
}
