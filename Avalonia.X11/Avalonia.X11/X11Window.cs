using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Platform;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.FreeDesktop;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Input.Raw;
using Avalonia.Input.TextInput;
using Avalonia.OpenGL.Egl;
using Avalonia.Platform;
using Avalonia.Platform.Interop;
using Avalonia.Platform.Storage;
using Avalonia.Rendering.Composition;
using Avalonia.Threading;
using Avalonia.X11.Glx;
using Avalonia.X11.NativeDialogs;

namespace Avalonia.X11;

internal class X11Window : IWindowImpl, IWindowBaseImpl, ITopLevelImpl, IOptionalFeatureProvider, IDisposable, IPopupImpl, IXI2Client
{
	private enum XSyncState
	{
		None,
		WaitConfigure,
		WaitPaint
	}

	private class SurfaceInfo : EglGlPlatformSurface.IEglWindowGlPlatformSurfaceInfo
	{
		private readonly X11Window _window;

		private readonly IntPtr _display;

		private readonly IntPtr _parent;

		public IntPtr Handle { get; }

		public PixelSize Size
		{
			get
			{
				XLib.XLockDisplay(_display);
				XLib.XGetGeometry(_display, _parent, out var geo);
				XLib.XResizeWindow(_display, Handle, geo.width, geo.height);
				XLib.XUnlockDisplay(_display);
				return new PixelSize(geo.width, geo.height);
			}
		}

		public double Scaling => _window.RenderScaling;

		public SurfaceInfo(X11Window window, IntPtr display, IntPtr parent, IntPtr xid)
		{
			_window = window;
			_display = display;
			_parent = parent;
			Handle = xid;
		}
	}

	public class SurfacePlatformHandle : INativePlatformHandleSurface, IPlatformHandle
	{
		private readonly X11Window _owner;

		public PixelSize Size => _owner.ToPixelSize(_owner.ClientSize);

		public double Scaling => _owner.RenderScaling;

		public IntPtr Handle => _owner._renderHandle;

		public string HandleDescriptor => "XID";

		public SurfacePlatformHandle(X11Window owner)
		{
			_owner = owner;
		}
	}

	private class RawKeyEventArgsWithText : RawKeyEventArgs
	{
		public string Text { get; }

		public RawKeyEventArgsWithText(IKeyboardDevice device, ulong timestamp, IInputRoot root, RawKeyEventType type, Key key, RawInputModifiers modifiers, string text)
			: base(device, timestamp, root, type, key, modifiers)
		{
			Text = text;
		}
	}

	private class XimInputMethod : ITextInputMethodImpl, IX11InputMethodControl, IDisposable
	{
		private readonly X11Window _parent;

		private bool _windowActive;

		private bool _imeActive;

		private Rect? _queuedCursorRect;

		private TextInputMethodClient? _client;

		public TextInputMethodClient? Client => _client;

		public bool IsActive => _client != null;

		public bool IsEnabled => false;

		public event Action<string> Commit
		{
			add
			{
			}
			remove
			{
			}
		}

		public event Action<X11InputMethodForwardedKey> ForwardKey
		{
			add
			{
			}
			remove
			{
			}
		}

		public XimInputMethod(X11Window parent)
		{
			_parent = parent;
		}

		public void SetCursorRect(Rect rect)
		{
			bool num = !_queuedCursorRect.HasValue;
			_queuedCursorRect = rect;
			if (!num)
			{
				return;
			}
			Dispatcher.UIThread.Post(delegate
			{
				if (_queuedCursorRect.HasValue)
				{
					_ = _queuedCursorRect.Value;
					_queuedCursorRect = null;
					if (!(_parent._xic == IntPtr.Zero))
					{
						rect *= _parent._scaling;
						XPoint xPoint = default(XPoint);
						xPoint.X = (short)Math.Min(Math.Max(rect.X, -32768.0), 32767.0);
						xPoint.Y = (short)Math.Min(Math.Max(rect.Y + rect.Height, -32768.0), 32767.0);
						XPoint point = xPoint;
						using Utf8Buffer name = new Utf8Buffer("spotLocation");
						IntPtr data = XLib.XVaCreateNestedList(0, name, ref point, IntPtr.Zero);
						XLib.XSetICValues(_parent._xic, "preeditAttributes", data, IntPtr.Zero);
						XLib.XFree(data);
					}
				}
			}, DispatcherPriority.Background);
		}

		public void SetWindowActive(bool active)
		{
			_windowActive = active;
			UpdateActive();
		}

		public void SetClient(TextInputMethodClient client)
		{
			_client = client;
			UpdateActive();
		}

		private void UpdateActive()
		{
			bool flag = _windowActive && IsActive;
			if (!(_parent._xic == IntPtr.Zero) && flag != _imeActive)
			{
				_imeActive = flag;
				if (flag)
				{
					Reset();
					XLib.XSetICFocus(_parent._xic);
				}
				else
				{
					XLib.XUnsetICFocus(_parent._xic);
				}
			}
		}

		public void UpdateWindowInfo(PixelPoint position, double scaling)
		{
		}

		public void SetOptions(TextInputOptions options)
		{
		}

		public void Reset()
		{
			if (!(_parent._xic == IntPtr.Zero))
			{
				IntPtr intPtr = XLib.XmbResetIC(_parent._xic);
				if (intPtr != IntPtr.Zero)
				{
					XLib.XFree(intPtr);
				}
			}
		}

		public void Dispose()
		{
		}

		public ValueTask<bool> HandleEventAsync(RawKeyEventArgs args, int keyVal, int keyCode)
		{
			return new ValueTask<bool>(result: false);
		}
	}

	private readonly AvaloniaX11Platform _platform;

	private readonly bool _popup;

	private readonly X11Info _x11;

	private XConfigureEvent? _configure;

	private PixelPoint? _configurePoint;

	private bool _triggeredExpose;

	private IInputRoot? _inputRoot;

	private readonly MouseDevice _mouse;

	private readonly TouchDevice _touch;

	private readonly IKeyboardDevice _keyboard;

	private readonly ITopLevelNativeMenuExporter? _nativeMenuExporter;

	private readonly IStorageProvider _storageProvider;

	private readonly X11NativeControlHost _nativeControlHost;

	private PixelPoint? _position;

	private PixelSize _realSize;

	private bool _cleaningUp;

	private IntPtr _handle;

	private IntPtr _xic;

	private IntPtr _renderHandle;

	private IntPtr _xSyncCounter;

	private XLib.XSyncValue _xSyncValue;

	private XSyncState _xSyncState;

	private bool _mapped;

	private bool _wasMappedAtLeastOnce;

	private double? _scalingOverride;

	private bool _disabled;

	private TransparencyHelper? _transparencyHelper;

	private RawEventGrouper? _rawEventGrouper;

	private bool _useRenderWindow;

	private bool _usePositioningFlags;

	private WindowState _lastWindowState;

	private SystemDecorations _systemDecorations = SystemDecorations.Full;

	private bool _canResize = true;

	private const int MaxWindowDimension = 100000;

	private (Size minSize, Size maxSize) _scaledMinMaxSize = (minSize: new Size(1.0, 1.0), maxSize: new Size(double.PositiveInfinity, double.PositiveInfinity));

	private (PixelSize minSize, PixelSize maxSize) _minMaxSize = (minSize: new PixelSize(1, 1), maxSize: new PixelSize(100000, 100000));

	private double _scaling = 1.0;

	private ITextInputMethodImpl _ime;

	private IX11InputMethodControl _imeControl;

	private bool _processingIme;

	private Queue<(RawKeyEventArgs args, XEvent xev, int keyval, int keycode)> _imeQueue = new Queue<(RawKeyEventArgs, XEvent, int, int)>();

	private const int ImeBufferSize = 65536;

	[ThreadStatic]
	private static IntPtr ImeBuffer;

