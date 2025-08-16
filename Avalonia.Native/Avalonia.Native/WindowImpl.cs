using System;
using Avalonia.Controls;
using Avalonia.Controls.Platform;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Input.TextInput;
using Avalonia.Media;
using Avalonia.Native.Interop;
using Avalonia.Platform;
using MicroCom.Runtime;

namespace Avalonia.Native;

internal class WindowImpl : WindowBaseImpl, IWindowImpl, IWindowBaseImpl, ITopLevelImpl, IOptionalFeatureProvider, IDisposable
{
	private class WindowEvents : WindowBaseEvents, IAvnWindowEvents, IAvnWindowBaseEvents, IUnknown, IDisposable
	{
		private readonly WindowImpl _parent;

		public WindowEvents(WindowImpl parent)
			: base(parent)
		{
			_parent = parent;
		}

		int IAvnWindowEvents.Closing()
		{
			if (_parent.Closing != null)
			{
				return _parent.Closing(WindowCloseReason.WindowClosing).AsComBool();
			}
			return Extensions.AsComBool(b: true);
		}

		void IAvnWindowEvents.WindowStateChanged(AvnWindowState state)
		{
			_parent.InvalidateExtendedMargins();
			_parent.WindowStateChanged?.Invoke((WindowState)state);
		}

		void IAvnWindowEvents.GotInputWhenDisabled()
		{
			_parent.GotInputWhenDisabled?.Invoke();
		}
	}

	private readonly AvaloniaNativePlatformOptions _opts;

	private readonly AvaloniaNativeGlPlatformGraphics _graphics;

	private IAvnWindow _native;

	private double _extendTitleBarHeight = -1.0;

	private DoubleClickHelper _doubleClickHelper;

	private readonly ITopLevelNativeMenuExporter _nativeMenuExporter;

	private readonly AvaloniaNativeTextInputMethod _inputMethod;

	private bool _canResize = true;

	private bool _isExtended;

	public new IAvnWindow Native => _native;

	public WindowState WindowState
	{
		get
		{
			return (WindowState)_native.WindowState;
		}
		set
		{
			_native.SetWindowState((AvnWindowState)value);
		}
	}

	public Action<WindowState> WindowStateChanged { get; set; }

	public Action<bool> ExtendClientAreaToDecorationsChanged { get; set; }

	public Thickness ExtendedMargins { get; private set; }

	public Thickness OffScreenMargin { get; }

	public bool IsClientAreaExtendedToDecorations => _isExtended;

	public bool NeedsManagedDecorations => false;

	public Func<WindowCloseReason, bool> Closing { get; set; }

	public Action GotInputWhenDisabled { get; set; }

	internal WindowImpl(IAvaloniaNativeFactory factory, AvaloniaNativePlatformOptions opts)
		: base(factory)
	{
		_opts = opts;
		_doubleClickHelper = new DoubleClickHelper();
		using (WindowEvents cb = new WindowEvents(this))
		{
			Init(_native = factory.CreateWindow(cb), factory.CreateScreens());
		}
		_nativeMenuExporter = new AvaloniaNativeMenuExporter(_native, factory);
		_inputMethod = new AvaloniaNativeTextInputMethod(_native);
	}

	public void CanResize(bool value)
	{
		_canResize = value;
		_native.SetCanResize(value.AsComBool());
	}

	public void SetSystemDecorations(Avalonia.Controls.SystemDecorations enabled)
	{
		_native.SetDecorations((Avalonia.Native.Interop.SystemDecorations)enabled);
	}

	public void SetTitleBarColor(Color color)
	{
		_native.SetTitleBarColor(new AvnColor
		{
			Alpha = color.A,
			Red = color.R,
			Green = color.G,
			Blue = color.B
		});
	}

	public void SetTitle(string title)
	{
		_native.SetTitle(title ?? "");
	}

	public override void Show(bool activate, bool isDialog)
	{
		base.Show(activate, isDialog);
		InvalidateExtendedMargins();
	}

	protected override bool ChromeHitTest(RawPointerEventArgs e)
	{
		if (_isExtended && e.Type == RawPointerEventType.LeftButtonDown && ((_inputRoot is Window window) ? window.Renderer.HitTestFirst(e.Position, window, (Visual x) => (!(x is IInputElement inputElement) || (inputElement.IsHitTestVisible && inputElement.IsEffectivelyVisible)) ? true : false) : null) == null)
		{
			if (_doubleClickHelper.IsDoubleClick(e.Timestamp, e.Position))
			{
				if (_canResize)
				{
					WindowState windowState = WindowState;
					WindowState = ((windowState != WindowState.Maximized && windowState != WindowState.FullScreen) ? WindowState.Maximized : WindowState.Normal);
				}
			}
			else
			{
				_native.BeginMoveDrag();
			}
		}
		return false;
	}

	private void InvalidateExtendedMargins()
	{
		if (WindowState == WindowState.FullScreen)
		{
			ExtendedMargins = default(Thickness);
		}
		else
		{
			ExtendedMargins = (_isExtended ? new Thickness(0.0, (_extendTitleBarHeight == -1.0) ? _native.ExtendTitleBarHeight : _extendTitleBarHeight, 0.0, 0.0) : default(Thickness));
		}
		ExtendClientAreaToDecorationsChanged?.Invoke(_isExtended);
	}

	public void SetExtendClientAreaToDecorationsHint(bool extendIntoClientAreaHint)
	{
		_isExtended = extendIntoClientAreaHint;
		_native.SetExtendClientArea(extendIntoClientAreaHint.AsComBool());
		InvalidateExtendedMargins();
	}

	public void SetExtendClientAreaChromeHints(ExtendClientAreaChromeHints hints)
	{
		_native.SetExtendClientAreaHints((AvnExtendClientAreaChromeHints)hints);
	}

	public void SetExtendClientAreaTitleBarHeightHint(double titleBarHeight)
	{
		_extendTitleBarHeight = titleBarHeight;
		_native.SetExtendTitleBarHeight(titleBarHeight);
		ExtendedMargins = (_isExtended ? new Thickness(0.0, (titleBarHeight == -1.0) ? _native.ExtendTitleBarHeight : titleBarHeight, 0.0, 0.0) : default(Thickness));
		ExtendClientAreaToDecorationsChanged?.Invoke(_isExtended);
	}

	public void ShowTaskbarIcon(bool value)
	{
	}

	public void SetIcon(IWindowIconImpl icon)
	{
	}

	public void Move(PixelPoint point)
	{
		base.Position = point;
	}

	public override IPopupImpl CreatePopup()
	{
		if (!_opts.OverlayPopups)
		{
			return new PopupImpl(_factory, this);
		}
		return null;
	}

	public void SetParent(IWindowImpl parent)
	{
		_native.SetParent(((WindowImpl)parent).Native);
	}

	public void SetEnabled(bool enable)
	{
		_native.SetEnabled(enable.AsComBool());
	}

	public override object TryGetFeature(Type featureType)
	{
		if (featureType == typeof(ITextInputMethodImpl))
		{
			return _inputMethod;
		}
		if (featureType == typeof(ITopLevelNativeMenuExporter))
		{
			return _nativeMenuExporter;
		}
		return base.TryGetFeature(featureType);
	}
}
