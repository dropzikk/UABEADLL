using System;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT;

internal interface ICompositorWithBlurredWallpaperBackdropBrush : IInspectable, IUnknown, IDisposable
{
	ICompositionBackdropBrush TryCreateBlurredWallpaperBackdropBrush();
}