	public Size ClientSize => new Size((double)_realSize.Width / RenderScaling, (double)_realSize.Height / RenderScaling);

	public Size? FrameSize
	{
		get
		{
			Thickness? frameExtents = GetFrameExtents();
			if (!frameExtents.HasValue)
			{
				return null;
			}
			return new Size(((double)_realSize.Width + frameExtents.Value.Left + frameExtents.Value.Right) / RenderScaling, ((double)_realSize.Height + frameExtents.Value.Top + frameExtents.Value.Bottom) / RenderScaling);
		}
	}

	public double RenderScaling
	{
		get
		{
			return Interlocked.CompareExchange(ref _scaling, 0.0, 0.0);
		}
		private set
		{
			Interlocked.Exchange(ref _scaling, value);
		}
	}

	public double DesktopScaling => RenderScaling;

	public IEnumerable<object> Surfaces { get; }

	public Action<RawInputEventArgs>? Input { get; set; }

	public Action<Rect>? Paint { get; set; }

	public Action<Size, WindowResizeReason>? Resized { get; set; }

	public Action<double>? ScalingChanged { get; set; }

	public Action? Deactivated { get; set; }

	public Action? Activated { get; set; }

	public Func<WindowCloseReason, bool>? Closing { get; set; }

	public Action<WindowState>? WindowStateChanged { get; set; }

	public Action<WindowTransparencyLevel>? TransparencyLevelChanged
	{
		get
		{
			return _transparencyHelper?.TransparencyLevelChanged;
		}
		set
		{
			if (_transparencyHelper != null)
			{
				_transparencyHelper.TransparencyLevelChanged = value;
			}
		}
	}

	public Action<bool>? ExtendClientAreaToDecorationsChanged { get; set; }

	public Thickness ExtendedMargins { get; }

	public Thickness OffScreenMargin { get; }

	public bool IsClientAreaExtendedToDecorations { get; }

	public Action? Closed { get; set; }

	public Action<PixelPoint>? PositionChanged { get; set; }

	public Action? LostFocus { get; set; }

	public Compositor Compositor => _platform.Compositor;

	public WindowState WindowState
	{
		get
		{
			return _lastWindowState;
		}
		set
		{
			if (_lastWindowState != value)
			{
				_lastWindowState = value;
				switch (value)
				{
				case WindowState.Minimized:
					XLib.XIconifyWindow(_x11.Display, _handle, _x11.DefaultScreen);
					break;
				case WindowState.Maximized:
					ChangeWMAtoms(false, _x11.Atoms._NET_WM_STATE_HIDDEN);
					ChangeWMAtoms(false, _x11.Atoms._NET_WM_STATE_FULLSCREEN);
					ChangeWMAtoms(true, _x11.Atoms._NET_WM_STATE_MAXIMIZED_VERT, _x11.Atoms._NET_WM_STATE_MAXIMIZED_HORZ);
					break;
				case WindowState.FullScreen:
					ChangeWMAtoms(false, _x11.Atoms._NET_WM_STATE_HIDDEN);
					ChangeWMAtoms(true, _x11.Atoms._NET_WM_STATE_FULLSCREEN);
					ChangeWMAtoms(false, _x11.Atoms._NET_WM_STATE_MAXIMIZED_VERT, _x11.Atoms._NET_WM_STATE_MAXIMIZED_HORZ);
					break;
				default:
					ChangeWMAtoms(false, _x11.Atoms._NET_WM_STATE_HIDDEN);
					ChangeWMAtoms(false, _x11.Atoms._NET_WM_STATE_FULLSCREEN);
					ChangeWMAtoms(false, _x11.Atoms._NET_WM_STATE_MAXIMIZED_VERT, _x11.Atoms._NET_WM_STATE_MAXIMIZED_HORZ);
					SendNetWMMessage(_x11.Atoms._NET_ACTIVE_WINDOW, (IntPtr)1, _x11.LastActivityTimestamp, IntPtr.Zero, null, null);
					break;
				}
			}
		}
	}

	public IInputRoot? InputRoot => _inputRoot;

	public IPlatformHandle Handle { get; }

	public PixelPoint Position
	{
		get
		{
			if (!_position.HasValue)
			{
				return default(PixelPoint);
			}
			Thickness? thickness = GetFrameExtents();
			if (!thickness.HasValue)
			{
				thickness = default(Thickness);
			}
			return new PixelPoint(_position.Value.X - (int)thickness.Value.Left, _position.Value.Y - (int)thickness.Value.Top);
		}
		set
		{
			if (!_usePositioningFlags)
			{
				_usePositioningFlags = true;
				UpdateSizeHints(null);
			}
			XWindowChanges xWindowChanges = default(XWindowChanges);
			xWindowChanges.x = value.X;
			xWindowChanges.y = value.Y;
			XWindowChanges values = xWindowChanges;
			XLib.XConfigureWindow(_x11.Display, _handle, ChangeWindowFlags.CWX | ChangeWindowFlags.CWY, ref values);
			XLib.XFlush(_x11.Display);
			if (!_wasMappedAtLeastOnce)
			{
				_position = value;
				PositionChanged?.Invoke(value);
			}
		}
	}

	public IMouseDevice MouseDevice => _mouse;

	public TouchDevice TouchDevice => _touch;

	public IScreenImpl Screen => _platform.Screens;

	public Size MaxAutoSizeHint => (from s in _platform.X11Screens.Screens
		select s.Bounds.Size.ToSize(s.Scaling) into x
		orderby x.Width + x.Height descending
		select x).FirstOrDefault();

	public Action? GotInputWhenDisabled { get; set; }

	public IPopupPositioner? PopupPositioner { get; }

	public WindowTransparencyLevel TransparencyLevel => _transparencyHelper?.CurrentLevel ?? WindowTransparencyLevel.None;

	public AcrylicPlatformCompensationLevels AcrylicCompensationLevels { get; } = new AcrylicPlatformCompensationLevels(1.0, 0.8, 0.8);

	public bool NeedsManagedDecorations => false;

	public bool IsEnabled => !_disabled;

