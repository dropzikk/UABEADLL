using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using Avalonia.Automation.Peers;
using Avalonia.Collections.Pooled;
using Avalonia.Controls;
using Avalonia.Controls.Platform;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Input.Raw;
using Avalonia.Input.TextInput;
using Avalonia.OpenGL.Egl;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using Avalonia.Rendering.Composition;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Avalonia.Win32.Automation;
using Avalonia.Win32.DirectX;
using Avalonia.Win32.Input;
using Avalonia.Win32.Interop;
using Avalonia.Win32.Interop.Automation;
using Avalonia.Win32.OpenGl;
using Avalonia.Win32.OpenGl.Angle;
using Avalonia.Win32.WinRT;
using Avalonia.Win32.WinRT.Composition;

namespace Avalonia.Win32;

internal class WindowImpl : IWindowImpl, IWindowBaseImpl, ITopLevelImpl, IOptionalFeatureProvider, IDisposable, EglGlPlatformSurface.IEglWindowGlPlatformSurfaceInfo
{
	private struct SavedWindowInfo
	{
		public UnmanagedMethods.WindowStyles Style { get; set; }

		public UnmanagedMethods.WindowStyles ExStyle { get; set; }

		public UnmanagedMethods.RECT WindowRect { get; set; }
	}

	private struct WindowProperties
	{
		public bool ShowInTaskbar;

		public bool IsResizable;

		public SystemDecorations Decorations;

		public bool IsFullScreen;
	}

	private struct ResizeReasonScope : IDisposable
	{
		private readonly WindowImpl _owner;

		private readonly WindowResizeReason _restore;

		public ResizeReasonScope(WindowImpl owner, WindowResizeReason restore)
		{
			_owner = owner;
			_restore = restore;
		}

		public void Dispose()
		{
			_owner._resizeReason = _restore;
		}
	}

	private class WindowImplPlatformHandle : INativePlatformHandleSurface, IPlatformHandle
	{
		private readonly WindowImpl _owner;

		public IntPtr Handle => _owner.Hwnd;

		public string HandleDescriptor => "HWND";

		public PixelSize Size => PixelSize.FromSize(_owner.ClientSize, Scaling);

		public double Scaling => _owner.RenderScaling;

		public WindowImplPlatformHandle(WindowImpl owner)
		{
			_owner = owner;
		}
	}

	private static readonly List<WindowImpl> s_instances = new List<WindowImpl>();

	private static readonly IntPtr s_defaultCursor = UnmanagedMethods.LoadCursor(IntPtr.Zero, new IntPtr(32512));

	private static readonly Dictionary<WindowEdge, UnmanagedMethods.HitTestValues> s_edgeLookup = new Dictionary<WindowEdge, UnmanagedMethods.HitTestValues>
	{
		{
			WindowEdge.East,
			UnmanagedMethods.HitTestValues.HTRIGHT
		},
		{
			WindowEdge.North,
			UnmanagedMethods.HitTestValues.HTTOP
		},
		{
			WindowEdge.NorthEast,
			UnmanagedMethods.HitTestValues.HTTOPRIGHT
		},
		{
			WindowEdge.NorthWest,
			UnmanagedMethods.HitTestValues.HTTOPLEFT
		},
		{
			WindowEdge.South,
			UnmanagedMethods.HitTestValues.HTBOTTOM
		},
		{
			WindowEdge.SouthEast,
			UnmanagedMethods.HitTestValues.HTBOTTOMRIGHT
		},
		{
			WindowEdge.SouthWest,
			UnmanagedMethods.HitTestValues.HTBOTTOMLEFT
		},
		{
			WindowEdge.West,
			UnmanagedMethods.HitTestValues.HTLEFT
		}
	};

	private SavedWindowInfo _savedWindowInfo;

	private bool _isFullScreenActive;

	private bool _isClientAreaExtended;

	private Thickness _extendedMargins;

	private Thickness _offScreenMargin;

	private double _extendTitleBarHint = -1.0;

	private readonly bool _isUsingComposition;

	private readonly IBlurHost? _blurHost;

	private WindowResizeReason _resizeReason;

	private UnmanagedMethods.MOUSEMOVEPOINT _lastWmMousePoint;

	private const UnmanagedMethods.WindowStyles WindowStateMask = UnmanagedMethods.WindowStyles.WS_MAXIMIZE | UnmanagedMethods.WindowStyles.WS_MINIMIZE;

	private readonly TouchDevice _touchDevice;

	private readonly WindowsMouseDevice _mouseDevice;

	private readonly PenDevice _penDevice;

	private readonly FramebufferManager _framebuffer;

	private readonly object? _gl;

	private readonly bool _wmPointerEnabled;

	private readonly Win32NativeControlHost _nativeControlHost;

	private readonly IStorageProvider _storageProvider;

	private UnmanagedMethods.WndProc _wndProcDelegate;

	private string? _className;

	private IntPtr _hwnd;

	private IInputRoot? _owner;

	private WindowProperties _windowProperties;

	private bool _trackingMouse;

	private bool _topmost;

	private double _scaling = 1.0;

	private WindowState _showWindowState;

	private WindowState _lastWindowState;

	private OleDropTarget? _dropTarget;

	private Size _minSize;

	private Size _maxSize;

	private UnmanagedMethods.POINT _maxTrackSize;

	private WindowImpl? _parent;

	private ExtendClientAreaChromeHints _extendChromeHints = ExtendClientAreaChromeHints.Default;

	private bool _isCloseRequested;

	private bool _shown;

	private bool _hiddenWindowIsParent;

	private uint _langid;

	internal bool _ignoreWmChar;

	private WindowTransparencyLevel _transparencyLevel;

	private const int MaxPointerHistorySize = 512;

	private static readonly PooledList<RawPointerPoint> s_intermediatePointsPooledList = new PooledList<RawPointerPoint>();

	private static UnmanagedMethods.POINTER_TOUCH_INFO[]? s_historyTouchInfos;

	private static UnmanagedMethods.POINTER_PEN_INFO[]? s_historyPenInfos;

	private static UnmanagedMethods.POINTER_INFO[]? s_historyInfos;

	private static UnmanagedMethods.MOUSEMOVEPOINT[]? s_mouseHistoryInfos;

	private PlatformThemeVariant _currentThemeVariant;

	private const int MF_BYCOMMAND = 0;

	private const int MF_ENABLED = 0;

	private const int MF_GRAYED = 1;

	private const int MF_DISABLED = 2;

	private const int SC_CLOSE = 61536;

	internal IInputRoot Owner => _owner ?? throw new InvalidOperationException("SetInputRoot must have been called");

	public Action? Activated { get; set; }

	public Func<WindowCloseReason, bool>? Closing { get; set; }

	public Action? Closed { get; set; }

	public Action? Deactivated { get; set; }

	public Action<RawInputEventArgs>? Input { get; set; }

	public Action<Rect>? Paint { get; set; }

	public Action<Size, WindowResizeReason>? Resized { get; set; }

	public Action<double>? ScalingChanged { get; set; }

	public Action<PixelPoint>? PositionChanged { get; set; }

	public Action<WindowState>? WindowStateChanged { get; set; }

	public Action? LostFocus { get; set; }

	public Action<WindowTransparencyLevel>? TransparencyLevelChanged { get; set; }

	public Thickness BorderThickness
	{
		get
		{
			if (HasFullDecorations)
			{
				UnmanagedMethods.WindowStyles style = GetStyle();
				UnmanagedMethods.WindowStyles extendedStyle = GetExtendedStyle();
				UnmanagedMethods.RECT lpRect = default(UnmanagedMethods.RECT);
				if (UnmanagedMethods.AdjustWindowRectEx(ref lpRect, (uint)style, bMenu: false, (uint)extendedStyle))
				{
					return new Thickness(-lpRect.left, -lpRect.top, lpRect.right, lpRect.bottom);
				}
				throw new Win32Exception();
			}
			return default(Thickness);
		}
	}

	private double PrimaryScreenRenderScaling => Screen.AllScreens.FirstOrDefault((Screen screen) => screen.IsPrimary)?.Scaling ?? 1.0;

	public double RenderScaling => _scaling;

	public double DesktopScaling => RenderScaling;

	public Size ClientSize
	{
		get
		{
			UnmanagedMethods.GetClientRect(_hwnd, out var lpRect);
			return new Size(lpRect.right, lpRect.bottom) / RenderScaling;
		}
	}

	public Size? FrameSize
	{
		get
		{
			if (UnmanagedMethods.DwmIsCompositionEnabled(out var enabled) != 0 || !enabled)
			{
				UnmanagedMethods.GetWindowRect(_hwnd, out var lpRect);
				return new Size(lpRect.Width, lpRect.Height) / RenderScaling;
			}
			UnmanagedMethods.DwmGetWindowAttribute(_hwnd, 9, out var pvAttribute, Marshal.SizeOf<UnmanagedMethods.RECT>());
			return new Size(pvAttribute.Width, pvAttribute.Height) / RenderScaling;
		}
	}

	public IScreenImpl Screen { get; }

	public IPlatformHandle Handle { get; private set; }

	public virtual Size MaxAutoSizeHint => new Size((double)_maxTrackSize.X / RenderScaling, (double)_maxTrackSize.Y / RenderScaling);

	public IMouseDevice MouseDevice => _mouseDevice;

	public WindowState WindowState
	{
		get
		{
			if (!UnmanagedMethods.IsWindowVisible(_hwnd))
			{
				return _showWindowState;
			}
			if (_isFullScreenActive)
			{
				return WindowState.FullScreen;
			}
			UnmanagedMethods.WINDOWPLACEMENT lpwndpl = default(UnmanagedMethods.WINDOWPLACEMENT);
			UnmanagedMethods.GetWindowPlacement(_hwnd, ref lpwndpl);
			return lpwndpl.ShowCmd switch
			{
				UnmanagedMethods.ShowWindowCommand.Maximize => WindowState.Maximized, 
				UnmanagedMethods.ShowWindowCommand.Minimize => WindowState.Minimized, 
				_ => WindowState.Normal, 
			};
		}
		set
		{
			if (UnmanagedMethods.IsWindowVisible(_hwnd) && _lastWindowState != value)
			{
				ShowWindow(value, value != WindowState.Minimized);
			}
			_lastWindowState = value;
			_showWindowState = value;
		}
	}

	public WindowTransparencyLevel TransparencyLevel
	{
		get
		{
			return _transparencyLevel;
		}
		private set
		{
			if (_transparencyLevel != value)
			{
				_transparencyLevel = value;
				TransparencyLevelChanged?.Invoke(value);
			}
		}
	}

	protected IntPtr Hwnd => _hwnd;

	private bool IsMouseInPointerEnabled
	{
		get
		{
			if (_wmPointerEnabled)
			{
				return UnmanagedMethods.IsMouseInPointerEnabled();
			}
			return false;
		}
	}

	public IEnumerable<object> Surfaces
	{
		get
		{
			if (_gl == null)
			{
				return new object[2] { Handle, _framebuffer };
			}
			return new object[3] { Handle, _gl, _framebuffer };
		}
	}

	public PixelPoint Position
	{
		get
		{
			UnmanagedMethods.GetWindowRect(_hwnd, out var lpRect);
			PixelSize hiddenBorderSize = HiddenBorderSize;
			return new PixelPoint(lpRect.left + hiddenBorderSize.Width, lpRect.top + hiddenBorderSize.Height);
		}
		set
		{
			PixelSize hiddenBorderSize = HiddenBorderSize;
			value = new PixelPoint(value.X - hiddenBorderSize.Width, value.Y - hiddenBorderSize.Height);
			UnmanagedMethods.SetWindowPos(Handle.Handle, IntPtr.Zero, value.X, value.Y, 0, 0, UnmanagedMethods.SetWindowPosFlags.SWP_NOACTIVATE | UnmanagedMethods.SetWindowPosFlags.SWP_NOSIZE | UnmanagedMethods.SetWindowPosFlags.SWP_NOZORDER);
		}
	}

