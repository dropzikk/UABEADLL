using System;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop;

internal interface IAvaloniaNativeFactory : IUnknown, IDisposable
{
	IAvnMacOptions MacOptions { get; }

	void Initialize(IAvnGCHandleDeallocatorCallback deallocator, IAvnApplicationEvents appCb, IAvnDispatcher dispatcher);

	IAvnWindow CreateWindow(IAvnWindowEvents cb);

	IAvnPopup CreatePopup(IAvnWindowEvents cb);

	IAvnPlatformThreadingInterface CreatePlatformThreadingInterface();

	IAvnSystemDialogs CreateSystemDialogs();

	IAvnScreens CreateScreens();

	IAvnClipboard CreateClipboard();

	IAvnClipboard CreateDndClipboard();

	IAvnCursorFactory CreateCursorFactory();

	IAvnGlDisplay ObtainGlDisplay();

	IAvnMetalDisplay ObtainMetalDisplay();

	void SetAppMenu(IAvnMenu menu);

	void SetServicesMenu(IAvnMenu menu);

	IAvnMenu CreateMenu(IAvnMenuEvents cb);

	IAvnMenuItem CreateMenuItem();

	IAvnMenuItem CreateMenuItemSeparator();

	IAvnTrayIcon CreateTrayIcon();

	IAvnApplicationCommands CreateApplicationCommands();

	IAvnPlatformSettings CreatePlatformSettings();

	IAvnPlatformBehaviorInhibition CreatePlatformBehaviorInhibition();
}