	public unsafe X11Window(AvaloniaX11Platform platform, IWindowImpl? popupParent)
	{
		_platform = platform;
		_popup = popupParent != null;
		_x11 = platform.Info;
		_mouse = new MouseDevice();
		_touch = new TouchDevice();
		_keyboard = platform.KeyboardDevice;
		IPlatformGraphics service = AvaloniaLocator.Current.GetService<IPlatformGraphics>();
		XSetWindowAttributes attributes = default(XSetWindowAttributes);
		SetWindowValuemask setWindowValuemask = SetWindowValuemask.Nothing;
		attributes.backing_store = 1;
		attributes.bit_gravity = Gravity.NorthWestGravity;
		attributes.win_gravity = Gravity.NorthWestGravity;
		setWindowValuemask |= SetWindowValuemask.BackPixmap | SetWindowValuemask.BackPixel | SetWindowValuemask.BorderPixel | SetWindowValuemask.BitGravity | SetWindowValuemask.WinGravity | SetWindowValuemask.BackingStore;
		if (_popup)
		{
			attributes.override_redirect = 1;
			setWindowValuemask |= SetWindowValuemask.OverrideRedirect;
		}
		XVisualInfo? xVisualInfo = null;
		_useRenderWindow = service != null;
		GlxPlatformGraphics glxPlatformGraphics = service as GlxPlatformGraphics;
		if (glxPlatformGraphics != null)
		{
			xVisualInfo = *glxPlatformGraphics.Display.VisualInfo;
		}
		else if (service == null)
		{
			xVisualInfo = _x11.TransparentVisualInfo;
		}
		EglPlatformGraphics eglPlatformGraphics = service as EglPlatformGraphics;
		IntPtr visual = IntPtr.Zero;
		int depth = 24;
		if (xVisualInfo.HasValue)
		{
			visual = xVisualInfo.Value.visual;
			depth = (int)xVisualInfo.Value.depth;
			attributes.colormap = XLib.XCreateColormap(_x11.Display, _x11.RootWindow, xVisualInfo.Value.visual, 0);
			setWindowValuemask |= SetWindowValuemask.ColorMap;
		}
		int val = 0;
		int val2 = 0;
		if (!_popup && Screen != null)
		{
			Screen screen = Screen.AllScreens.OrderBy((Screen x) => x.Scaling).FirstOrDefault((Screen m) => m.Bounds.Contains(_position.GetValueOrDefault()));
			if (screen != null)
			{
				val = (int)((double)screen.WorkingArea.Width * 0.75);
				val2 = (int)((double)screen.WorkingArea.Height * 0.7);
			}
		}
		val = Math.Max(val, 300);
		val2 = Math.Max(val2, 200);
		_handle = XLib.XCreateWindow(_x11.Display, _x11.RootWindow, 10, 10, val, val2, 0, depth, 1, visual, new UIntPtr((uint)setWindowValuemask), ref attributes);
		if (_useRenderWindow)
		{
			_renderHandle = XLib.XCreateWindow(_x11.Display, _handle, 0, 0, val, val2, 0, depth, 1, visual, new UIntPtr(120u), ref attributes);
		}
		else
		{
			_renderHandle = _handle;
		}
		Handle = new SurfacePlatformHandle(this);
		_realSize = new PixelSize(val, val2);
		platform.Windows[_handle] = OnEvent;
		XEventMask xEventMask = XEventMask.PointerMotionHintMask | XEventMask.ResizeRedirectMask | XEventMask.SubstructureRedirectMask;
		if (platform.XI2 != null)
		{
			xEventMask |= platform.XI2.AddWindow(_handle, this);
		}
		XLib.XSelectInput(mask: new IntPtr((int)((XEventMask.KeyPressMask | XEventMask.KeyReleaseMask | XEventMask.ButtonPressMask | XEventMask.ButtonReleaseMask | XEventMask.EnterWindowMask | XEventMask.LeaveWindowMask | XEventMask.PointerMotionMask | XEventMask.PointerMotionHintMask | XEventMask.Button1MotionMask | XEventMask.Button2MotionMask | XEventMask.Button3MotionMask | XEventMask.Button4MotionMask | XEventMask.Button5MotionMask | XEventMask.ButtonMotionMask | XEventMask.KeymapStateMask | XEventMask.ExposureMask | XEventMask.VisibilityChangeMask | XEventMask.StructureNotifyMask | XEventMask.ResizeRedirectMask | XEventMask.SubstructureNotifyMask | XEventMask.SubstructureRedirectMask | XEventMask.FocusChangeMask | XEventMask.PropertyChangeMask | XEventMask.ColormapChangeMask) ^ xEventMask)), display: _x11.Display, window: _handle);
		IntPtr[] array = new IntPtr[1] { _x11.Atoms.WM_DELETE_WINDOW };
		XLib.XSetWMProtocols(_x11.Display, _handle, array, array.Length);
		XLib.XChangeProperty(_x11.Display, _handle, _x11.Atoms._NET_WM_WINDOW_TYPE, _x11.Atoms.XA_ATOM, 32, PropertyMode.Replace, new IntPtr[1] { _x11.Atoms._NET_WM_WINDOW_TYPE_NORMAL }, 1);
		SetWmClass(_platform.Options.WmClass);
		List<object> list = new List<object>
		{
			new X11FramebufferSurface(_x11.DeferredDisplay, _renderHandle, depth, () => RenderScaling)
		};
		if (eglPlatformGraphics != null)
		{
			list.Insert(0, new EglGlPlatformSurface(new SurfaceInfo(this, _x11.DeferredDisplay, _handle, _renderHandle)));
		}
		if (glxPlatformGraphics != null)
		{
			list.Insert(0, new GlxGlPlatformSurface(new SurfaceInfo(this, _x11.DeferredDisplay, _handle, _renderHandle)));
		}
		list.Add(Handle);
		Surfaces = list.ToArray();
		UpdateMotifHints();
		UpdateSizeHints(null);
		_rawEventGrouper = new RawEventGrouper(DispatchInput, platform.EventGrouperDispatchQueue);
		_transparencyHelper = new TransparencyHelper(_x11, _handle, platform.Globals);
		_transparencyHelper.SetTransparencyRequest(Array.Empty<WindowTransparencyLevel>());
		CreateIC();
		XLib.XFlush(_x11.Display);
		if (_popup)
		{
			PopupPositioner = new ManagedPopupPositioner(new ManagedPopupPositionerPopupImplHelper(popupParent, MoveResize));
		}
		if (platform.Options.UseDBusMenu)
		{
			_nativeMenuExporter = DBusMenuExporter.TryCreateTopLevelNativeMenu(_handle);
		}
		_nativeControlHost = new X11NativeControlHost(_platform, this);
		InitializeIme();
		XLib.XChangeProperty(_x11.Display, _handle, _x11.Atoms.WM_PROTOCOLS, _x11.Atoms.XA_ATOM, 32, PropertyMode.Replace, new IntPtr[2]
		{
			_x11.Atoms.WM_DELETE_WINDOW,
			_x11.Atoms._NET_WM_SYNC_REQUEST
		}, 2);
		if (_x11.HasXSync)
		{
			_xSyncCounter = XLib.XSyncCreateCounter(_x11.Display, _xSyncValue);
			XLib.XChangeProperty(_x11.Display, _handle, _x11.Atoms._NET_WM_SYNC_REQUEST_COUNTER, _x11.Atoms.XA_CARDINAL, 32, PropertyMode.Replace, ref _xSyncCounter, 1);
		}
		_storageProvider = new CompositeStorageProvider(new Func<Task<IStorageProvider>>[2]
		{
			() => (Task<IStorageProvider?>)((!_platform.Options.UseDBusFilePicker) ? ((Task)Task.FromResult<IStorageProvider>(null)) : ((Task)DBusSystemDialog.TryCreateAsync(Handle))),
			() => GtkSystemDialog.TryCreate(this)
		});
	}

	private void UpdateMotifHints()
	{
		MotifFunctions motifFunctions = MotifFunctions.Resize | MotifFunctions.Move | MotifFunctions.Minimize | MotifFunctions.Maximize | MotifFunctions.Close;
		MotifDecorations motifDecorations = MotifDecorations.Border | MotifDecorations.ResizeH | MotifDecorations.Title | MotifDecorations.Menu | MotifDecorations.Minimize | MotifDecorations.Maximize;
		if (_popup || _systemDecorations == SystemDecorations.None)
		{
			motifDecorations = (MotifDecorations)0;
		}
		if (!_canResize)
		{
			motifFunctions &= ~(MotifFunctions.Resize | MotifFunctions.Maximize);
			motifDecorations &= ~(MotifDecorations.ResizeH | MotifDecorations.Maximize);
		}
		MotifWmHints motifWmHints = default(MotifWmHints);
		motifWmHints.flags = new IntPtr(3);
		motifWmHints.decorations = new IntPtr((int)motifDecorations);
		motifWmHints.functions = new IntPtr((int)motifFunctions);
		MotifWmHints data = motifWmHints;
		XLib.XChangeProperty(_x11.Display, _handle, _x11.Atoms._MOTIF_WM_HINTS, _x11.Atoms._MOTIF_WM_HINTS, 32, PropertyMode.Replace, ref data, 5);
	}

