using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Metadata;

namespace Avalonia.Platform;

[Unstable]
public interface IWindowImpl : IWindowBaseImpl, ITopLevelImpl, IOptionalFeatureProvider, IDisposable
{
	WindowState WindowState { get; set; }

	Action<WindowState>? WindowStateChanged { get; set; }

	Action? GotInputWhenDisabled { get; set; }

	Func<WindowCloseReason, bool>? Closing { get; set; }

	bool IsClientAreaExtendedToDecorations { get; }

	Action<bool>? ExtendClientAreaToDecorationsChanged { get; set; }

	bool NeedsManagedDecorations { get; }

	Thickness ExtendedMargins { get; }

	Thickness OffScreenMargin { get; }

	void SetTitle(string? title);

	void SetParent(IWindowImpl parent);

	void SetEnabled(bool enable);

	void SetSystemDecorations(SystemDecorations enabled);

	void SetIcon(IWindowIconImpl? icon);

	void ShowTaskbarIcon(bool value);

	void CanResize(bool value);

	void BeginMoveDrag(PointerPressedEventArgs e);

	void BeginResizeDrag(WindowEdge edge, PointerPressedEventArgs e);

	void Resize(Size clientSize, WindowResizeReason reason = WindowResizeReason.Application);

	void Move(PixelPoint point);

	void SetMinMaxSize(Size minSize, Size maxSize);

	void SetExtendClientAreaToDecorationsHint(bool extendIntoClientAreaHint);

	void SetExtendClientAreaChromeHints(ExtendClientAreaChromeHints hints);

	void SetExtendClientAreaTitleBarHeightHint(double titleBarHeight);
}
