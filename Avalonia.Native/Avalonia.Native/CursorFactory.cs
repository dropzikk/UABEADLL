using System;
using System.IO;
using Avalonia.Input;
using Avalonia.Native.Interop;
using Avalonia.Platform;

namespace Avalonia.Native;

internal class CursorFactory : ICursorFactory
{
	private IAvnCursorFactory _native;

	public CursorFactory(IAvnCursorFactory native)
	{
		_native = native;
	}

	public ICursorImpl GetCursor(StandardCursorType cursorType)
	{
		return new AvaloniaNativeCursor(_native.GetCursor((AvnStandardCursorType)cursorType));
	}

	public unsafe ICursorImpl CreateCursor(IBitmapImpl cursor, PixelPoint hotSpot)
	{
		using MemoryStream memoryStream = new MemoryStream();
		cursor.Save(memoryStream, null);
		byte[] array = memoryStream.ToArray();
		fixed (byte* ptr = array)
		{
			void* bitmapData = ptr;
			return new AvaloniaNativeCursor(_native.CreateCustomCursor(bitmapData, new IntPtr(array.Length), new AvnPixelSize
			{
				Width = hotSpot.X,
				Height = hotSpot.Y
			}));
		}
	}
}