	private void UpdateSizeHints(PixelSize? preResize)
	{
		PixelSize pixelSize = _minMaxSize.minSize;
		PixelSize pixelSize2 = _minMaxSize.maxSize;
		if (!_canResize)
		{
			pixelSize2 = (pixelSize = _realSize);
		}
		if (preResize.HasValue)
		{
			PixelSize value = preResize.Value;
			pixelSize2 = new PixelSize(Math.Max(value.Width, pixelSize2.Width), Math.Max(value.Height, pixelSize2.Height));
			pixelSize = new PixelSize(Math.Min(value.Width, pixelSize.Width), Math.Min(value.Height, pixelSize.Height));
		}
		XSizeHints xSizeHints = default(XSizeHints);
		xSizeHints.min_width = pixelSize.Width;
		xSizeHints.min_height = pixelSize.Height;
		XSizeHints hints = xSizeHints;
		hints.height_inc = (hints.width_inc = 1);
		XSizeHintsFlags xSizeHintsFlags = XSizeHintsFlags.PMinSize | XSizeHintsFlags.PResizeInc;
		if (_usePositioningFlags)
		{
			xSizeHintsFlags |= XSizeHintsFlags.PPosition | XSizeHintsFlags.PSize;
		}
		if (pixelSize2.Width < 100000 && pixelSize2.Height < 100000)
		{
			hints.max_width = pixelSize2.Width;
			hints.max_height = pixelSize2.Height;
			xSizeHintsFlags |= XSizeHintsFlags.PMaxSize;
		}
		hints.flags = (IntPtr)(int)xSizeHintsFlags;
		XLib.XSetWMNormalHints(_x11.Display, _handle, ref hints);
	}

	private unsafe void OnEvent(ref XEvent ev)
	{
		if (_inputRoot == null)
		{
			return;
		}
		if (ev.type == XEventName.MapNotify)
		{
			_mapped = true;
			if (_useRenderWindow)
			{
				XLib.XMapWindow(_x11.Display, _renderHandle);
			}
		}
		else if (ev.type == XEventName.UnmapNotify)
		{
			_mapped = false;
		}
		else if (ev.type == XEventName.Expose || (ev.type == XEventName.VisibilityNotify && ev.VisibilityEvent.state < 2))
		{
			EnqueuePaint();
		}
		else if (ev.type == XEventName.FocusIn)
		{
			if (!ActivateTransientChildIfNeeded())
			{
				Activated?.Invoke();
				_imeControl?.SetWindowActive(active: true);
			}
		}
		else if (ev.type == XEventName.FocusOut)
		{
			_imeControl?.SetWindowActive(active: false);
			Deactivated?.Invoke();
		}
		else if (ev.type == XEventName.MotionNotify)
		{
			MouseEvent(RawPointerEventType.Move, ref ev, ev.MotionEvent.state);
		}
		else if (ev.type == XEventName.LeaveNotify)
		{
			MouseEvent(RawPointerEventType.LeaveWindow, ref ev, ev.CrossingEvent.state);
		}
		else if (ev.type == XEventName.PropertyNotify)
		{
			OnPropertyChange(ev.PropertyEvent.atom, ev.PropertyEvent.state == 0);
		}
		else if (ev.type == XEventName.ButtonPress)
		{
			if (!ActivateTransientChildIfNeeded())
			{
				if (ev.ButtonEvent.button < 4 || ev.ButtonEvent.button == 8 || ev.ButtonEvent.button == 9)
				{
					MouseEvent(ev.ButtonEvent.button switch
					{
						1 => RawPointerEventType.LeftButtonDown, 
						2 => RawPointerEventType.MiddleButtonDown, 
						3 => RawPointerEventType.RightButtonDown, 
						8 => RawPointerEventType.XButton1Down, 
						9 => RawPointerEventType.XButton2Down, 
						_ => throw new NotSupportedException("Unexepected RawPointerEventType."), 
					}, ref ev, ev.ButtonEvent.state);
				}
				else
				{
					Vector delta = ((ev.ButtonEvent.button == 4) ? new Vector(0.0, 1.0) : ((ev.ButtonEvent.button == 5) ? new Vector(0.0, -1.0) : ((ev.ButtonEvent.button == 6) ? new Vector(1.0, 0.0) : new Vector(-1.0, 0.0))));
					ScheduleInput(new RawMouseWheelEventArgs(_mouse, (ulong)ev.ButtonEvent.time.ToInt64(), _inputRoot, new Point(ev.ButtonEvent.x, ev.ButtonEvent.y), delta, TranslateModifiers(ev.ButtonEvent.state)), ref ev);
				}
			}
		}
		else if (ev.type == XEventName.ButtonRelease)
		{
			if (ev.ButtonEvent.button < 4 || ev.ButtonEvent.button == 8 || ev.ButtonEvent.button == 9)
			{
				MouseEvent(ev.ButtonEvent.button switch
				{
					1 => RawPointerEventType.LeftButtonUp, 
					2 => RawPointerEventType.MiddleButtonUp, 
					3 => RawPointerEventType.RightButtonUp, 
					8 => RawPointerEventType.XButton1Up, 
					9 => RawPointerEventType.XButton2Up, 
					_ => throw new NotSupportedException("Unexepected RawPointerEventType."), 
				}, ref ev, ev.ButtonEvent.state);
			}
		}
		else if (ev.type == XEventName.ConfigureNotify)
		{
			if (ev.ConfigureEvent.window != _handle)
			{
				return;
			}
			bool num = !_configure.HasValue;
			_configure = ev.ConfigureEvent;
			if (ev.ConfigureEvent.override_redirect != 0 || ev.ConfigureEvent.send_event != 0)
			{
				_configurePoint = new PixelPoint(ev.ConfigureEvent.x, ev.ConfigureEvent.y);
			}
			else
			{
				XLib.XTranslateCoordinates(_x11.Display, _handle, _x11.RootWindow, 0, 0, out var intdest_x_return, out var dest_y_return, out var _);
				_configurePoint = new PixelPoint(intdest_x_return, dest_y_return);
			}
			if (num)
			{
				Dispatcher.UIThread.Post(delegate
				{
					if (_configure.HasValue)
					{
						XConfigureEvent value = _configure.Value;
						PixelPoint value2 = _configurePoint.Value;
						_configure = null;
						_configurePoint = null;
						PixelSize pixelSize = new PixelSize(value.width, value.height);
						bool num2 = _realSize != pixelSize;
						int num3;
						if (_position.HasValue)
						{
							PixelPoint value3 = value2;
							PixelPoint? position = _position;
							num3 = ((value3 != position) ? 1 : 0);
						}
						else
						{
							num3 = 1;
						}
						_realSize = pixelSize;
						_position = value2;
						bool flag = false;
						if (num3 != 0)
						{
							PositionChanged?.Invoke(value2);
							flag = UpdateScaling();
						}
						UpdateImePosition();
						if (num2 && !flag && !_popup)
						{
							Resized?.Invoke(ClientSize, WindowResizeReason.Unspecified);
						}
					}
				}, DispatcherPriority.AsyncRenderTargetResize);
			}
			if (_useRenderWindow)
			{
				XLib.XConfigureResizeWindow(_x11.Display, _renderHandle, ev.ConfigureEvent.width, ev.ConfigureEvent.height);
			}
			if (_xSyncState == XSyncState.WaitConfigure)
			{
				_xSyncState = XSyncState.WaitPaint;
				EnqueuePaint();
			}
		}
		else if (ev.type == XEventName.DestroyNotify && ev.DestroyWindowEvent.window == _handle)
		{
			Cleanup(fromDestroyNotification: true);
		}
		else if (ev.type == XEventName.ClientMessage)
		{
			if (!(ev.ClientMessageEvent.message_type == _x11.Atoms.WM_PROTOCOLS))
			{
				return;
			}
			if (ev.ClientMessageEvent.ptr1 == _x11.Atoms.WM_DELETE_WINDOW)
			{
				Func<WindowCloseReason, bool>? closing = Closing;
				if (closing == null || !closing(WindowCloseReason.WindowClosing))
				{
					Dispose();
				}
			}
			else if (ev.ClientMessageEvent.ptr1 == _x11.Atoms._NET_WM_SYNC_REQUEST)
			{
				_xSyncValue.Lo = new UIntPtr(ev.ClientMessageEvent.ptr3.ToPointer()).ToUInt32();
				_xSyncValue.Hi = ev.ClientMessageEvent.ptr4.ToInt32();
				_xSyncState = XSyncState.WaitConfigure;
			}
		}
		else if ((ev.type == XEventName.KeyPress || ev.type == XEventName.KeyRelease) && !ActivateTransientChildIfNeeded())
		{
			HandleKeyEvent(ref ev);
		}
	}

