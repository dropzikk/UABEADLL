using System;
using Avalonia.Win32.Interop;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT;

internal interface ICompositionDrawingSurfaceInterop : IUnknown, IDisposable
{
	unsafe UnmanagedMethods.POINT BeginDraw(UnmanagedMethods.RECT* updateRect, Guid* iid, void** updateObject);

	void EndDraw();

	void Resize(UnmanagedMethods.POINT sizePixels);

	unsafe void Scroll(UnmanagedMethods.RECT* scrollRect, UnmanagedMethods.RECT* clipRect, int offsetX, int offsetY);

	void ResumeDraw();

	void SuspendDraw();
}
