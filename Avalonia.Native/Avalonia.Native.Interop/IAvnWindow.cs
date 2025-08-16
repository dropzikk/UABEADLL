using System;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop;

internal interface IAvnWindow : IAvnWindowBase, IUnknown, IDisposable
{
	AvnWindowState WindowState { get; }

	double ExtendTitleBarHeight { get; }

	void SetEnabled(int enable);

	void SetParent(IAvnWindow parent);

	void SetCanResize(int value);

	void SetDecorations(SystemDecorations value);

	void SetTitle(string utf8Title);

	void SetTitleBarColor(AvnColor color);

	void SetWindowState(AvnWindowState state);

	void TakeFocusFromChildren();

	void SetExtendClientArea(int enable);

	void SetExtendClientAreaHints(AvnExtendClientAreaChromeHints hints);

	void SetExtendTitleBarHeight(double value);
}