	private unsafe Thickness? GetFrameExtents()
	{
		XLib.XGetWindowProperty(_x11.Display, _handle, _x11.Atoms._NET_FRAME_EXTENTS, IntPtr.Zero, new IntPtr(4), delete: false, (IntPtr)0, out var _, out var _, out var nitems, out var _, out var prop);
		if (nitems.ToInt64() != 4)
		{
			return null;
		}
		IntPtr* ptr = (IntPtr*)prop.ToPointer();
		Thickness value = new Thickness(ptr->ToInt32(), ptr[2].ToInt32(), ptr[1].ToInt32(), ptr[3].ToInt32());
		XLib.XFree(prop);
		return value;
	}

	private bool UpdateScaling(bool skipResize = false)
	{
		double num = ((!_scalingOverride.HasValue) ? (_platform.X11Screens.Screens.OrderBy((X11Screen x) => x.Scaling).FirstOrDefault((X11Screen m) => m.Bounds.Contains(_position.GetValueOrDefault()))?.Scaling ?? RenderScaling) : _scalingOverride.Value);
		if (RenderScaling != num)
		{
			Size clientSize = ClientSize;
			RenderScaling = num;
			ScalingChanged?.Invoke(RenderScaling);
			UpdateImePosition();
			SetMinMaxSize(_scaledMinMaxSize.minSize, _scaledMinMaxSize.maxSize);
			if (!skipResize)
			{
				Resize(clientSize, force: true, WindowResizeReason.DpiChange);
			}
			return true;
		}
		return false;
	}

	private unsafe void OnPropertyChange(IntPtr atom, bool hasValue)
	{
		if (atom == _x11.Atoms._NET_FRAME_EXTENTS)
		{
			Resized?.Invoke(ClientSize, WindowResizeReason.Unspecified);
		}
		if (!(atom == _x11.Atoms._NET_WM_STATE))
		{
			return;
		}
		WindowState windowState = WindowState.Normal;
		if (hasValue)
		{
			XLib.XGetWindowProperty(_x11.Display, _handle, _x11.Atoms._NET_WM_STATE, IntPtr.Zero, new IntPtr(256), delete: false, (IntPtr)4, out var _, out var _, out var nitems, out var _, out var prop);
			int num = 0;
			IntPtr* ptr = (IntPtr*)prop.ToPointer();
			for (int i = 0; i < nitems.ToInt32(); i++)
			{
				if (ptr[i] == _x11.Atoms._NET_WM_STATE_HIDDEN)
				{
					windowState = WindowState.Minimized;
					break;
				}
				if (ptr[i] == _x11.Atoms._NET_WM_STATE_FULLSCREEN)
				{
					windowState = WindowState.FullScreen;
					break;
				}
				if (ptr[i] == _x11.Atoms._NET_WM_STATE_MAXIMIZED_HORZ || ptr[i] == _x11.Atoms._NET_WM_STATE_MAXIMIZED_VERT)
				{
					num++;
					if (num == 2)
					{
						windowState = WindowState.Maximized;
						break;
					}
				}
			}
			XLib.XFree(prop);
		}
		if (_lastWindowState != windowState)
		{
			_lastWindowState = windowState;
			WindowStateChanged?.Invoke(windowState);
		}
	}

	private static RawInputModifiers TranslateModifiers(XModifierMask state)
	{
		RawInputModifiers rawInputModifiers = RawInputModifiers.None;
		if (state.HasAllFlags(XModifierMask.Button1Mask))
		{
			rawInputModifiers |= RawInputModifiers.LeftMouseButton;
		}
		if (state.HasAllFlags(XModifierMask.Button2Mask))
		{
			rawInputModifiers |= RawInputModifiers.RightMouseButton;
		}
		if (state.HasAllFlags(XModifierMask.Button3Mask))
		{
			rawInputModifiers |= RawInputModifiers.MiddleMouseButton;
		}
		if (state.HasAllFlags(XModifierMask.Button4Mask))
		{
			rawInputModifiers |= RawInputModifiers.XButton1MouseButton;
		}
		if (state.HasAllFlags(XModifierMask.Button5Mask))
		{
			rawInputModifiers |= RawInputModifiers.XButton2MouseButton;
		}
		if (state.HasAllFlags(XModifierMask.ShiftMask))
		{
			rawInputModifiers |= RawInputModifiers.Shift;
		}
		if (state.HasAllFlags(XModifierMask.ControlMask))
		{
			rawInputModifiers |= RawInputModifiers.Control;
		}
		if (state.HasAllFlags(XModifierMask.Mod1Mask))
		{
			rawInputModifiers |= RawInputModifiers.Alt;
		}
		if (state.HasAllFlags(XModifierMask.Mod4Mask))
		{
			rawInputModifiers |= RawInputModifiers.Meta;
		}
		return rawInputModifiers;
	}

	private void ScheduleInput(RawInputEventArgs args, ref XEvent xev)
	{
		_x11.LastActivityTimestamp = xev.ButtonEvent.time;
		ScheduleInput(args);
	}

	private void DispatchInput(RawInputEventArgs args)
	{
		if (_inputRoot != null && (!_disabled || !(args is RawPointerEventArgs { Type: RawPointerEventType.Move })))
		{
			Input?.Invoke(args);
			if (!args.Handled && args is RawKeyEventArgsWithText rawKeyEventArgsWithText && !string.IsNullOrEmpty(rawKeyEventArgsWithText.Text))
			{
				Input?.Invoke(new RawTextInputEventArgs(_keyboard, args.Timestamp, _inputRoot, rawKeyEventArgsWithText.Text));
			}
		}
	}

	public void ScheduleXI2Input(RawInputEventArgs args)
	{
		if (args is RawPointerEventArgs rawPointerEventArgs)
		{
			if ((rawPointerEventArgs.Type == RawPointerEventType.TouchBegin || rawPointerEventArgs.Type == RawPointerEventType.TouchUpdate || rawPointerEventArgs.Type == RawPointerEventType.LeftButtonDown || rawPointerEventArgs.Type == RawPointerEventType.RightButtonDown || rawPointerEventArgs.Type == RawPointerEventType.MiddleButtonDown || rawPointerEventArgs.Type == RawPointerEventType.NonClientLeftButtonDown) && ActivateTransientChildIfNeeded())
			{
				return;
			}
			if (rawPointerEventArgs.Type == RawPointerEventType.TouchEnd && ActivateTransientChildIfNeeded())
			{
				rawPointerEventArgs.Type = RawPointerEventType.TouchCancel;
			}
		}
		ScheduleInput(args);
	}

	private void ScheduleInput(RawInputEventArgs args)
	{
		if (args is RawPointerEventArgs rawPointerEventArgs)
		{
			rawPointerEventArgs.Position /= RenderScaling;
		}
		if (args is RawDragEvent rawDragEvent)
		{
			rawDragEvent.Location /= RenderScaling;
		}
		_rawEventGrouper?.HandleEvent(args);
	}

	private void MouseEvent(RawPointerEventType type, ref XEvent ev, XModifierMask mods)
	{
		if (_inputRoot != null)
		{
			RawPointerEventArgs args = new RawPointerEventArgs(_mouse, (ulong)ev.ButtonEvent.time.ToInt64(), _inputRoot, type, new Point(ev.ButtonEvent.x, ev.ButtonEvent.y), TranslateModifiers(mods));
			ScheduleInput(args, ref ev);
		}
	}

