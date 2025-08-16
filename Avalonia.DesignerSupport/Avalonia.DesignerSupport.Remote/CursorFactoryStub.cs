using System;
using Avalonia.Input;
using Avalonia.Platform;

namespace Avalonia.DesignerSupport.Remote;

internal class CursorFactoryStub : ICursorFactory
{
	private class CursorStub : ICursorImpl, IDisposable
	{
		public void Dispose()
		{
		}
	}

	public ICursorImpl GetCursor(StandardCursorType cursorType)
	{
		return new CursorStub();
	}

	public ICursorImpl CreateCursor(IBitmapImpl cursor, PixelPoint hotSpot)
	{
		return new CursorStub();
	}
}
