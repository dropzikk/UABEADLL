using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Platform;
using Avalonia.Rendering;
using Avalonia.Rendering.Composition;

namespace Avalonia.DesignerSupport.Remote;

internal class WindowStub : IWindowImpl, IWindowBaseImpl, ITopLevelImpl, IOptionalFeatureProvider, IDisposable, IPopupImpl
{
	private class DummyRenderTimer : IRenderTimer
	{
		public bool RunsInBackground => false;

		public event Action<TimeSpan> Tick;
	}

	public Action Deactivated { get; set; }

	public Action Activated { get; set; }

	public IPlatformHandle Handle { get; }

	public Size MaxAutoSizeHint { get; }

	public Size ClientSize { get; }

	public Size? FrameSize => null;

	public double RenderScaling { get; } = 1.0;

	public double DesktopScaling => 1.0;

	public IEnumerable<object> Surfaces { get; }

	public Action<RawInputEventArgs> Input { get; set; }

	public Action<Rect> Paint { get; set; }

	public Action<Size, WindowResizeReason> Resized { get; set; }

	public Action<double> ScalingChanged { get; set; }

	public Func<WindowCloseReason, bool> Closing { get; set; }

	public Action Closed { get; set; }

	public Action LostFocus { get; set; }

	public IMouseDevice MouseDevice { get; } = new MouseDevice();

	public PixelPoint Position { get; set; }

	public Action<PixelPoint> PositionChanged { get; set; }

	public WindowState WindowState { get; set; }

	public Action<WindowState> WindowStateChanged { get; set; }

	public Action<WindowTransparencyLevel> TransparencyLevelChanged { get; set; }

	public Action<bool> ExtendClientAreaToDecorationsChanged { get; set; }

	public Thickness ExtendedMargins { get; }

	public Thickness OffScreenMargin { get; }

	public Compositor Compositor { get; } = new Compositor(new RenderLoop(new DummyRenderTimer()), null);

	public IScreenImpl Screen { get; } = new ScreenStub();

	public IPopupPositioner PopupPositioner { get; }

	public Action GotInputWhenDisabled { get; set; }

	public WindowTransparencyLevel TransparencyLevel => WindowTransparencyLevel.None;

	public bool IsClientAreaExtendedToDecorations { get; }

	public bool NeedsManagedDecorations => false;

	public AcrylicPlatformCompensationLevels AcrylicCompensationLevels { get; } = new AcrylicPlatformCompensationLevels(1.0, 1.0, 1.0);

	public IPopupImpl CreatePopup()
	{
		return new WindowStub(this);
	}

	public WindowStub(IWindowImpl parent = null)
	{
		if (parent != null)
		{
			PopupPositioner = new ManagedPopupPositioner(new ManagedPopupPositionerPopupImplHelper(parent, delegate(PixelPoint _, Size size, double __)
			{
				Resize(size, WindowResizeReason.Unspecified);
			}));
		}
	}

	public void Dispose()
	{
	}

	public void Invalidate(Rect rect)
	{
	}

	public void SetInputRoot(IInputRoot inputRoot)
	{
	}

	public Point PointToClient(PixelPoint p)
	{
		return p.ToPoint(1.0);
	}

	public PixelPoint PointToScreen(Point p)
	{
		return PixelPoint.FromPoint(p, 1.0);
	}

	public void SetCursor(ICursorImpl cursor)
	{
	}

	public void Show(bool activate, bool isDialog)
	{
	}

	public void Hide()
	{
	}

	public void BeginMoveDrag(PointerPressedEventArgs e)
	{
	}

	public void BeginResizeDrag(WindowEdge edge, PointerPressedEventArgs e)
	{
	}

	public void Activate()
	{
	}

	public void Resize(Size clientSize, WindowResizeReason reason)
	{
	}

	public void Move(PixelPoint point)
	{
	}

	public void SetMinMaxSize(Size minSize, Size maxSize)
	{
	}

	public void SetTitle(string title)
	{
	}

	public void ShowDialog(IWindowImpl parent)
	{
	}

	public void SetSystemDecorations(SystemDecorations enabled)
	{
	}

	public void SetIcon(IWindowIconImpl icon)
	{
	}

	public void ShowTaskbarIcon(bool value)
	{
	}

	public void CanResize(bool value)
	{
	}

	public void SetTopmost(bool value)
	{
	}

	public void SetParent(IWindowImpl parent)
	{
	}

	public void SetEnabled(bool enable)
	{
	}

	public void SetExtendClientAreaToDecorationsHint(bool extendIntoClientAreaHint)
	{
	}

	public void SetExtendClientAreaChromeHints(ExtendClientAreaChromeHints hints)
	{
	}

	public void SetExtendClientAreaTitleBarHeightHint(double titleBarHeight)
	{
	}

	public void SetTransparencyLevelHint(IReadOnlyList<WindowTransparencyLevel> transparencyLevel)
	{
	}

	public void SetWindowManagerAddShadowHint(bool enabled)
	{
	}

	public void SetFrameThemeVariant(PlatformThemeVariant themeVariant)
	{
	}

	public object TryGetFeature(Type featureType)
	{
		return null;
	}
}