	private void EnqueuePaint()
	{
		if (!_triggeredExpose)
		{
			_triggeredExpose = true;
			Dispatcher.UIThread.Post(delegate
			{
				_triggeredExpose = false;
				DoPaint();
			}, DispatcherPriority.UiThreadRender);
		}
	}

	private void DoPaint()
	{
		Paint?.Invoke(default(Rect));
		if (_xSyncCounter != IntPtr.Zero && _xSyncState == XSyncState.WaitPaint)
		{
			_xSyncState = XSyncState.None;
			XLib.XSyncSetCounter(_x11.Display, _xSyncCounter, _xSyncValue);
		}
	}

	public void Invalidate(Rect rect)
	{
	}

	public void SetInputRoot(IInputRoot inputRoot)
	{
		_inputRoot = inputRoot;
	}

	public void Dispose()
	{
		Cleanup(fromDestroyNotification: false);
	}

	public virtual object? TryGetFeature(Type featureType)
	{
		if (featureType == typeof(ITopLevelNativeMenuExporter))
		{
			return _nativeMenuExporter;
		}
		if (featureType == typeof(IStorageProvider))
		{
			return _storageProvider;
		}
		if (featureType == typeof(ITextInputMethodImpl))
		{
			return _ime;
		}
		if (featureType == typeof(INativeControlHostImpl))
		{
			return _nativeControlHost;
		}
		if (featureType == typeof(IClipboard))
		{
			return AvaloniaLocator.Current.GetRequiredService<IClipboard>();
		}
		return null;
	}

	private void Cleanup(bool fromDestroyNotification)
	{
		if (_cleaningUp)
		{
			return;
		}
		_cleaningUp = true;
		if (_handle != IntPtr.Zero)
		{
			Closed?.Invoke();
		}
		if (_rawEventGrouper != null)
		{
			_rawEventGrouper.Dispose();
			_rawEventGrouper = null;
		}
		if (_transparencyHelper != null)
		{
			_transparencyHelper.Dispose();
			_transparencyHelper = null;
		}
		if (_imeControl != null)
		{
			_imeControl.Dispose();
			_imeControl = null;
			_ime = null;
		}
		if (_xic != IntPtr.Zero)
		{
			XLib.XDestroyIC(_xic);
			_xic = IntPtr.Zero;
		}
		if (_xSyncCounter != IntPtr.Zero)
		{
			XLib.XSyncDestroyCounter(_x11.Display, _xSyncCounter);
			_xSyncCounter = IntPtr.Zero;
		}
		if (_handle != IntPtr.Zero)
		{
			_platform.Windows.Remove(_handle);
			_platform.XI2?.OnWindowDestroyed(_handle);
			IntPtr handle = _handle;
			_handle = IntPtr.Zero;
			_mouse.Dispose();
			_touch.Dispose();
			if (!fromDestroyNotification)
			{
				XLib.XDestroyWindow(_x11.Display, handle);
			}
		}
		if (_useRenderWindow && _renderHandle != IntPtr.Zero)
		{
			_renderHandle = IntPtr.Zero;
		}
	}

	private bool ActivateTransientChildIfNeeded()
	{
		if (_disabled)
		{
			GotInputWhenDisabled?.Invoke();
			return true;
		}
		return false;
	}

	public void SetParent(IWindowImpl parent)
	{
		if (parent == null || parent.Handle == null || parent.Handle.Handle == IntPtr.Zero)
		{
			XLib.XDeleteProperty(_x11.Display, _handle, _x11.Atoms.XA_WM_TRANSIENT_FOR);
		}
		else
		{
			XLib.XSetTransientForHint(_x11.Display, _handle, parent.Handle.Handle);
		}
	}

	public void Show(bool activate, bool isDialog)
	{
		_wasMappedAtLeastOnce = true;
		XLib.XMapWindow(_x11.Display, _handle);
		XLib.XFlush(_x11.Display);
	}

	public void Hide()
	{
		XLib.XUnmapWindow(_x11.Display, _handle);
	}

	public Point PointToClient(PixelPoint point)
	{
		return new Point((double)(point.X - _position.GetValueOrDefault().X) / RenderScaling, (double)(point.Y - _position.GetValueOrDefault().Y) / RenderScaling);
	}

	public PixelPoint PointToScreen(Point point)
	{
		return new PixelPoint((int)(point.X * RenderScaling + (double)_position.GetValueOrDefault().X), (int)(point.Y * RenderScaling + (double)_position.GetValueOrDefault().Y));
	}

	public void SetSystemDecorations(SystemDecorations enabled)
	{
		_systemDecorations = ((enabled == SystemDecorations.Full) ? SystemDecorations.Full : SystemDecorations.None);
		UpdateMotifHints();
		UpdateSizeHints(null);
	}

	public void Resize(Size clientSize, WindowResizeReason reason)
	{
		Resize(clientSize, force: false, reason);
	}

	public void Move(PixelPoint point)
	{
		Position = point;
	}

	private void MoveResize(PixelPoint position, Size size, double scaling)
	{
		Move(position);
		_scalingOverride = scaling;
		UpdateScaling(skipResize: true);
		Resize(size, force: true, WindowResizeReason.Layout);
	}

	private PixelSize ToPixelSize(Size size)
	{
		return new PixelSize((int)(size.Width * RenderScaling), (int)(size.Height * RenderScaling));
	}

	private void Resize(Size clientSize, bool force, WindowResizeReason reason)
	{
		if (force || !(clientSize == ClientSize))
		{
			bool flag = clientSize != ClientSize;
			PixelSize pixelSize = ToPixelSize(clientSize);
			UpdateSizeHints(pixelSize);
			XLib.XConfigureResizeWindow(_x11.Display, _handle, pixelSize);
			if (_useRenderWindow)
			{
				XLib.XConfigureResizeWindow(_x11.Display, _renderHandle, pixelSize);
			}
			XLib.XFlush(_x11.Display);
			if (force || !_wasMappedAtLeastOnce || (_popup && flag))
			{
				_realSize = pixelSize;
				Resized?.Invoke(ClientSize, reason);
			}
		}
	}

	public void CanResize(bool value)
	{
		_canResize = value;
		UpdateMotifHints();
		UpdateSizeHints(null);
	}

	public void SetCursor(ICursorImpl? cursor)
	{
		if (cursor == null)
		{
			XLib.XDefineCursor(_x11.Display, _handle, _x11.DefaultCursor);
		}
		else if (cursor is CursorImpl cursorImpl)
		{
			XLib.XDefineCursor(_x11.Display, _handle, cursorImpl.Handle);
		}
	}

	public IPopupImpl? CreatePopup()
	{
		if (!_platform.Options.OverlayPopups)
		{
			return new X11Window(_platform, this);
		}
		return null;
	}

	public void Activate()
	{
		if (_x11.Atoms._NET_ACTIVE_WINDOW != IntPtr.Zero)
		{
			SendNetWMMessage(_x11.Atoms._NET_ACTIVE_WINDOW, (IntPtr)1, _x11.LastActivityTimestamp, IntPtr.Zero, null, null);
			return;
		}
		XLib.XRaiseWindow(_x11.Display, _handle);
		XLib.XSetInputFocus(_x11.Display, _handle, RevertTo.None, IntPtr.Zero);
	}

	private void SendNetWMMessage(IntPtr message_type, IntPtr l0, IntPtr? l1 = null, IntPtr? l2 = null, IntPtr? l3 = null, IntPtr? l4 = null)
	{
		XEvent xEvent = default(XEvent);
		xEvent.ClientMessageEvent.type = XEventName.ClientMessage;
		xEvent.ClientMessageEvent.send_event = 1;
		xEvent.ClientMessageEvent.window = _handle;
		xEvent.ClientMessageEvent.message_type = message_type;
		xEvent.ClientMessageEvent.format = 32;
		xEvent.ClientMessageEvent.ptr1 = l0;
		xEvent.ClientMessageEvent.ptr2 = l1 ?? IntPtr.Zero;
		xEvent.ClientMessageEvent.ptr3 = l2 ?? IntPtr.Zero;
		xEvent.ClientMessageEvent.ptr4 = l3 ?? IntPtr.Zero;
		XEvent send_event = xEvent;
		send_event.ClientMessageEvent.ptr4 = l4 ?? IntPtr.Zero;
		XLib.XSendEvent(_x11.Display, _x11.RootWindow, propagate: false, new IntPtr(1572864), ref send_event);
	}