	private bool HasFullDecorations => _windowProperties.Decorations == SystemDecorations.Full;

	private PixelSize HiddenBorderSize
	{
		get
		{
			if (Win32Platform.WindowsVersion.Major < 10 || !HasFullDecorations)
			{
				return PixelSize.Empty;
			}
			UnmanagedMethods.DwmGetWindowAttribute(_hwnd, 9, out var pvAttribute, Marshal.SizeOf(typeof(UnmanagedMethods.RECT)));
			UnmanagedMethods.GetWindowRect(_hwnd, out var lpRect);
			int systemMetrics = UnmanagedMethods.GetSystemMetrics(UnmanagedMethods.SystemMetric.SM_CXBORDER);
			return new PixelSize(pvAttribute.left - lpRect.left - systemMetrics, 0);
		}
	}

	public Compositor Compositor => Win32Platform.Compositor;

	public Action? GotInputWhenDisabled { get; set; }

	PixelSize EglGlPlatformSurface.IEglWindowGlPlatformSurfaceInfo.Size
	{
		get
		{
			UnmanagedMethods.GetClientRect(_hwnd, out var lpRect);
			return new PixelSize(Math.Max(1, lpRect.right - lpRect.left), Math.Max(1, lpRect.bottom - lpRect.top));
		}
	}

	double EglGlPlatformSurface.IEglWindowGlPlatformSurfaceInfo.Scaling => RenderScaling;

	IntPtr EglGlPlatformSurface.IEglWindowGlPlatformSurfaceInfo.Handle => Handle.Handle;

	public bool IsClientAreaExtendedToDecorations => _isClientAreaExtended;

	public Action<bool>? ExtendClientAreaToDecorationsChanged { get; set; }

	public bool NeedsManagedDecorations
	{
		get
		{
			if (_isClientAreaExtended)
			{
				return _extendChromeHints.HasAllFlags(ExtendClientAreaChromeHints.Default);
			}
			return false;
		}
	}

	public Thickness ExtendedMargins => _extendedMargins;

	public Thickness OffScreenMargin => _offScreenMargin;

	public AcrylicPlatformCompensationLevels AcrylicCompensationLevels { get; } = new AcrylicPlatformCompensationLevels(1.0, 0.8, 0.0);

	public INativeControlHostImpl NativeControlHost => _nativeControlHost;

	protected virtual bool ShouldTakeFocusOnClick => true;

