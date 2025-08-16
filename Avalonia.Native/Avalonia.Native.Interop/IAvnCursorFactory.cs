using System;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop;

internal interface IAvnCursorFactory : IUnknown, IDisposable
{
	IAvnCursor GetCursor(AvnStandardCursorType cursorType);

	unsafe IAvnCursor CreateCustomCursor(void* bitmapData, IntPtr length, AvnPixelSize hotPixel);
}