	private void BeginMoveResize(NetWmMoveResize side, PointerPressedEventArgs e)
	{
		(int, int) cursorPos = XLib.GetCursorPos(_x11, null);
		XLib.XUngrabPointer(_x11.Display, new IntPtr(0));
		SendNetWMMessage(_x11.Atoms._NET_WM_MOVERESIZE, (IntPtr)cursorPos.Item1, (IntPtr)cursorPos.Item2, (IntPtr)(int)side, (IntPtr)1, (IntPtr)1);
		e.Pointer.Capture(null);
	}

	public void BeginMoveDrag(PointerPressedEventArgs e)
	{
		BeginMoveResize(NetWmMoveResize._NET_WM_MOVERESIZE_MOVE, e);
	}

	public void BeginResizeDrag(WindowEdge edge, PointerPressedEventArgs e)
	{
		NetWmMoveResize side = NetWmMoveResize._NET_WM_MOVERESIZE_CANCEL;
		if (edge == WindowEdge.East)
		{
			side = NetWmMoveResize._NET_WM_MOVERESIZE_SIZE_RIGHT;
		}
		if (edge == WindowEdge.North)
		{
			side = NetWmMoveResize._NET_WM_MOVERESIZE_SIZE_TOP;
		}
		if (edge == WindowEdge.South)
		{
			side = NetWmMoveResize._NET_WM_MOVERESIZE_SIZE_BOTTOM;
		}
		if (edge == WindowEdge.West)
		{
			side = NetWmMoveResize._NET_WM_MOVERESIZE_SIZE_LEFT;
		}
		if (edge == WindowEdge.NorthEast)
		{
			side = NetWmMoveResize._NET_WM_MOVERESIZE_SIZE_TOPRIGHT;
		}
		if (edge == WindowEdge.NorthWest)
		{
			side = NetWmMoveResize._NET_WM_MOVERESIZE_SIZE_TOPLEFT;
		}
		if (edge == WindowEdge.SouthEast)
		{
			side = NetWmMoveResize._NET_WM_MOVERESIZE_SIZE_BOTTOMRIGHT;
		}
		if (edge == WindowEdge.SouthWest)
		{
			side = NetWmMoveResize._NET_WM_MOVERESIZE_SIZE_BOTTOMLEFT;
		}
		BeginMoveResize(side, e);
	}

	public unsafe void SetTitle(string? title)
	{
		if (string.IsNullOrEmpty(title))
		{
			XLib.XDeleteProperty(_x11.Display, _handle, _x11.Atoms._NET_WM_NAME);
			XLib.XDeleteProperty(_x11.Display, _handle, _x11.Atoms.XA_WM_NAME);
			return;
		}
		byte[] bytes = Encoding.UTF8.GetBytes(title);
		fixed (byte* ptr = bytes)
		{
			void* data = ptr;
			XLib.XChangeProperty(_x11.Display, _handle, _x11.Atoms._NET_WM_NAME, _x11.Atoms.UTF8_STRING, 8, PropertyMode.Replace, data, bytes.Length);
			XLib.XStoreName(_x11.Display, _handle, title);
		}
	}

	public unsafe void SetWmClass(string wmClass)
	{
		string text = Environment.GetEnvironmentVariable("RESOURCE_NAME") ?? Process.GetCurrentProcess().ProcessName;
		byte[] bytes = Encoding.ASCII.GetBytes(text);
		byte[] bytes2 = Encoding.ASCII.GetBytes(wmClass ?? text);
		XLib.XClassHint* ptr = XLib.XAllocClassHint();
		fixed (byte* res_name = bytes)
		{
			fixed (byte* res_class = bytes2)
			{
				ptr->res_name = res_name;
				ptr->res_class = res_class;
				XLib.XSetClassHint(_x11.Display, _handle, ptr);
			}
		}
		XLib.XFree(ptr);
	}

	public void SetMinMaxSize(Size minSize, Size maxSize)
	{
		_scaledMinMaxSize = (minSize: minSize, maxSize: maxSize);
		PixelSize item = new PixelSize((int)((minSize.Width < 1.0) ? 1.0 : (minSize.Width * RenderScaling)), (int)((minSize.Height < 1.0) ? 1.0 : (minSize.Height * RenderScaling)));
		PixelSize item2 = new PixelSize((int)((maxSize.Width > 100000.0) ? 100000.0 : Math.Max(item.Width, maxSize.Width * RenderScaling)), (int)((maxSize.Height > 100000.0) ? 100000.0 : Math.Max(item.Height, maxSize.Height * RenderScaling)));
		_minMaxSize = (minSize: item, maxSize: item2);
		UpdateSizeHints(null);
	}

	public void SetTopmost(bool value)
	{
		ChangeWMAtoms(value, _x11.Atoms._NET_WM_STATE_ABOVE);
	}

	public void SetEnabled(bool enable)
	{
		_disabled = !enable;
		UpdateWMHints();
	}