	[UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "We do .NET COM interop availability checks")]
	[UnconditionalSuppressMessage("Trimming", "IL2050", Justification = "We do .NET COM interop availability checks")]
	protected unsafe virtual IntPtr AppWndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
	{
		uint timestamp = (uint)UnmanagedMethods.GetMessageTime();
		RawInputEventArgs rawInputEventArgs = null;
		bool flag = false;
		RawPointerEventType type = default(RawPointerEventType);
		switch (msg)
		{
		case 6u:
			switch ((UnmanagedMethods.WindowActivate)(ToInt32(wParam) & 0xFFFF))
			{
			case UnmanagedMethods.WindowActivate.WA_ACTIVE:
			case UnmanagedMethods.WindowActivate.WA_CLICKACTIVE:
				Activated?.Invoke();
				UpdateInputMethod(UnmanagedMethods.GetKeyboardLayout(0));
				break;
			case UnmanagedMethods.WindowActivate.WA_INACTIVE:
				Deactivated?.Invoke();
				break;
			}
			return IntPtr.Zero;
		case 131u:
			if ((ToInt32(wParam) == 1 && _windowProperties.Decorations == SystemDecorations.None) || _isClientAreaExtended)
			{
				return IntPtr.Zero;
			}
			break;
		case 16u:
			if (Closing?.Invoke(WindowCloseReason.WindowClosing) == true)
			{
				return IntPtr.Zero;
			}
			BeforeCloseCleanup(isDisposing: false);
			_isCloseRequested = true;
			break;
		case 2u:
			Closed?.Invoke();
			if (UiaCoreTypesApi.IsNetComInteropAvailable)
			{
				UiaCoreProviderApi.UiaReturnRawElementProvider(_hwnd, IntPtr.Zero, IntPtr.Zero, null);
			}
			if (Imm32InputMethod.Current.Hwnd == _hwnd)
			{
				Imm32InputMethod.Current.ClearLanguageAndWindow();
			}
			(_gl as IDisposable)?.Dispose();
			if (_dropTarget != null)
			{
				OleContext.Current?.UnregisterDragDrop(Handle);
				_dropTarget.Dispose();
				_dropTarget = null;
			}
			_framebuffer.Dispose();
			_hwnd = IntPtr.Zero;
			s_instances.Remove(this);
			_mouseDevice.Dispose();
			_touchDevice.Dispose();
			Dispose();
			Dispatcher.UIThread.Post(AfterCloseCleanup);
			return IntPtr.Zero;
		case 736u:
		{
			int num3 = ToInt32(wParam) & 0xFFFF;
			UnmanagedMethods.RECT rECT = Marshal.PtrToStructure<UnmanagedMethods.RECT>(lParam);
			_scaling = (double)num3 / 96.0;
			ScalingChanged?.Invoke(_scaling);
			using (SetResizeReason(WindowResizeReason.DpiChange))
			{
				UnmanagedMethods.SetWindowPos(hWnd, IntPtr.Zero, rECT.left, rECT.top, rECT.right - rECT.left, rECT.bottom - rECT.top, UnmanagedMethods.SetWindowPosFlags.SWP_NOACTIVATE | UnmanagedMethods.SetWindowPosFlags.SWP_NOZORDER);
			}
			return IntPtr.Zero;
		}
		case 256u:
		case 260u:
		{
			Key key2 = KeyInterop.KeyFromVirtualKey(ToInt32(wParam), ToInt32(lParam));
			if (key2 != 0)
			{
				rawInputEventArgs = new RawKeyEventArgs(WindowsKeyboardDevice.Instance, timestamp, Owner, RawKeyEventType.KeyDown, key2, WindowsKeyboardDevice.Instance.Modifiers);
			}
			break;
		}
		case 274u:
			if ((int)wParam == 61696 && HighWord(ToInt32(lParam)) <= 0)
			{
				return IntPtr.Zero;
			}
			break;
		case 288u:
			return (IntPtr)65536;
		case 257u:
		case 261u:
		{
			Key key = KeyInterop.KeyFromVirtualKey(ToInt32(wParam), ToInt32(lParam));
			if (key != 0)
			{
				rawInputEventArgs = new RawKeyEventArgs(WindowsKeyboardDevice.Instance, timestamp, Owner, RawKeyEventType.KeyUp, key, WindowsKeyboardDevice.Instance.Modifiers);
			}
			break;
		}
		case 258u:
			if (!Imm32InputMethod.Current.IsComposing && ToInt32(wParam) >= 32 && !_ignoreWmChar)
			{
				string text = new string((char)ToInt32(wParam), 1);
				rawInputEventArgs = new RawTextInputEventArgs(WindowsKeyboardDevice.Instance, timestamp, Owner, text);
			}
			break;
		case 513u:
		case 516u:
		case 519u:
		case 523u:
			if (IsMouseInPointerEnabled)
			{
				break;
			}
			flag = ShouldTakeFocusOnClick;
			if (!ShouldIgnoreTouchEmulatedMessage())
			{
				IInputDevice mouseDevice = _mouseDevice;
				ulong timestamp2 = timestamp;
				IInputRoot owner = Owner;
				switch (msg)
				{
				case 513u:
					type = RawPointerEventType.LeftButtonDown;
					break;
				case 516u:
					type = RawPointerEventType.RightButtonDown;
					break;
				case 519u:
					type = RawPointerEventType.MiddleButtonDown;
					break;
				case 523u:
					type = ((HighWord(ToInt32(wParam)) == 1) ? RawPointerEventType.XButton1Down : RawPointerEventType.XButton2Down);
					break;
				default:
					global::_003CPrivateImplementationDetails_003E.ThrowSwitchExpressionException((UnmanagedMethods.WindowsMessage)msg);
					break;
				}
				rawInputEventArgs = new RawPointerEventArgs(mouseDevice, timestamp2, owner, type, DipFromLParam(lParam), GetMouseModifiers(wParam));
			}
			break;
		case 514u:
		case 517u:
		case 520u:
		case 524u:
			if (!IsMouseInPointerEnabled && !ShouldIgnoreTouchEmulatedMessage())
			{
				IInputDevice mouseDevice = _mouseDevice;
				ulong timestamp2 = timestamp;
				IInputRoot owner = Owner;
				switch (msg)
				{
				case 514u:
					type = RawPointerEventType.LeftButtonUp;
					break;
				case 517u:
					type = RawPointerEventType.RightButtonUp;
					break;
				case 520u:
					type = RawPointerEventType.MiddleButtonUp;
					break;
				case 524u:
					type = ((HighWord(ToInt32(wParam)) == 1) ? RawPointerEventType.XButton1Up : RawPointerEventType.XButton2Up);
					break;
				default:
					global::_003CPrivateImplementationDetails_003E.ThrowSwitchExpressionException((UnmanagedMethods.WindowsMessage)msg);
					break;
				}
				rawInputEventArgs = new RawPointerEventArgs(mouseDevice, timestamp2, owner, type, DipFromLParam(lParam), GetMouseModifiers(wParam));
			}
			break;
		case 31u:
			if (!IsMouseInPointerEnabled)
			{
				_mouseDevice.Capture(null);
			}
			break;
		case 512u:
			if (!IsMouseInPointerEnabled && !ShouldIgnoreTouchEmulatedMessage())
			{
				if (!_trackingMouse)
				{
					UnmanagedMethods.TRACKMOUSEEVENT tRACKMOUSEEVENT = default(UnmanagedMethods.TRACKMOUSEEVENT);
					tRACKMOUSEEVENT.cbSize = Marshal.SizeOf<UnmanagedMethods.TRACKMOUSEEVENT>();
					tRACKMOUSEEVENT.dwFlags = 2u;
					tRACKMOUSEEVENT.hwndTrack = _hwnd;
					tRACKMOUSEEVENT.dwHoverTime = 0;
					UnmanagedMethods.TRACKMOUSEEVENT lpEventTrack = tRACKMOUSEEVENT;
					UnmanagedMethods.TrackMouseEvent(ref lpEventTrack);
				}
				Point position = DipFromLParam(lParam);
				UnmanagedMethods.POINT pOINT = default(UnmanagedMethods.POINT);
				pOINT.X = (int)(position.X * RenderScaling);
				pOINT.Y = (int)(position.Y * RenderScaling);
				UnmanagedMethods.POINT lpPoint = pOINT;
				UnmanagedMethods.ClientToScreen(_hwnd, ref lpPoint);
				UnmanagedMethods.MOUSEMOVEPOINT currPoint = new UnmanagedMethods.MOUSEMOVEPOINT
				{
					x = (lpPoint.X & 0xFFFF),
					y = (lpPoint.Y & 0xFFFF),
					time = (int)timestamp
				};
				UnmanagedMethods.MOUSEMOVEPOINT prevPoint = _lastWmMousePoint;
				_lastWmMousePoint = currPoint;
				rawInputEventArgs = new RawPointerEventArgs(_mouseDevice, timestamp, Owner, RawPointerEventType.Move, position, GetMouseModifiers(wParam))
				{
					IntermediatePoints = new Lazy<IReadOnlyList<RawPointerPoint>>(() => CreateIntermediatePoints(currPoint, prevPoint))
				};
			}
			break;
		case 522u:
			if (!IsMouseInPointerEnabled)
			{
				rawInputEventArgs = new RawMouseWheelEventArgs(_mouseDevice, timestamp, Owner, PointToClient(PointFromLParam(lParam)), new Vector(0.0, (double)(ToInt32(wParam) >> 16) / 120.0), GetMouseModifiers(wParam));
			}
			break;
		case 526u:
			if (!IsMouseInPointerEnabled)
			{
				rawInputEventArgs = new RawMouseWheelEventArgs(_mouseDevice, timestamp, Owner, PointToClient(PointFromLParam(lParam)), new Vector((double)(-(ToInt32(wParam) >> 16)) / 120.0, 0.0), GetMouseModifiers(wParam));
			}
			break;
		case 675u:
			if (!IsMouseInPointerEnabled)
			{
				_trackingMouse = false;
				rawInputEventArgs = new RawPointerEventArgs(_mouseDevice, timestamp, Owner, RawPointerEventType.LeaveWindow, new Point(-1.0, -1.0), WindowsKeyboardDevice.Instance.Modifiers);
			}
			break;
		case 161u:
		case 164u:
		case 167u:
		case 171u:
			if (!IsMouseInPointerEnabled)
			{
				IInputDevice mouseDevice = _mouseDevice;
				ulong timestamp2 = timestamp;
				IInputRoot owner = Owner;
				switch (msg)
				{
				case 161u:
					type = RawPointerEventType.NonClientLeftButtonDown;
					break;
				case 164u:
					type = RawPointerEventType.RightButtonDown;
					break;
				case 167u:
					type = RawPointerEventType.MiddleButtonDown;
					break;
				case 171u:
					type = ((HighWord(ToInt32(wParam)) == 1) ? RawPointerEventType.XButton1Down : RawPointerEventType.XButton2Down);
					break;
				default:
					global::_003CPrivateImplementationDetails_003E.ThrowSwitchExpressionException((UnmanagedMethods.WindowsMessage)msg);
					break;
				}
				rawInputEventArgs = new RawPointerEventArgs(mouseDevice, timestamp2, owner, type, PointToClient(PointFromLParam(lParam)), GetMouseModifiers(wParam));
			}
			break;
		case 576u:
		{
			if (_wmPointerEnabled)
			{
				break;
			}
			Action<RawInputEventArgs> input = Input;
			if (input == null)
			{
				break;
			}
			int num2 = wParam.ToInt32();
			UnmanagedMethods.TOUCHINPUT* ptr = stackalloc UnmanagedMethods.TOUCHINPUT[num2];
			Span<UnmanagedMethods.TOUCHINPUT> span = new Span<UnmanagedMethods.TOUCHINPUT>(ptr, num2);
			if (UnmanagedMethods.GetTouchInputInfo(lParam, (uint)num2, ptr, Marshal.SizeOf<UnmanagedMethods.TOUCHINPUT>()))
			{
				Span<UnmanagedMethods.TOUCHINPUT> span2 = span;
				for (int i = 0; i < span2.Length; i++)
				{
					UnmanagedMethods.TOUCHINPUT tOUCHINPUT = span2[i];
					input(new RawTouchEventArgs(_touchDevice, tOUCHINPUT.Time, Owner, tOUCHINPUT.Flags.HasAllFlags(UnmanagedMethods.TouchInputFlags.TOUCHEVENTF_UP) ? RawPointerEventType.TouchEnd : (tOUCHINPUT.Flags.HasAllFlags(UnmanagedMethods.TouchInputFlags.TOUCHEVENTF_DOWN) ? RawPointerEventType.TouchBegin : RawPointerEventType.TouchUpdate), PointToClient(new PixelPoint(tOUCHINPUT.X / 100, tOUCHINPUT.Y / 100)), WindowsKeyboardDevice.Instance.Modifiers, tOUCHINPUT.Id));
				}
				UnmanagedMethods.CloseTouchInputHandle(lParam);
				return IntPtr.Zero;
			}
			break;
		}
		case 578u:
		case 579u:
		case 581u:
		case 582u:
		case 583u:
			if (_wmPointerEnabled)
			{
				GetDevicePointerInfo(wParam, out IPointerDevice device4, out UnmanagedMethods.POINTER_INFO info4, out RawPointerPoint point4, out RawInputModifiers modifiers4, ref timestamp);
				RawPointerEventType eventType2 = GetEventType((UnmanagedMethods.WindowsMessage)msg, info4);
				RawPointerEventArgs rawPointerEventArgs = CreatePointerArgs(device4, timestamp, eventType2, point4, modifiers4, info4.pointerId);
				rawPointerEventArgs.IntermediatePoints = CreateLazyIntermediatePoints(info4);
				rawInputEventArgs = rawPointerEventArgs;
			}
			break;
		case 570u:
		case 586u:
		case 588u:
			if (_wmPointerEnabled)
			{
				GetDevicePointerInfo(wParam, out IPointerDevice device3, out UnmanagedMethods.POINTER_INFO info3, out RawPointerPoint point3, out RawInputModifiers modifiers3, ref timestamp);
				RawPointerEventType eventType = ((device3 is TouchDevice) ? RawPointerEventType.TouchCancel : RawPointerEventType.LeaveWindow);
				rawInputEventArgs = CreatePointerArgs(device3, timestamp, eventType, point3, modifiers3, info3.pointerId);
			}
			break;
		case 590u:
		case 591u:
			if (_wmPointerEnabled)
			{
				GetDevicePointerInfo(wParam, out IPointerDevice device2, out UnmanagedMethods.POINTER_INFO info2, out RawPointerPoint point2, out RawInputModifiers modifiers2, ref timestamp);
				double num = (double)(ToInt32(wParam) >> 16) / 120.0;
				Vector delta = ((msg == 590) ? new Vector(0.0, num) : new Vector(num, 0.0));
				rawInputEventArgs = new RawMouseWheelEventArgs(device2, timestamp, Owner, point2.Position, delta, modifiers2)
				{
					RawPointerId = info2.pointerId
				};
			}
			break;
		case 569u:
			if (_wmPointerEnabled)
			{
				GetDevicePointerInfo(wParam, out IPointerDevice device, out UnmanagedMethods.POINTER_INFO _, out RawPointerPoint _, out RawInputModifiers _, ref timestamp);
				if (device != _mouseDevice)
				{
					_mouseDevice.Capture(null);
					return IntPtr.Zero;
				}
			}
			break;
		case 133u:
			if (!HasFullDecorations)
			{
				return IntPtr.Zero;
			}
			break;
		case 134u:
			if (!HasFullDecorations)
			{
				return new IntPtr(1);
			}
			break;
		case 15u:
			using (NonPumpingSyncContext.Use(NonPumpingWaitHelperImpl.Instance))
			{
				if (UnmanagedMethods.BeginPaint(_hwnd, out var lpPaint) != IntPtr.Zero)
				{
					double renderScaling = RenderScaling;
					UnmanagedMethods.RECT rcPaint = lpPaint.rcPaint;
					Paint?.Invoke(new Rect((double)rcPaint.left / renderScaling, (double)rcPaint.top / renderScaling, (double)(rcPaint.right - rcPaint.left) / renderScaling, (double)(rcPaint.bottom - rcPaint.top) / renderScaling));
					UnmanagedMethods.EndPaint(_hwnd, ref lpPaint);
				}
			}
			return IntPtr.Zero;
		case 561u:
			_resizeReason = WindowResizeReason.User;
			break;
		case 5u:
		{
			UnmanagedMethods.SizeCommand sizeCommand = (UnmanagedMethods.SizeCommand)(int)wParam;
			if (Resized != null && (sizeCommand == UnmanagedMethods.SizeCommand.Restored || sizeCommand == UnmanagedMethods.SizeCommand.Maximized))
			{
				Size size = new Size(ToInt32(lParam) & 0xFFFF, ToInt32(lParam) >> 16);
				Resized(size / RenderScaling, _resizeReason);
			}
			WindowState windowState = sizeCommand switch
			{
				UnmanagedMethods.SizeCommand.Maximized => WindowState.Maximized, 
				UnmanagedMethods.SizeCommand.Minimized => WindowState.Minimized, 
				_ => _isFullScreenActive ? WindowState.FullScreen : WindowState.Normal, 
			};
			if (windowState != _lastWindowState)
			{
				_lastWindowState = windowState;
				WindowStateChanged?.Invoke(windowState);
				if (_isClientAreaExtended)
				{
					UpdateExtendMargins();
					ExtendClientAreaToDecorationsChanged?.Invoke(obj: true);
				}
			}
			return IntPtr.Zero;
		}
		case 562u:
			_resizeReason = WindowResizeReason.Unspecified;
			break;
		case 3u:
			PositionChanged?.Invoke(new PixelPoint((short)(ToInt32(lParam) & 0xFFFF), (short)(ToInt32(lParam) >> 16)));
			return IntPtr.Zero;
		case 36u:
		{
			UnmanagedMethods.MINMAXINFO structure = Marshal.PtrToStructure<UnmanagedMethods.MINMAXINFO>(lParam);
			_maxTrackSize = structure.ptMaxTrackSize;
			if (_minSize.Width > 0.0)
			{
				structure.ptMinTrackSize.X = (int)(_minSize.Width * RenderScaling + BorderThickness.Left + BorderThickness.Right);
			}
			if (_minSize.Height > 0.0)
			{
				structure.ptMinTrackSize.Y = (int)(_minSize.Height * RenderScaling + BorderThickness.Top + BorderThickness.Bottom);
			}
			if (!double.IsInfinity(_maxSize.Width) && _maxSize.Width > 0.0)
			{
				structure.ptMaxTrackSize.X = (int)(_maxSize.Width * RenderScaling + BorderThickness.Left + BorderThickness.Right);
			}
			if (!double.IsInfinity(_maxSize.Height) && _maxSize.Height > 0.0)
			{
				structure.ptMaxTrackSize.Y = (int)(_maxSize.Height * RenderScaling + BorderThickness.Top + BorderThickness.Bottom);
			}
			Marshal.StructureToPtr(structure, lParam, fDeleteOld: true);
			return IntPtr.Zero;
		}
		case 126u:
			(Screen as ScreenImpl)?.InvalidateScreensCache();
			return IntPtr.Zero;
		case 8u:
			LostFocus?.Invoke();
			break;
		case 81u:
			UpdateInputMethod(lParam);
			break;
		case 641u:
			UnmanagedMethods.DefWindowProc(Hwnd, msg, wParam, (nint)lParam & (nint)(~(nuint)2147483648u));
			UpdateInputMethod(UnmanagedMethods.GetKeyboardLayout(0));
			return IntPtr.Zero;
		case 271u:
			Imm32InputMethod.Current.HandleComposition(wParam, lParam, timestamp);
			break;
		case 269u:
			Imm32InputMethod.Current.HandleCompositionStart();
			return IntPtr.Zero;
		case 270u:
			Imm32InputMethod.Current.HandleCompositionEnd();
			return IntPtr.Zero;
		case 61u:
			if ((long)lParam == -25 && UiaCoreTypesApi.IsNetComInteropAvailable && _owner is Control element)
			{
				AutomationNode orCreate = AutomationNode.GetOrCreate(ControlAutomationPeer.CreatePeerForElement(element));
				return UiaCoreProviderApi.UiaReturnRawElementProvider(_hwnd, wParam, lParam, orCreate);
			}
			break;
		}
		if (flag)
		{
			UnmanagedMethods.SetFocus(_hwnd);
		}
		if (rawInputEventArgs != null && Input != null)
		{
			Input(rawInputEventArgs);
			if (msg == 256)
			{
				_ignoreWmChar = rawInputEventArgs.Handled;
			}
			if (s_intermediatePointsPooledList.Count > 0)
			{
				s_intermediatePointsPooledList.Dispose();
			}
			if (rawInputEventArgs.Handled)
			{
				return IntPtr.Zero;
			}
		}
		return UnmanagedMethods.DefWindowProc(hWnd, msg, wParam, lParam);
	}

	private Lazy<IReadOnlyList<RawPointerPoint>?>? CreateLazyIntermediatePoints(UnmanagedMethods.POINTER_INFO info)
	{
		int historyCount = Math.Min((int)info.historyCount, 512);
		if (historyCount > 1)
		{
			return new Lazy<IReadOnlyList<RawPointerPoint>>(delegate
			{
				s_intermediatePointsPooledList.Clear();
				s_intermediatePointsPooledList.Capacity = historyCount;
				if (info.pointerType == UnmanagedMethods.PointerInputType.PT_TOUCH)
				{
					if (s_historyTouchInfos == null)
					{
						s_historyTouchInfos = new UnmanagedMethods.POINTER_TOUCH_INFO[512];
					}
					if (UnmanagedMethods.GetPointerTouchInfoHistory(info.pointerId, ref historyCount, s_historyTouchInfos))
					{
						for (int num = historyCount - 1; num >= 1; num--)
						{
							UnmanagedMethods.POINTER_TOUCH_INFO info2 = s_historyTouchInfos[num];
							s_intermediatePointsPooledList.Add(CreateRawPointerPoint(info2));
						}
					}
				}
				else if (info.pointerType == UnmanagedMethods.PointerInputType.PT_PEN)
				{
					if (s_historyPenInfos == null)
					{
						s_historyPenInfos = new UnmanagedMethods.POINTER_PEN_INFO[512];
					}
					if (UnmanagedMethods.GetPointerPenInfoHistory(info.pointerId, ref historyCount, s_historyPenInfos))
					{
						for (int num2 = historyCount - 1; num2 >= 1; num2--)
						{
							UnmanagedMethods.POINTER_PEN_INFO info3 = s_historyPenInfos[num2];
							s_intermediatePointsPooledList.Add(CreateRawPointerPoint(info3));
						}
					}
				}
				else
				{
					if (s_historyInfos == null)
					{
						s_historyInfos = new UnmanagedMethods.POINTER_INFO[512];
					}
					if (UnmanagedMethods.GetPointerInfoHistory(info.pointerId, ref historyCount, s_historyInfos))
					{
						for (int num3 = historyCount - 1; num3 >= 1; num3--)
						{
							UnmanagedMethods.POINTER_INFO pointerInfo = s_historyInfos[num3];
							s_intermediatePointsPooledList.Add(CreateRawPointerPoint(pointerInfo));
						}
					}
				}
				return s_intermediatePointsPooledList;
			});
		}
		return null;
	}

	private unsafe IReadOnlyList<RawPointerPoint> CreateIntermediatePoints(UnmanagedMethods.MOUSEMOVEPOINT movePoint, UnmanagedMethods.MOUSEMOVEPOINT prevMovePoint)
	{
		if (s_mouseHistoryInfos == null)
		{
			s_mouseHistoryInfos = new UnmanagedMethods.MOUSEMOVEPOINT[64];
		}
		fixed (UnmanagedMethods.MOUSEMOVEPOINT* pointsBufferOut = s_mouseHistoryInfos)
		{
			UnmanagedMethods.MOUSEMOVEPOINT structure = movePoint;
			structure.time = 0;
			int mouseMovePointsEx = UnmanagedMethods.GetMouseMovePointsEx((uint)Marshal.SizeOf(structure), &structure, pointsBufferOut, s_mouseHistoryInfos.Length, 1u);
			if (mouseMovePointsEx <= 1)
			{
				return Array.Empty<RawPointerPoint>();
			}
			s_intermediatePointsPooledList.Clear();
			s_intermediatePointsPooledList.Capacity = mouseMovePointsEx;
			for (int num = mouseMovePointsEx - 1; num >= 1; num--)
			{
				UnmanagedMethods.MOUSEMOVEPOINT mOUSEMOVEPOINT = s_mouseHistoryInfos[num];
				if (mOUSEMOVEPOINT.time <= movePoint.time && (mOUSEMOVEPOINT.time != movePoint.time || mOUSEMOVEPOINT.x != movePoint.x || mOUSEMOVEPOINT.y != movePoint.y) && mOUSEMOVEPOINT.time >= prevMovePoint.time && (mOUSEMOVEPOINT.time != prevMovePoint.time || mOUSEMOVEPOINT.x != prevMovePoint.x || mOUSEMOVEPOINT.y != prevMovePoint.y))
				{
					if (mOUSEMOVEPOINT.x > 32767)
					{
						mOUSEMOVEPOINT.x -= 65536;
					}
					if (mOUSEMOVEPOINT.y > 32767)
					{
						mOUSEMOVEPOINT.y -= 65536;
					}
					Point position = PointToClient(new PixelPoint(mOUSEMOVEPOINT.x, mOUSEMOVEPOINT.y));
					s_intermediatePointsPooledList.Add(new RawPointerPoint
					{
						Position = position
					});
				}
			}
			return s_intermediatePointsPooledList;
		}
	}

	private RawPointerEventArgs CreatePointerArgs(IInputDevice device, ulong timestamp, RawPointerEventType eventType, RawPointerPoint point, RawInputModifiers modifiers, uint rawPointerId)
	{
		if (!(device is TouchDevice))
		{
			return new RawPointerEventArgs(device, timestamp, Owner, eventType, point, modifiers)
			{
				RawPointerId = rawPointerId
			};
		}
		return new RawTouchEventArgs(device, timestamp, Owner, eventType, point, modifiers, rawPointerId);
	}

	private void GetDevicePointerInfo(IntPtr wParam, out IPointerDevice device, out UnmanagedMethods.POINTER_INFO info, out RawPointerPoint point, out RawInputModifiers modifiers, ref uint timestamp)
	{
		uint pointerId = (uint)(ToInt32(wParam) & 0xFFFF);
		UnmanagedMethods.GetPointerType(pointerId, out var pointerType);
		modifiers = RawInputModifiers.None;
		switch (pointerType)
		{
		case UnmanagedMethods.PointerInputType.PT_PEN:
		{
			device = _penDevice;
			UnmanagedMethods.GetPointerPenInfo(pointerId, out var penInfo);
			info = penInfo.pointerInfo;
			point = CreateRawPointerPoint(penInfo);
			if (penInfo.penFlags.HasFlag(UnmanagedMethods.PenFlags.PEN_FLAGS_BARREL))
			{
				modifiers |= RawInputModifiers.PenBarrelButton;
			}
			if (penInfo.penFlags.HasFlag(UnmanagedMethods.PenFlags.PEN_FLAGS_ERASER))
			{
				modifiers |= RawInputModifiers.PenEraser;
			}
			if (penInfo.penFlags.HasFlag(UnmanagedMethods.PenFlags.PEN_FLAGS_INVERTED))
			{
				modifiers |= RawInputModifiers.PenInverted;
			}
			break;
		}
		case UnmanagedMethods.PointerInputType.PT_TOUCH:
		{
			device = _touchDevice;
			UnmanagedMethods.GetPointerTouchInfo(pointerId, out var touchInfo);
			info = touchInfo.pointerInfo;
			point = CreateRawPointerPoint(touchInfo);
			break;
		}
		default:
			device = _mouseDevice;
			UnmanagedMethods.GetPointerInfo(pointerId, out info);
			point = CreateRawPointerPoint(info);
			break;
		}
		if (info.dwTime != 0)
		{
			timestamp = info.dwTime;
		}
		modifiers |= GetInputModifiers(info.pointerFlags);
	}

	private RawPointerPoint CreateRawPointerPoint(UnmanagedMethods.POINTER_INFO pointerInfo)
	{
		Point position = PointToClient(new PixelPoint(pointerInfo.ptPixelLocationX, pointerInfo.ptPixelLocationY));
		RawPointerPoint result = new RawPointerPoint();
		result.Position = position;
		return result;
	}

	private RawPointerPoint CreateRawPointerPoint(UnmanagedMethods.POINTER_TOUCH_INFO info)
	{
		UnmanagedMethods.POINTER_INFO pointerInfo = info.pointerInfo;
		Point position = PointToClient(new PixelPoint(pointerInfo.ptPixelLocationX, pointerInfo.ptPixelLocationY));
		RawPointerPoint result = new RawPointerPoint();
		result.Position = position;
		result.Pressure = (float)info.pressure / 1024f;
		return result;
	}

	private RawPointerPoint CreateRawPointerPoint(UnmanagedMethods.POINTER_PEN_INFO info)
	{
		UnmanagedMethods.POINTER_INFO pointerInfo = info.pointerInfo;
		Point position = PointToClient(new PixelPoint(pointerInfo.ptPixelLocationX, pointerInfo.ptPixelLocationY));
		RawPointerPoint result = new RawPointerPoint();
		result.Position = position;
		result.Pressure = (float)info.pressure / 1024f;
		result.Twist = info.rotation;
		result.XTilt = info.tiltX;
		result.YTilt = info.tiltY;
		return result;
	}

	private static RawPointerEventType GetEventType(UnmanagedMethods.WindowsMessage message, UnmanagedMethods.POINTER_INFO info)
	{
		bool flag = info.pointerType == UnmanagedMethods.PointerInputType.PT_TOUCH;
		if (info.pointerFlags.HasFlag(UnmanagedMethods.PointerFlags.POINTER_FLAG_CANCELED))
		{
			if (!flag)
			{
				return RawPointerEventType.LeaveWindow;
			}
			return RawPointerEventType.TouchCancel;
		}
		RawPointerEventType rawPointerEventType = ToEventType(info.ButtonChangeType, flag);
		if (rawPointerEventType == RawPointerEventType.LeftButtonDown && message == UnmanagedMethods.WindowsMessage.WM_NCPOINTERDOWN)
		{
			rawPointerEventType = RawPointerEventType.NonClientLeftButtonDown;
		}
		return rawPointerEventType;
	}

	private static RawPointerEventType ToEventType(UnmanagedMethods.PointerButtonChangeType type, bool isTouch)
	{
		UnmanagedMethods.PointerButtonChangeType num = type - 1;
		if (num <= UnmanagedMethods.PointerButtonChangeType.POINTER_CHANGE_FIFTHBUTTON_DOWN)
		{
			switch (num)
			{
			case UnmanagedMethods.PointerButtonChangeType.POINTER_CHANGE_NONE:
				if (isTouch)
				{
					return RawPointerEventType.TouchBegin;
				}
				if (!isTouch)
				{
					return RawPointerEventType.LeftButtonDown;
				}
				break;
			case UnmanagedMethods.PointerButtonChangeType.POINTER_CHANGE_FIRSTBUTTON_UP:
				return RawPointerEventType.RightButtonDown;
			case UnmanagedMethods.PointerButtonChangeType.POINTER_CHANGE_SECONDBUTTON_UP:
				return RawPointerEventType.MiddleButtonDown;
			case UnmanagedMethods.PointerButtonChangeType.POINTER_CHANGE_THIRDBUTTON_UP:
				return RawPointerEventType.XButton1Down;
			case UnmanagedMethods.PointerButtonChangeType.POINTER_CHANGE_FOURTHBUTTON_UP:
				return RawPointerEventType.XButton2Down;
			case UnmanagedMethods.PointerButtonChangeType.POINTER_CHANGE_FIRSTBUTTON_DOWN:
				if (isTouch)
				{
					return RawPointerEventType.TouchEnd;
				}
				if (!isTouch)
				{
					return RawPointerEventType.LeftButtonUp;
				}
				break;
			case UnmanagedMethods.PointerButtonChangeType.POINTER_CHANGE_SECONDBUTTON_DOWN:
				return RawPointerEventType.RightButtonUp;
			case UnmanagedMethods.PointerButtonChangeType.POINTER_CHANGE_THIRDBUTTON_DOWN:
				return RawPointerEventType.MiddleButtonUp;
			case UnmanagedMethods.PointerButtonChangeType.POINTER_CHANGE_FOURTHBUTTON_DOWN:
				return RawPointerEventType.XButton1Up;
			case UnmanagedMethods.PointerButtonChangeType.POINTER_CHANGE_FIFTHBUTTON_DOWN:
				return RawPointerEventType.XButton2Up;
			}
		}
		if (isTouch)
		{
			return RawPointerEventType.TouchUpdate;
		}
		return RawPointerEventType.Move;
	}

	private void UpdateInputMethod(IntPtr hkl)
	{
		uint num = UnmanagedMethods.LGID(hkl);
		if (num != _langid || !(Imm32InputMethod.Current.Hwnd == Hwnd))
		{
			_langid = num;
			Imm32InputMethod.Current.SetLanguageAndWindow(this, Hwnd, hkl);
		}
	}

	private static int ToInt32(IntPtr ptr)
	{
		if (IntPtr.Size == 4)
		{
			return ptr.ToInt32();
		}
		return (int)(ptr.ToInt64() & 0xFFFFFFFFu);
	}

	private static int HighWord(int param)
	{
		return param >> 16;
	}

	private Point DipFromLParam(IntPtr lParam)
	{
		return new Point((short)(ToInt32(lParam) & 0xFFFF), (short)(ToInt32(lParam) >> 16)) / RenderScaling;
	}

	private static PixelPoint PointFromLParam(IntPtr lParam)
	{
		return new PixelPoint((short)(ToInt32(lParam) & 0xFFFF), (short)(ToInt32(lParam) >> 16));
	}

	private bool ShouldIgnoreTouchEmulatedMessage()
	{
		return (UnmanagedMethods.GetMessageExtraInfo().ToInt64() & 0xFF515700u) == 4283520768u;
	}

	private static RawInputModifiers GetMouseModifiers(IntPtr wParam)
	{
		return GetInputModifiers((UnmanagedMethods.ModifierKeys)ToInt32(wParam));
	}

	private static RawInputModifiers GetInputModifiers(UnmanagedMethods.ModifierKeys keys)
	{
		RawInputModifiers rawInputModifiers = WindowsKeyboardDevice.Instance.Modifiers;
		if (keys.HasAllFlags(UnmanagedMethods.ModifierKeys.MK_LBUTTON))
		{
			rawInputModifiers |= RawInputModifiers.LeftMouseButton;
		}
		if (keys.HasAllFlags(UnmanagedMethods.ModifierKeys.MK_RBUTTON))
		{
			rawInputModifiers |= RawInputModifiers.RightMouseButton;
		}
		if (keys.HasAllFlags(UnmanagedMethods.ModifierKeys.MK_MBUTTON))
		{
			rawInputModifiers |= RawInputModifiers.MiddleMouseButton;
		}
		if (keys.HasAllFlags(UnmanagedMethods.ModifierKeys.MK_ALT))
		{
			rawInputModifiers |= RawInputModifiers.XButton1MouseButton;
		}
		if (keys.HasAllFlags(UnmanagedMethods.ModifierKeys.MK_XBUTTON2))
		{
			rawInputModifiers |= RawInputModifiers.XButton2MouseButton;
		}
		return rawInputModifiers;
	}

	private static RawInputModifiers GetInputModifiers(UnmanagedMethods.PointerFlags flags)
	{
		RawInputModifiers rawInputModifiers = WindowsKeyboardDevice.Instance.Modifiers;
		if (flags.HasAllFlags(UnmanagedMethods.PointerFlags.POINTER_FLAG_FIRSTBUTTON))
		{
			rawInputModifiers |= RawInputModifiers.LeftMouseButton;
		}
		if (flags.HasAllFlags(UnmanagedMethods.PointerFlags.POINTER_FLAG_SECONDBUTTON))
		{
			rawInputModifiers |= RawInputModifiers.RightMouseButton;
		}
		if (flags.HasAllFlags(UnmanagedMethods.PointerFlags.POINTER_FLAG_THIRDBUTTON))
		{
			rawInputModifiers |= RawInputModifiers.MiddleMouseButton;
		}
		if (flags.HasAllFlags(UnmanagedMethods.PointerFlags.POINTER_FLAG_FOURTHBUTTON))
		{
			rawInputModifiers |= RawInputModifiers.XButton1MouseButton;
		}
		if (flags.HasAllFlags(UnmanagedMethods.PointerFlags.POINTER_FLAG_FIFTHBUTTON))
		{
			rawInputModifiers |= RawInputModifiers.XButton2MouseButton;
		}
		return rawInputModifiers;
	}

	public WindowImpl()
	{
		_touchDevice = new TouchDevice();
		_mouseDevice = new WindowsMouseDevice();
		_penDevice = new PenDevice();
		_windowProperties = new WindowProperties
		{
			ShowInTaskbar = false,
			IsResizable = true,
			Decorations = SystemDecorations.Full
		};
		IPlatformGraphics service = AvaloniaLocator.Current.GetService<IPlatformGraphics>();
		WinUiCompositorConnection service2 = AvaloniaLocator.Current.GetService<WinUiCompositorConnection>();
		bool flag = service is AngleWin32PlatformGraphics angleWin32PlatformGraphics && angleWin32PlatformGraphics.PlatformApi == AngleOptions.PlatformApi.DirectX11;
		_isUsingComposition = service2 != null && flag;
		DxgiConnection dxgiConnection = null;
		bool flag2 = false;
		if (!_isUsingComposition)
		{
			dxgiConnection = AvaloniaLocator.Current.GetService<DxgiConnection>();
			flag2 = dxgiConnection != null && flag;
		}
		_wmPointerEnabled = Win32Platform.WindowsVersion >= PlatformConstants.Windows8;
		CreateWindow();
		_framebuffer = new FramebufferManager(_hwnd);
		if (!(this is PopupImpl))
		{
			UpdateInputMethod(UnmanagedMethods.GetKeyboardLayout(0));
		}
		if (service != null)
		{
			if (_isUsingComposition)
			{
				_gl = (_blurHost = service2.CreateSurface(this));
			}
			else if (flag2)
			{
				DxgiSwapchainWindow gl = new DxgiSwapchainWindow(dxgiConnection, this);
				_gl = gl;
			}
			else if (service is AngleWin32PlatformGraphics)
			{
				_gl = new EglGlPlatformSurface(this);
			}
			else if (service is WglPlatformOpenGlInterface)
			{
				_gl = new WglGlPlatformSurface(this);
			}
		}
		Screen = new ScreenImpl();
		_storageProvider = new Win32StorageProvider(this);
		_nativeControlHost = new Win32NativeControlHost(this, _isUsingComposition);
		_transparencyLevel = (_isUsingComposition ? WindowTransparencyLevel.Transparent : WindowTransparencyLevel.None);
		s_instances.Add(this);
	}

	public object? TryGetFeature(Type featureType)
	{
		if (featureType == typeof(ITextInputMethodImpl))
		{
			return Imm32InputMethod.Current;
		}
		if (featureType == typeof(INativeControlHostImpl))
		{
			return _nativeControlHost;
		}
		if (featureType == typeof(IStorageProvider))
		{
			return _storageProvider;
		}
		if (featureType == typeof(IClipboard))
		{
			return AvaloniaLocator.Current.GetRequiredService<IClipboard>();
		}
		return null;
	}

	public void SetTransparencyLevelHint(IReadOnlyList<WindowTransparencyLevel> transparencyLevels)
	{
		Version windowsVersion = Win32Platform.WindowsVersion;
		foreach (WindowTransparencyLevel transparencyLevel in transparencyLevels)
		{
			if (!IsSupported(transparencyLevel, windowsVersion))
			{
				continue;
			}
			if (!(transparencyLevel == TransparencyLevel))
			{
				if (transparencyLevel == WindowTransparencyLevel.Transparent)
				{
					SetTransparencyTransparent(windowsVersion);
				}
				else if (transparencyLevel == WindowTransparencyLevel.Blur)
				{
					SetTransparencyBlur(windowsVersion);
				}
				else if (transparencyLevel == WindowTransparencyLevel.AcrylicBlur)
				{
					SetTransparencyAcrylicBlur(windowsVersion);
				}
				else if (transparencyLevel == WindowTransparencyLevel.Mica)
				{
					SetTransparencyMica(windowsVersion);
				}
				TransparencyLevel = transparencyLevel;
			}
			return;
		}
		if (_isUsingComposition)
		{
			SetTransparencyTransparent(windowsVersion);
			TransparencyLevel = WindowTransparencyLevel.Transparent;
		}
		else
		{
			TransparencyLevel = WindowTransparencyLevel.None;
		}
	}

	private bool IsSupported(WindowTransparencyLevel level, Version windowsVersion)
	{
		if (!_isUsingComposition)
		{
			return level == WindowTransparencyLevel.None;
		}
		if (level == WindowTransparencyLevel.None)
		{
			return false;
		}
		if (level == WindowTransparencyLevel.Transparent)
		{
			return windowsVersion >= PlatformConstants.Windows8;
		}
		if (level == WindowTransparencyLevel.Blur)
		{
			return windowsVersion < PlatformConstants.Windows10;
		}
		if (level == WindowTransparencyLevel.AcrylicBlur)
		{
			return windowsVersion >= WinUiCompositionShared.MinAcrylicVersion;
		}
		if (level == WindowTransparencyLevel.Mica)
		{
			return windowsVersion >= WinUiCompositionShared.MinHostBackdropVersion;
		}
		return false;
	}

	private void SetTransparencyTransparent(Version windowsVersion)
	{
		if (_isUsingComposition && !(windowsVersion < PlatformConstants.Windows8))
		{
			if (windowsVersion < PlatformConstants.Windows10)
			{
				SetAccentState(UnmanagedMethods.AccentState.ACCENT_ENABLE_BLURBEHIND);
				UnmanagedMethods.DWM_BLURBEHIND blurBehind = new UnmanagedMethods.DWM_BLURBEHIND(enabled: false);
				UnmanagedMethods.DwmEnableBlurBehindWindow(_hwnd, ref blurBehind);
			}
			SetUseHostBackdropBrush(useHostBackdropBrush: false);
			_blurHost?.SetBlur(BlurEffect.None);
		}
	}

	private void SetTransparencyBlur(Version windowsVersion)
	{
		if (_isUsingComposition && !(windowsVersion >= PlatformConstants.Windows10))
		{
			SetAccentState(UnmanagedMethods.AccentState.ACCENT_DISABLED);
			UnmanagedMethods.DWM_BLURBEHIND blurBehind = new UnmanagedMethods.DWM_BLURBEHIND(enabled: true);
			UnmanagedMethods.DwmEnableBlurBehindWindow(_hwnd, ref blurBehind);
		}
	}

	private void SetTransparencyAcrylicBlur(Version windowsVersion)
	{
		if (_isUsingComposition && !(windowsVersion < WinUiCompositionShared.MinAcrylicVersion))
		{
			SetUseHostBackdropBrush(useHostBackdropBrush: true);
			_blurHost?.SetBlur(BlurEffect.Acrylic);
		}
	}

	private void SetTransparencyMica(Version windowsVersion)
	{
		if (_isUsingComposition && !(windowsVersion < WinUiCompositionShared.MinHostBackdropVersion))
		{
			SetUseHostBackdropBrush(useHostBackdropBrush: false);
			_blurHost?.SetBlur(_currentThemeVariant switch
			{
				PlatformThemeVariant.Light => BlurEffect.MicaLight, 
				PlatformThemeVariant.Dark => BlurEffect.MicaDark, 
				_ => throw new ArgumentOutOfRangeException(), 
			});
		}
	}

	private void SetAccentState(UnmanagedMethods.AccentState state)
	{
		UnmanagedMethods.AccentPolicy structure = default(UnmanagedMethods.AccentPolicy);
		int num = Marshal.SizeOf(structure);
		structure.AccentState = state;
		IntPtr intPtr = Marshal.AllocHGlobal(num);
		Marshal.StructureToPtr(structure, intPtr, fDeleteOld: false);
		UnmanagedMethods.WindowCompositionAttributeData data = default(UnmanagedMethods.WindowCompositionAttributeData);
		data.Attribute = UnmanagedMethods.WindowCompositionAttribute.WCA_ACCENT_POLICY;
		data.SizeOfData = num;
		data.Data = intPtr;
		UnmanagedMethods.SetWindowCompositionAttribute(_hwnd, ref data);
		Marshal.FreeHGlobal(intPtr);
	}

	private unsafe void SetUseHostBackdropBrush(bool useHostBackdropBrush)
	{
		if (!(Win32Platform.WindowsVersion < WinUiCompositionShared.MinHostBackdropVersion))
		{
			int num = (useHostBackdropBrush ? 1 : 0);
			UnmanagedMethods.DwmSetWindowAttribute(_hwnd, 17, &num, 4);
		}
	}

	public void Move(PixelPoint point)
	{
		Position = point;
	}

	public void SetMinMaxSize(Size minSize, Size maxSize)
	{
		_minSize = minSize;
		_maxSize = maxSize;
	}

	public void Resize(Size value, WindowResizeReason reason)
	{
		if (WindowState != 0)
		{
			return;
		}
		int num = (int)(value.Width * RenderScaling);
		int num2 = (int)(value.Height * RenderScaling);
		UnmanagedMethods.GetClientRect(_hwnd, out var lpRect);
		if (num == lpRect.Width && num2 == lpRect.Height)
		{
			return;
		}
		UnmanagedMethods.GetWindowRect(_hwnd, out var lpRect2);
		using (SetResizeReason(reason))
		{
			UnmanagedMethods.SetWindowPos(_hwnd, IntPtr.Zero, 0, 0, num + ((!_isClientAreaExtended) ? (lpRect2.Width - lpRect.Width) : 0), num2 + ((!_isClientAreaExtended) ? (lpRect2.Height - lpRect.Height) : 0), UnmanagedMethods.SetWindowPosFlags.SWP_RESIZE);
		}
	}

	public void Activate()
	{
		UnmanagedMethods.SetForegroundWindow(_hwnd);
	}

	public IPopupImpl? CreatePopup()
	{
		if (!Win32Platform.UseOverlayPopups)
		{
			return new PopupImpl(this);
		}
		return null;
	}

	public void Dispose()
	{
		if (_hwnd != IntPtr.Zero)
		{
			if (!_isCloseRequested)
			{
				BeforeCloseCleanup(isDisposing: true);
			}
			UnmanagedMethods.DestroyWindow(_hwnd);
			_hwnd = IntPtr.Zero;
		}
	}

	public void Invalidate(Rect rect)
	{
		double renderScaling = RenderScaling;
		UnmanagedMethods.RECT rECT = default(UnmanagedMethods.RECT);
		rECT.left = (int)Math.Floor(rect.X * renderScaling);
		rECT.top = (int)Math.Floor(rect.Y * renderScaling);
		rECT.right = (int)Math.Ceiling(rect.Right * renderScaling);
		rECT.bottom = (int)Math.Ceiling(rect.Bottom * renderScaling);
		UnmanagedMethods.RECT lpRect = rECT;
		UnmanagedMethods.InvalidateRect(_hwnd, ref lpRect, bErase: false);
	}

	public Point PointToClient(PixelPoint point)
	{
		UnmanagedMethods.POINT pOINT = default(UnmanagedMethods.POINT);
		pOINT.X = point.X;
		pOINT.Y = point.Y;
		UnmanagedMethods.POINT lpPoint = pOINT;
		UnmanagedMethods.ScreenToClient(_hwnd, ref lpPoint);
		return new Point(lpPoint.X, lpPoint.Y) / RenderScaling;
	}

	public PixelPoint PointToScreen(Point point)
	{
		point *= RenderScaling;
		UnmanagedMethods.POINT pOINT = default(UnmanagedMethods.POINT);
		pOINT.X = (int)point.X;
		pOINT.Y = (int)point.Y;
		UnmanagedMethods.POINT lpPoint = pOINT;
		UnmanagedMethods.ClientToScreen(_hwnd, ref lpPoint);
		return new PixelPoint(lpPoint.X, lpPoint.Y);
	}

	public void SetInputRoot(IInputRoot inputRoot)
	{
		_owner = inputRoot;
		CreateDropTarget(inputRoot);
	}

	public void Hide()
	{
		UnmanagedMethods.ShowWindow(_hwnd, UnmanagedMethods.ShowWindowCommand.Hide);
		_shown = false;
	}

	public virtual void Show(bool activate, bool isDialog)
	{
		SetParent(_parent);
		ShowWindow(_showWindowState, activate);
	}

	public void SetParent(IWindowImpl? parent)
	{
		_parent = parent as WindowImpl;
		IntPtr intPtr = _parent?._hwnd ?? IntPtr.Zero;
		if (intPtr == IntPtr.Zero && !_windowProperties.ShowInTaskbar)
		{
			intPtr = OffscreenParentWindow.Handle;
			_hiddenWindowIsParent = true;
		}
		UnmanagedMethods.SetWindowLongPtr(_hwnd, -8, intPtr);
	}

	public void SetEnabled(bool enable)
	{
		UnmanagedMethods.EnableWindow(_hwnd, enable);
	}

	public void BeginMoveDrag(PointerPressedEventArgs e)
	{
		e.Pointer.Capture(null);
		UnmanagedMethods.DefWindowProc(_hwnd, 161u, new IntPtr(2), IntPtr.Zero);
	}

	public void BeginResizeDrag(WindowEdge edge, PointerPressedEventArgs e)
	{
		if (_windowProperties.IsResizable)
		{
			e.Pointer.Capture(null);
			UnmanagedMethods.DefWindowProc(_hwnd, 161u, new IntPtr((int)s_edgeLookup[edge]), IntPtr.Zero);
		}
	}

	public void SetTitle(string? title)
	{
		UnmanagedMethods.SetWindowText(_hwnd, title);
	}

	public void SetCursor(ICursorImpl? cursor)
	{
		IntPtr intPtr = (cursor as CursorImpl)?.Handle ?? s_defaultCursor;
		UnmanagedMethods.SetClassLong(_hwnd, UnmanagedMethods.ClassLongIndex.GCLP_HCURSOR, intPtr);
		if (Owner.IsPointerOver)
		{
			UnmanagedMethods.SetCursor(intPtr);
		}
	}

	public void SetIcon(IWindowIconImpl? icon)
	{
		IntPtr lParam = (icon as IconImpl)?.HIcon ?? IntPtr.Zero;
		UnmanagedMethods.PostMessage(_hwnd, 128u, new IntPtr(1), lParam);
	}

	public void ShowTaskbarIcon(bool value)
	{
		WindowProperties windowProperties = _windowProperties;
		windowProperties.ShowInTaskbar = value;
		UpdateWindowProperties(windowProperties);
	}

	public void CanResize(bool value)
	{
		WindowProperties windowProperties = _windowProperties;
		windowProperties.IsResizable = value;
		UpdateWindowProperties(windowProperties);
	}

	public void SetSystemDecorations(SystemDecorations value)
	{
		WindowProperties windowProperties = _windowProperties;
		windowProperties.Decorations = value;
		UpdateWindowProperties(windowProperties);
	}

	public void SetTopmost(bool value)
	{
		if (value != _topmost)
		{
			IntPtr hWndInsertAfter = (value ? UnmanagedMethods.WindowPosZOrder.HWND_TOPMOST : UnmanagedMethods.WindowPosZOrder.HWND_NOTOPMOST);
			UnmanagedMethods.SetWindowPos(_hwnd, hWndInsertAfter, 0, 0, 0, 0, UnmanagedMethods.SetWindowPosFlags.SWP_NOACTIVATE | UnmanagedMethods.SetWindowPosFlags.SWP_NOMOVE | UnmanagedMethods.SetWindowPosFlags.SWP_NOSIZE);
			_topmost = value;
		}
	}

	public unsafe void SetFrameThemeVariant(PlatformThemeVariant themeVariant)
	{
		_currentThemeVariant = themeVariant;
		if (Win32Platform.WindowsVersion.Build >= 22000)
		{
			int num = ((themeVariant == PlatformThemeVariant.Dark) ? 1 : 0);
			UnmanagedMethods.DwmSetWindowAttribute(_hwnd, 20, &num, 4);
			if (TransparencyLevel == WindowTransparencyLevel.Mica)
			{
				SetTransparencyMica(Win32Platform.WindowsVersion);
			}
		}
	}

	protected virtual IntPtr CreateWindowOverride(ushort atom)
	{
		return UnmanagedMethods.CreateWindowEx(_isUsingComposition ? 2097152 : 0, atom, null, 47120384u, int.MinValue, int.MinValue, int.MinValue, int.MinValue, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
	}

	[MemberNotNull("_wndProcDelegate")]
	[MemberNotNull("_className")]
	[MemberNotNull("Handle")]
	private void CreateWindow()
	{
		_wndProcDelegate = WndProc;
		_className = "Avalonia-" + Guid.NewGuid();
		UnmanagedMethods.WNDCLASSEX wNDCLASSEX = default(UnmanagedMethods.WNDCLASSEX);
		wNDCLASSEX.cbSize = Marshal.SizeOf<UnmanagedMethods.WNDCLASSEX>();
		wNDCLASSEX.style = 35;
		wNDCLASSEX.lpfnWndProc = _wndProcDelegate;
		wNDCLASSEX.hInstance = UnmanagedMethods.GetModuleHandle(null);
		wNDCLASSEX.hCursor = s_defaultCursor;
		wNDCLASSEX.hbrBackground = IntPtr.Zero;
		wNDCLASSEX.lpszClassName = _className;
		UnmanagedMethods.WNDCLASSEX lpwcx = wNDCLASSEX;
		ushort num = UnmanagedMethods.RegisterClassEx(ref lpwcx);
		if (num == 0)
		{
			throw new Win32Exception();
		}
		_hwnd = CreateWindowOverride(num);
		if (_hwnd == IntPtr.Zero)
		{
			throw new Win32Exception();
		}
		Handle = new WindowImplPlatformHandle(this);
		UnmanagedMethods.RegisterTouchWindow(_hwnd, 0);
		if (UnmanagedMethods.ShCoreAvailable && Win32Platform.WindowsVersion > PlatformConstants.Windows8 && UnmanagedMethods.GetDpiForMonitor(UnmanagedMethods.MonitorFromWindow(_hwnd, UnmanagedMethods.MONITOR.MONITOR_DEFAULTTONEAREST), UnmanagedMethods.MONITOR_DPI_TYPE.MDT_EFFECTIVE_DPI, out var dpiX, out var _) == 0L)
		{
			_scaling = (double)dpiX / 96.0;
		}
	}

	private void CreateDropTarget(IInputRoot inputRoot)
	{
		IDragDropDevice service = AvaloniaLocator.Current.GetService<IDragDropDevice>();
		if (service != null)
		{
			OleDropTarget oleDropTarget = new OleDropTarget(this, inputRoot, service);
			OleContext? current = OleContext.Current;
			if (current != null && current.RegisterDragDrop(Handle, oleDropTarget))
			{
				_dropTarget = oleDropTarget;
			}
		}
	}

	private void SetFullScreen(bool fullscreen)
	{
		if (fullscreen)
		{
			UnmanagedMethods.GetWindowRect(_hwnd, out var lpRect);
			UnmanagedMethods.GetClientRect(_hwnd, out var lpRect2);
			lpRect2.left += lpRect.left;
			lpRect2.right += lpRect.left;
			lpRect2.top += lpRect.top;
			lpRect2.bottom += lpRect.top;
			_savedWindowInfo.WindowRect = lpRect2;
			UnmanagedMethods.WindowStyles style = GetStyle();
			UnmanagedMethods.WindowStyles extendedStyle = GetExtendedStyle();
			_savedWindowInfo.Style = style;
			_savedWindowInfo.ExStyle = extendedStyle;
			SetStyle((UnmanagedMethods.WindowStyles)((uint)style & 0xFF3BFFFFu), save: false);
			SetExtendedStyle((UnmanagedMethods.WindowStyles)((uint)extendedStyle & 0xFFFDFCFEu), save: false);
			UnmanagedMethods.MONITORINFO lpmi = UnmanagedMethods.MONITORINFO.Create();
			UnmanagedMethods.GetMonitorInfo(UnmanagedMethods.MonitorFromWindow(_hwnd, UnmanagedMethods.MONITOR.MONITOR_DEFAULTTONEAREST), ref lpmi);
			PixelRect pixelRect = lpmi.rcMonitor.ToPixelRect();
			_isFullScreenActive = true;
			UnmanagedMethods.SetWindowPos(_hwnd, IntPtr.Zero, pixelRect.X, pixelRect.Y, pixelRect.Width, pixelRect.Height, UnmanagedMethods.SetWindowPosFlags.SWP_DRAWFRAME | UnmanagedMethods.SetWindowPosFlags.SWP_NOACTIVATE | UnmanagedMethods.SetWindowPosFlags.SWP_NOZORDER);
		}
		else
		{
			_isFullScreenActive = false;
			UnmanagedMethods.WindowStyles windowStateStyles = GetWindowStateStyles();
			SetStyle((UnmanagedMethods.WindowStyles)(((uint)_savedWindowInfo.Style & 0xDEFFFFFFu) | (uint)windowStateStyles), save: false);
			SetExtendedStyle(_savedWindowInfo.ExStyle, save: false);
			PixelRect pixelRect2 = _savedWindowInfo.WindowRect.ToPixelRect();
			UnmanagedMethods.SetWindowPos(_hwnd, IntPtr.Zero, pixelRect2.X, pixelRect2.Y, pixelRect2.Width, pixelRect2.Height, UnmanagedMethods.SetWindowPosFlags.SWP_DRAWFRAME | UnmanagedMethods.SetWindowPosFlags.SWP_NOACTIVATE | UnmanagedMethods.SetWindowPosFlags.SWP_NOZORDER);
			UpdateWindowProperties(_windowProperties, forceChanges: true);
		}
		TaskBarList.MarkFullscreen(_hwnd, fullscreen);
		ExtendClientArea();
	}

	private UnmanagedMethods.MARGINS UpdateExtendMargins()
	{
		UnmanagedMethods.RECT lpRect = default(UnmanagedMethods.RECT);
		UnmanagedMethods.RECT lpRect2 = default(UnmanagedMethods.RECT);
		UnmanagedMethods.AdjustWindowRectEx(ref lpRect2, (uint)GetStyle(), bMenu: false, 0u);
		UnmanagedMethods.AdjustWindowRectEx(ref lpRect, (uint)GetStyle() & 0xFF3FFFFFu, bMenu: false, 0u);
		lpRect.left *= -1;
		lpRect.top *= -1;
		lpRect2.left *= -1;
		lpRect2.top *= -1;
		if (!_extendChromeHints.HasAllFlags(ExtendClientAreaChromeHints.SystemChrome) && _extendTitleBarHint != -1.0)
		{
			lpRect2.top = 1;
		}
		int num = ((!_isUsingComposition) ? 1 : 0);
		UnmanagedMethods.MARGINS result = default(UnmanagedMethods.MARGINS);
		result.cxLeftWidth = num;
		result.cxRightWidth = num;
		result.cyBottomHeight = num;
		if (_extendTitleBarHint != -1.0)
		{
			lpRect2.top = (int)(_extendTitleBarHint * RenderScaling);
		}
		result.cyTopHeight = ((_extendChromeHints.HasAllFlags(ExtendClientAreaChromeHints.SystemChrome) && !_extendChromeHints.HasAllFlags(ExtendClientAreaChromeHints.Default)) ? lpRect2.top : num);
		if (WindowState == WindowState.Maximized)
		{
			_extendedMargins = new Thickness(0.0, (double)(lpRect2.top - lpRect.top) / RenderScaling, 0.0, 0.0);
			_offScreenMargin = new Thickness((double)lpRect.left / PrimaryScreenRenderScaling, (double)lpRect.top / PrimaryScreenRenderScaling, (double)lpRect.right / PrimaryScreenRenderScaling, (double)lpRect.bottom / PrimaryScreenRenderScaling);
		}
		else
		{
			_extendedMargins = new Thickness(0.0, (double)lpRect2.top / RenderScaling, 0.0, 0.0);
			_offScreenMargin = default(Thickness);
		}
		return result;
	}

	private unsafe void ExtendClientArea()
	{
		if (!_shown)
		{
			return;
		}
		if (UnmanagedMethods.DwmIsCompositionEnabled(out var enabled) < 0 || !enabled)
		{
			_isClientAreaExtended = false;
			return;
		}
		UnmanagedMethods.GetClientRect(_hwnd, out var lpRect);
		UnmanagedMethods.GetWindowRect(_hwnd, out var lpRect2);
		UnmanagedMethods.SetWindowPos(_hwnd, IntPtr.Zero, lpRect2.left, lpRect2.top, lpRect.Width, lpRect.Height, UnmanagedMethods.SetWindowPosFlags.SWP_DRAWFRAME | UnmanagedMethods.SetWindowPosFlags.SWP_NOACTIVATE);
		if (_isClientAreaExtended && WindowState != WindowState.FullScreen)
		{
			UnmanagedMethods.MARGINS margins = UpdateExtendMargins();
			UnmanagedMethods.DwmExtendFrameIntoClientArea(_hwnd, ref margins);
			int num = 2;
			UnmanagedMethods.DwmSetWindowAttribute(_hwnd, 33, &num, 4);
		}
		else
		{
			UnmanagedMethods.MARGINS margins2 = default(UnmanagedMethods.MARGINS);
			UnmanagedMethods.DwmExtendFrameIntoClientArea(_hwnd, ref margins2);
			_offScreenMargin = default(Thickness);
			_extendedMargins = default(Thickness);
			Resize(new Size((double)lpRect2.Width / RenderScaling, (double)lpRect2.Height / RenderScaling), WindowResizeReason.Layout);
			int num2 = 0;
			UnmanagedMethods.DwmSetWindowAttribute(_hwnd, 33, &num2, 4);
		}
		if (!_isClientAreaExtended || (_extendChromeHints.HasAllFlags(ExtendClientAreaChromeHints.SystemChrome) && !_extendChromeHints.HasAllFlags(ExtendClientAreaChromeHints.Default)))
		{
			EnableCloseButton(_hwnd);
		}
		else
		{
			DisableCloseButton(_hwnd);
		}
		ExtendClientAreaToDecorationsChanged?.Invoke(_isClientAreaExtended);
	}

	private void ShowWindow(WindowState state, bool activate)
	{
		_shown = true;
		if (_isClientAreaExtended)
		{
			ExtendClientArea();
		}
		WindowProperties windowProperties = _windowProperties;
		UnmanagedMethods.ShowWindowCommand? showWindowCommand;
		switch (state)
		{
		case WindowState.Minimized:
			windowProperties.IsFullScreen = false;
			showWindowCommand = UnmanagedMethods.ShowWindowCommand.Minimize;
			break;
		case WindowState.Maximized:
			windowProperties.IsFullScreen = false;
			showWindowCommand = UnmanagedMethods.ShowWindowCommand.Maximize;
			break;
		case WindowState.Normal:
			windowProperties.IsFullScreen = false;
			showWindowCommand = (UnmanagedMethods.IsWindowVisible(_hwnd) ? UnmanagedMethods.ShowWindowCommand.Restore : (activate ? UnmanagedMethods.ShowWindowCommand.Normal : UnmanagedMethods.ShowWindowCommand.ShowNoActivate));
			break;
		case WindowState.FullScreen:
			windowProperties.IsFullScreen = true;
			showWindowCommand = (UnmanagedMethods.IsWindowVisible(_hwnd) ? ((UnmanagedMethods.ShowWindowCommand?)null) : new UnmanagedMethods.ShowWindowCommand?(UnmanagedMethods.ShowWindowCommand.Restore));
			break;
		default:
			throw new ArgumentException("Invalid WindowState.");
		}
		UpdateWindowProperties(windowProperties);
		if (showWindowCommand.HasValue)
		{
			UnmanagedMethods.ShowWindow(_hwnd, showWindowCommand.Value);
		}
		if (state == WindowState.Maximized)
		{
			MaximizeWithoutCoveringTaskbar();
		}
		if (!Design.IsDesignMode && activate)
		{
			UnmanagedMethods.SetFocus(_hwnd);
			UnmanagedMethods.SetForegroundWindow(_hwnd);
		}
	}

	private void BeforeCloseCleanup(bool isDisposing)
	{
		if (_parent != null && UnmanagedMethods.IsWindow(_parent._hwnd))
		{
			bool num = UnmanagedMethods.GetActiveWindow() == _hwnd;
			if (!isDisposing)
			{
				_parent.SetEnabled(enable: true);
			}
			if (num)
			{
				UnmanagedMethods.SetActiveWindow(_parent._hwnd);
			}
		}
	}

	private void AfterCloseCleanup()
	{
		if (_className != null)
		{
			UnmanagedMethods.UnregisterClass(_className, UnmanagedMethods.GetModuleHandle(null));
			_className = null;
		}
	}

	private void MaximizeWithoutCoveringTaskbar()
	{
		IntPtr intPtr = UnmanagedMethods.MonitorFromWindow(_hwnd, UnmanagedMethods.MONITOR.MONITOR_DEFAULTTONEAREST);
		if (!(intPtr != IntPtr.Zero))
		{
			return;
		}
		UnmanagedMethods.MONITORINFO lpmi = UnmanagedMethods.MONITORINFO.Create();
		if (UnmanagedMethods.GetMonitorInfo(intPtr, ref lpmi))
		{
			int num = lpmi.rcWork.left;
			int top = lpmi.rcWork.top;
			int num2 = Math.Abs(lpmi.rcWork.right - num);
			int num3 = Math.Abs(lpmi.rcWork.bottom - top);
			UnmanagedMethods.WindowStyles windowLong = (UnmanagedMethods.WindowStyles)UnmanagedMethods.GetWindowLong(_hwnd, -16);
			if (!windowLong.HasFlag(UnmanagedMethods.WindowStyles.WS_SIZEFRAME))
			{
				Thickness borderThickness = BorderThickness;
				num -= (int)borderThickness.Left;
				num2 += (int)borderThickness.Left + (int)borderThickness.Right;
				num3 += (int)borderThickness.Bottom;
			}
			UnmanagedMethods.SetWindowPos(_hwnd, UnmanagedMethods.WindowPosZOrder.HWND_NOTOPMOST, num, top, num2, num3, UnmanagedMethods.SetWindowPosFlags.SWP_DRAWFRAME | UnmanagedMethods.SetWindowPosFlags.SWP_SHOWWINDOW);
		}
	}

	private UnmanagedMethods.WindowStyles GetWindowStateStyles()
	{
		return GetStyle() & (UnmanagedMethods.WindowStyles.WS_MAXIMIZE | UnmanagedMethods.WindowStyles.WS_MINIMIZE);
	}

	private UnmanagedMethods.WindowStyles GetStyle()
	{
		if (_isFullScreenActive)
		{
			return _savedWindowInfo.Style;
		}
		return (UnmanagedMethods.WindowStyles)UnmanagedMethods.GetWindowLong(_hwnd, -16);
	}

	private UnmanagedMethods.WindowStyles GetExtendedStyle()
	{
		if (_isFullScreenActive)
		{
			return _savedWindowInfo.ExStyle;
		}
		return (UnmanagedMethods.WindowStyles)UnmanagedMethods.GetWindowLong(_hwnd, -20);
	}

	private void SetStyle(UnmanagedMethods.WindowStyles style, bool save = true)
	{
		if (save)
		{
			_savedWindowInfo.Style = style;
		}
		if (!_isFullScreenActive)
		{
			UnmanagedMethods.SetWindowLong(_hwnd, -16, (uint)style);
		}
	}

	private void SetExtendedStyle(UnmanagedMethods.WindowStyles style, bool save = true)
	{
		if (save)
		{
			_savedWindowInfo.ExStyle = style;
		}
		if (!_isFullScreenActive)
		{
			UnmanagedMethods.SetWindowLong(_hwnd, -20, (uint)style);
		}
	}

	private void UpdateWindowProperties(WindowProperties newProperties, bool forceChanges = false)
	{
		WindowProperties windowProperties = _windowProperties;
		_windowProperties = newProperties;
		if (windowProperties.ShowInTaskbar != newProperties.ShowInTaskbar || forceChanges)
		{
			UnmanagedMethods.WindowStyles extendedStyle = GetExtendedStyle();
			if (newProperties.ShowInTaskbar)
			{
				extendedStyle |= UnmanagedMethods.WindowStyles.WS_SIZEFRAME;
				if (_hiddenWindowIsParent)
				{
					bool num = UnmanagedMethods.IsWindowVisible(_hwnd);
					bool activate = UnmanagedMethods.GetActiveWindow() == _hwnd;
					if (num)
					{
						Hide();
					}
					_hiddenWindowIsParent = false;
					SetParent(null);
					if (num)
					{
						Show(activate, isDialog: false);
					}
				}
			}
			else
			{
				if (_parent == null)
				{
					UnmanagedMethods.SetWindowLongPtr(_hwnd, -8, OffscreenParentWindow.Handle);
					_hiddenWindowIsParent = true;
				}
				extendedStyle = (UnmanagedMethods.WindowStyles)((uint)extendedStyle & 0xFFFBFFFFu);
			}
			SetExtendedStyle(extendedStyle);
		}
		UnmanagedMethods.WindowStyles style;
		if (windowProperties.IsResizable != newProperties.IsResizable || forceChanges)
		{
			style = GetStyle();
			if (newProperties.IsResizable)
			{
				style |= UnmanagedMethods.WindowStyles.WS_SIZEFRAME;
				style |= UnmanagedMethods.WindowStyles.WS_MAXIMIZEBOX;
			}
			else
			{
				style = (UnmanagedMethods.WindowStyles)((uint)style & 0xFFFBFFFFu);
				style = (UnmanagedMethods.WindowStyles)((uint)style & 0xFFFEFFFFu);
			}
			SetStyle(style);
		}
		if (windowProperties.IsFullScreen != newProperties.IsFullScreen)
		{
			SetFullScreen(newProperties.IsFullScreen);
		}
		if (!(windowProperties.Decorations != newProperties.Decorations || forceChanges))
		{
			return;
		}
		style = GetStyle();
		style = ((newProperties.Decorations != SystemDecorations.Full) ? ((UnmanagedMethods.WindowStyles)((uint)style & 0xFF37FFFFu)) : (style | (UnmanagedMethods.WindowStyles.WS_CAPTION | UnmanagedMethods.WindowStyles.WS_SYSMENU)));
		SetStyle(style);
		if (!_isFullScreenActive)
		{
			UnmanagedMethods.MARGINS mARGINS = default(UnmanagedMethods.MARGINS);
			mARGINS.cyBottomHeight = 0;
			mARGINS.cxRightWidth = 0;
			mARGINS.cxLeftWidth = 0;
			mARGINS.cyTopHeight = 0;
			UnmanagedMethods.MARGINS margins = mARGINS;
			UnmanagedMethods.DwmExtendFrameIntoClientArea(_hwnd, ref margins);
			UnmanagedMethods.GetClientRect(_hwnd, out var lpRect);
			UnmanagedMethods.POINT lpPoint = default(UnmanagedMethods.POINT);
			UnmanagedMethods.ClientToScreen(_hwnd, ref lpPoint);
			lpRect.Offset(lpPoint);
			UnmanagedMethods.RECT lpRect2 = lpRect;
			if (newProperties.Decorations == SystemDecorations.Full)
			{
				UnmanagedMethods.AdjustWindowRectEx(ref lpRect2, (uint)style, bMenu: false, (uint)GetExtendedStyle());
			}
			UnmanagedMethods.SetWindowPos(_hwnd, IntPtr.Zero, lpRect2.left, lpRect2.top, lpRect2.Width, lpRect2.Height, UnmanagedMethods.SetWindowPosFlags.SWP_DRAWFRAME | UnmanagedMethods.SetWindowPosFlags.SWP_NOACTIVATE | UnmanagedMethods.SetWindowPosFlags.SWP_NOZORDER);
		}
	}

	private static void DisableCloseButton(IntPtr hwnd)
	{
		UnmanagedMethods.EnableMenuItem(UnmanagedMethods.GetSystemMenu(hwnd, bRevert: false), 61536u, 3u);
	}

	private static void EnableCloseButton(IntPtr hwnd)
	{
		UnmanagedMethods.EnableMenuItem(UnmanagedMethods.GetSystemMenu(hwnd, bRevert: false), 61536u, 0u);
	}

	public void SetExtendClientAreaToDecorationsHint(bool hint)
	{
		_isClientAreaExtended = hint;
		ExtendClientArea();
	}

	public void SetExtendClientAreaChromeHints(ExtendClientAreaChromeHints hints)
	{
		_extendChromeHints = hints;
		ExtendClientArea();
	}

	public void SetExtendClientAreaTitleBarHeightHint(double titleBarHeight)
	{
		_extendTitleBarHint = titleBarHeight;
		ExtendClientArea();
	}

	private ResizeReasonScope SetResizeReason(WindowResizeReason reason)
	{
		WindowResizeReason resizeReason = _resizeReason;
		_resizeReason = reason;
		return new ResizeReasonScope(this, resizeReason);
	}

	private unsafe UnmanagedMethods.HitTestValues HitTestNCA(IntPtr hWnd, IntPtr wParam, IntPtr lParam)
	{
		PixelPoint pixelPoint = PointFromLParam(lParam);
		UnmanagedMethods.GetWindowRect(hWnd, out var lpRect);
		UnmanagedMethods.RECT lpRect2 = default(UnmanagedMethods.RECT);
		UnmanagedMethods.AdjustWindowRectEx(ref lpRect2, 983040u, bMenu: false, 0u);
		UnmanagedMethods.RECT lpRect3 = default(UnmanagedMethods.RECT);
		UnmanagedMethods.AdjustWindowRectEx(ref lpRect3, (uint)GetStyle(), bMenu: false, 0u);
		lpRect3.left *= -1;
		lpRect3.top *= -1;
		if (_extendTitleBarHint >= 0.0)
		{
			lpRect3.top = (int)(_extendedMargins.Top * RenderScaling);
		}
		ushort num = 1;
		ushort num2 = 1;
		bool flag = false;
		if (pixelPoint.X >= lpRect.left && pixelPoint.X < lpRect.left + lpRect3.left)
		{
			num2 = 0;
		}
		else if (pixelPoint.X < lpRect.right && pixelPoint.X >= lpRect.right - lpRect3.right)
		{
			num2 = 2;
		}
		if (pixelPoint.Y >= lpRect.top && pixelPoint.Y < lpRect.top + lpRect3.top)
		{
			flag = pixelPoint.Y < lpRect.top - lpRect2.top;
			if (flag || num2 == 1)
			{
				num = 0;
			}
		}
		else if (pixelPoint.Y < lpRect.bottom && pixelPoint.Y >= lpRect.bottom - lpRect3.bottom)
		{
			num = 2;
		}
		byte* num3 = stackalloc byte[36];
		*(int*)num3 = 13;
		*(int*)(num3 + sizeof(UnmanagedMethods.HitTestValues)) = (flag ? 12 : 2);
		*(int*)(num3 + (nint)2 * (nint)sizeof(UnmanagedMethods.HitTestValues)) = 14;
		*(int*)(num3 + (nint)3 * (nint)sizeof(UnmanagedMethods.HitTestValues)) = 10;
		*(int*)(num3 + (nint)4 * (nint)sizeof(UnmanagedMethods.HitTestValues)) = 0;
		*(int*)(num3 + (nint)5 * (nint)sizeof(UnmanagedMethods.HitTestValues)) = 11;
		*(int*)(num3 + (nint)6 * (nint)sizeof(UnmanagedMethods.HitTestValues)) = 16;
		*(int*)(num3 + (nint)7 * (nint)sizeof(UnmanagedMethods.HitTestValues)) = 15;
		*(int*)(num3 + (nint)8 * (nint)sizeof(UnmanagedMethods.HitTestValues)) = 17;
		ReadOnlySpan<UnmanagedMethods.HitTestValues> readOnlySpan = new Span<UnmanagedMethods.HitTestValues>(num3, 9);
		int index = num * 3 + num2;
		return readOnlySpan[index];
	}

	protected virtual IntPtr CustomCaptionProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam, ref bool callDwp)
	{
		IntPtr plResult = IntPtr.Zero;
		callDwp = !UnmanagedMethods.DwmDefWindowProc(hWnd, msg, wParam, lParam, ref plResult);
		switch ((UnmanagedMethods.WindowsMessage)msg)
		{
		case UnmanagedMethods.WindowsMessage.WM_NCHITTEST:
		{
			if (!(plResult == IntPtr.Zero))
			{
				break;
			}
			if (WindowState == WindowState.FullScreen)
			{
				return (IntPtr)1;
			}
			UnmanagedMethods.HitTestValues hitTestValues = HitTestNCA(hWnd, wParam, lParam);
			plResult = (IntPtr)(int)hitTestValues;
			if (hitTestValues == UnmanagedMethods.HitTestValues.HTCAPTION)
			{
				Point p = PointToClient(PointFromLParam(lParam));
				if (_owner is Window visual && visual.GetVisualAt(p, (Visual x) => (!(x is IInputElement inputElement) || (inputElement.IsHitTestVisible && inputElement.IsEffectivelyVisible)) ? true : false) != null)
				{
					hitTestValues = UnmanagedMethods.HitTestValues.HTCLIENT;
					plResult = (IntPtr)(int)hitTestValues;
				}
			}
			if (hitTestValues != 0)
			{
				callDwp = false;
			}
			break;
		}
		}
		return plResult;
	}

	protected virtual IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
	{
		IntPtr result = IntPtr.Zero;
		bool callDwp = true;
		if (_isClientAreaExtended)
		{
			result = CustomCaptionProc(hWnd, msg, wParam, lParam, ref callDwp);
		}
		if (callDwp)
		{
			result = AppWndProc(hWnd, msg, wParam, lParam);
		}
		return result;
	}
}