	private void UpdateWMHints()
	{
		IntPtr intPtr = XLib.XGetWMHints(_x11.Display, _handle);
		XWMHints wmhints = default(XWMHints);
		if (intPtr != IntPtr.Zero)
		{
			wmhints = Marshal.PtrToStructure<XWMHints>(intPtr);
		}
		long num = wmhints.flags.ToInt64();
		num |= 1;
		wmhints.flags = (IntPtr)num;
		wmhints.input = ((!_disabled) ? 1 : 0);
		XLib.XSetWMHints(_x11.Display, _handle, ref wmhints);
		if (intPtr != IntPtr.Zero)
		{
			XLib.XFree(intPtr);
		}
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

	public unsafe void SetIcon(IWindowIconImpl? icon)
	{
		if (icon != null)
		{
			UIntPtr[] data = ((X11IconData)icon).Data;
			fixed (UIntPtr* ptr = data)
			{
				void* data2 = ptr;
				XLib.XChangeProperty(_x11.Display, _handle, _x11.Atoms._NET_WM_ICON, new IntPtr(6), 32, PropertyMode.Replace, data2, data.Length);
			}
		}
		else
		{
			XLib.XDeleteProperty(_x11.Display, _handle, _x11.Atoms._NET_WM_ICON);
		}
	}

	public void ShowTaskbarIcon(bool value)
	{
		ChangeWMAtoms(!value, _x11.Atoms._NET_WM_STATE_SKIP_TASKBAR);
	}

	private unsafe void ChangeWMAtoms(bool enable, params IntPtr[] atoms)
	{
		if (atoms.Length != 1 && atoms.Length != 2)
		{
			throw new ArgumentException();
		}
		if (!_mapped)
		{
			XLib.XGetWindowProperty(_x11.Display, _handle, _x11.Atoms._NET_WM_STATE, IntPtr.Zero, new IntPtr(256), delete: false, (IntPtr)4, out var _, out var _, out var nitems, out var _, out var prop);
			IntPtr* ptr = (IntPtr*)prop.ToPointer();
			HashSet<IntPtr> hashSet = new HashSet<IntPtr>();
			for (int i = 0; i < nitems.ToInt64(); i++)
			{
				hashSet.Add(*ptr);
			}
			XLib.XFree(prop);
			foreach (IntPtr item in atoms)
			{
				if (enable)
				{
					hashSet.Add(item);
				}
				else
				{
					hashSet.Remove(item);
				}
			}
			XLib.XChangeProperty(_x11.Display, _handle, _x11.Atoms._NET_WM_STATE, (IntPtr)4, 32, PropertyMode.Replace, hashSet.ToArray(), hashSet.Count);
		}
		SendNetWMMessage(_x11.Atoms._NET_WM_STATE, (IntPtr)(enable ? 1 : 0), atoms[0], (atoms.Length > 1) ? atoms[1] : IntPtr.Zero, (atoms.Length > 2) ? atoms[2] : IntPtr.Zero, (atoms.Length > 3) ? atoms[3] : IntPtr.Zero);
	}

	public void SetTransparencyLevelHint(IReadOnlyList<WindowTransparencyLevel> transparencyLevels)
	{
		_transparencyHelper?.SetTransparencyRequest(transparencyLevels);
	}

	public void SetWindowManagerAddShadowHint(bool enabled)
	{
	}

	public void SetFrameThemeVariant(PlatformThemeVariant themeVariant)
	{
	}

	private unsafe void CreateIC()
	{
		if (_x11.HasXim)
		{
			XLib.XGetIMValues(_x11.Xim, "queryInputStyle", out var value, IntPtr.Zero);
			for (int i = 0; i < value->count_styles; i++)
			{
				XIMProperties xIMProperties = (XIMProperties)(int)value->supported_styles[i];
				if ((xIMProperties & XIMProperties.XIMPreeditPosition) == 0 || (xIMProperties & XIMProperties.XIMStatusNothing) == 0)
				{
					continue;
				}
				XPoint xPoint = default(XPoint);
				using (Utf8Buffer xnSpotLocation = new Utf8Buffer("spotLocation"))
				{
					using Utf8Buffer xnFontSet = new Utf8Buffer("fontSet");
					IntPtr intPtr = XLib.XVaCreateNestedList(0, xnSpotLocation, &xPoint, xnFontSet, _x11.DefaultFontSet, IntPtr.Zero);
					_xic = XLib.XCreateIC(_x11.Xim, "clientWindow", _handle, "focusWindow", _handle, "inputStyle", new IntPtr((int)xIMProperties), "resourceName", _platform.Options.WmClass, "resourceClass", _platform.Options.WmClass, "preeditAttributes", intPtr, IntPtr.Zero);
					XLib.XFree(intPtr);
				}
				break;
			}
			XLib.XFree(new IntPtr(value));
		}
		if (_xic == IntPtr.Zero)
		{
			_xic = XLib.XCreateIC(_x11.Xim, "inputStyle", new IntPtr(1032), "clientWindow", _handle, "focusWindow", _handle, IntPtr.Zero);
		}
	}

	private void InitializeIme()
	{
		(ITextInputMethodImpl, IX11InputMethodControl)? tuple = AvaloniaLocator.Current.GetService<IX11InputMethodFactory>()?.CreateClient(_handle);
		if (!tuple.HasValue && _x11.HasXim)
		{
			XimInputMethod ximInputMethod = new XimInputMethod(this);
			tuple = (ximInputMethod, ximInputMethod);
		}
		if (tuple.HasValue)
		{
			(_ime, _imeControl) = tuple.Value;
			_imeControl.Commit += delegate(string s)
			{
				ScheduleInput(new RawTextInputEventArgs(_keyboard, (ulong)_x11.LastActivityTimestamp.ToInt64(), _inputRoot, s));
			};
			_imeControl.ForwardKey += delegate(X11InputMethodForwardedKey ev)
			{
				ScheduleInput(new RawKeyEventArgs(_keyboard, (ulong)_x11.LastActivityTimestamp.ToInt64(), _inputRoot, ev.Type, X11KeyTransform.ConvertKey((X11Key)ev.KeyVal), (RawInputModifiers)ev.Modifiers));
			};
		}
	}

	private void UpdateImePosition()
	{
		_imeControl?.UpdateWindowInfo(_position.GetValueOrDefault(), RenderScaling);
	}

	private void HandleKeyEvent(ref XEvent ev)
	{
		bool flag = ev.KeyEvent.state.HasAllFlags(XModifierMask.ShiftMask);
		X11Key x11Key = (X11Key)XLib.XKeycodeToKeysym(_x11.Display, ev.KeyEvent.keycode, flag ? 1 : 0).ToInt32();
		if (ev.KeyEvent.state.HasAllFlags(XModifierMask.Mod2Mask) && x11Key > X11Key.Num_Lock && x11Key <= X11Key.KP_9)
		{
			x11Key = (X11Key)XLib.XKeycodeToKeysym(_x11.Display, ev.KeyEvent.keycode, (!flag) ? 1 : 0).ToInt32();
		}
		Key key = X11KeyTransform.ConvertKey(x11Key);
		RawInputModifiers modifiers = TranslateModifiers(ev.KeyEvent.state);
		ulong timestamp = (ulong)ev.KeyEvent.time.ToInt64();
		RawKeyEventArgs args = ((ev.type == XEventName.KeyPress) ? new RawKeyEventArgsWithText(_keyboard, timestamp, _inputRoot, RawKeyEventType.KeyDown, key, modifiers, TranslateEventToString(ref ev)) : new RawKeyEventArgs(_keyboard, timestamp, _inputRoot, RawKeyEventType.KeyUp, key, modifiers));
		ScheduleKeyInput(args, ref ev, (int)x11Key, ev.KeyEvent.keycode);
	}

	private void TriggerClassicTextInputEvent(ref XEvent ev)
	{
		string text = TranslateEventToString(ref ev);
		if (text != null)
		{
			ScheduleInput(new RawTextInputEventArgs(_keyboard, (ulong)ev.KeyEvent.time.ToInt64(), _inputRoot, text), ref ev);
		}
	}

	private unsafe string TranslateEventToString(ref XEvent ev)
	{
		if (ImeBuffer == IntPtr.Zero)
		{
			ImeBuffer = Marshal.AllocHGlobal(65536);
		}
		IntPtr keysym;
		IntPtr status;
		int num = ((!(_xic != IntPtr.Zero)) ? XLib.XLookupString(ref ev, ImeBuffer.ToPointer(), 65536, out keysym, out status) : XLib.Xutf8LookupString(_xic, ref ev, ImeBuffer.ToPointer(), 65536, out keysym, out status));
		if (num == 0)
		{
			return null;
		}
		if ((int)status == -1)
		{
			return null;
		}
		string @string = Encoding.UTF8.GetString((byte*)ImeBuffer.ToPointer(), num);
		if (@string == null)
		{
			return null;
		}
		if (@string.Length == 1 && (@string[0] < ' ' || @string[0] == '\u007f'))
		{
			return null;
		}
		return @string;
	}

	private void ScheduleKeyInput(RawKeyEventArgs args, ref XEvent xev, int keyval, int keycode)
	{
		_x11.LastActivityTimestamp = xev.ButtonEvent.time;
		IX11InputMethodControl imeControl = _imeControl;
		if (imeControl == null || !imeControl.IsEnabled || !FilterIme(args, xev, keyval, keycode))
		{
			ScheduleInput(args);
		}
	}

	private bool FilterIme(RawKeyEventArgs args, XEvent xev, int keyval, int keycode)
	{
		if (_ime == null)
		{
			return false;
		}
		_imeQueue.Enqueue((args, xev, keyval, keycode));
		if (!_processingIme)
		{
			ProcessNextImeEvent();
		}
		return true;
	}

	private async void ProcessNextImeEvent()
	{
		if (_processingIme)
		{
			return;
		}
		_processingIme = true;
		try
		{
			while (_imeQueue.Count != 0)
			{
				(RawKeyEventArgs args, XEvent xev, int keyval, int keycode) ev = _imeQueue.Dequeue();
				bool flag = _imeControl == null;
				if (!flag)
				{
					flag = !(await _imeControl.HandleEventAsync(ev.args, ev.keyval, ev.keycode));
				}
				if (flag)
				{
					ScheduleInput(ev.args);
				}
			}
		}
		finally
		{
			_processingIme = false;
		}
	}
}
