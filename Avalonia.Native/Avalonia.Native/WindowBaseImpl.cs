using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Avalonia.Automation.Peers;
using Avalonia.Controls;
using Avalonia.Controls.Platform;
using Avalonia.Controls.Platform.Surfaces;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Input.Raw;
using Avalonia.Native.Interop;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using Avalonia.Rendering.Composition;
using Avalonia.Threading;
using MicroCom.Runtime;

namespace Avalonia.Native;

internal abstract class WindowBaseImpl : IWindowBaseImpl, ITopLevelImpl, IOptionalFeatureProvider, IDisposable, IFramebufferPlatformSurface
{
	private class FramebufferRenderTarget : IFramebufferRenderTarget, IDisposable
	{
		private readonly WindowBaseImpl _parent;

		private IAvnSoftwareRenderTarget? _target;

		public FramebufferRenderTarget(WindowBaseImpl parent, IAvnSoftwareRenderTarget target)
		{
			_parent = parent;
			_target = target;
		}

		public void Dispose()
		{
			lock (_parent._syncRoot)
			{
				_target?.Dispose();
				_target = null;
			}
		}

		public ILockedFramebuffer Lock()
		{
			double num = _parent._savedLogicalSize.Width * _parent._savedScaling;
			double num2 = _parent._savedLogicalSize.Height * _parent._savedScaling;
			double num3 = _parent._savedScaling * 96.0;
			return new DeferredFramebuffer(_target, delegate(Action<IAvnWindowBase> cb)
			{
				lock (_parent._syncRoot)
				{
					if (_parent._native != null && _target != null)
					{
						cb(_parent._native);
						_parent._lastRenderedLogicalSize = _parent._savedLogicalSize;
					}
				}
			}, (int)num, (int)num2, new Vector(num3, num3));
		}
	}

	protected class WindowBaseEvents : NativeCallbackBase, IAvnWindowBaseEvents, IUnknown, IDisposable
	{
		private readonly WindowBaseImpl _parent;

		IAvnAutomationPeer IAvnWindowBaseEvents.AutomationPeer => AvnAutomationPeer.Wrap(_parent.GetAutomationPeer());

		public WindowBaseEvents(WindowBaseImpl parent)
		{
			_parent = parent;
		}

		void IAvnWindowBaseEvents.Closed()
		{
			IAvnWindowBase native = _parent._native;
			try
			{
				_parent?.Closed?.Invoke();
			}
			finally
			{
				_parent?.Dispose();
				native?.Dispose();
			}
		}

		void IAvnWindowBaseEvents.Activated()
		{
			_parent.Activated?.Invoke();
		}

		void IAvnWindowBaseEvents.Deactivated()
		{
			_parent.Deactivated?.Invoke();
		}

		void IAvnWindowBaseEvents.Paint()
		{
			Dispatcher.UIThread.RunJobs(DispatcherPriority.UiThreadRender);
			Size clientSize = _parent.ClientSize;
			_parent.Paint?.Invoke(new Rect(0.0, 0.0, clientSize.Width, clientSize.Height));
		}

		unsafe void IAvnWindowBaseEvents.Resized(AvnSize* size, AvnPlatformResizeReason reason)
		{
			if (_parent?._native != null)
			{
				Size size2 = new Size(size->Width, size->Height);
				_parent._savedLogicalSize = size2;
				_parent.Resized?.Invoke(size2, (WindowResizeReason)reason);
			}
		}

		void IAvnWindowBaseEvents.PositionChanged(AvnPoint position)
		{
			_parent.PositionChanged?.Invoke(position.ToAvaloniaPixelPoint());
		}

		void IAvnWindowBaseEvents.RawMouseEvent(AvnRawMouseEventType type, ulong timeStamp, AvnInputModifiers modifiers, AvnPoint point, AvnVector delta)
		{
			_parent.RawMouseEvent(type, timeStamp, modifiers, point, delta);
		}

		int IAvnWindowBaseEvents.RawKeyEvent(AvnRawKeyEventType type, ulong timeStamp, AvnInputModifiers modifiers, uint key)
		{
			return _parent.RawKeyEvent(type, timeStamp, modifiers, key).AsComBool();
		}

		int IAvnWindowBaseEvents.RawTextInputEvent(ulong timeStamp, string text)
		{
			return _parent.RawTextInputEvent(timeStamp, text).AsComBool();
		}

		void IAvnWindowBaseEvents.ScalingChanged(double scaling)
		{
			_parent._savedScaling = scaling;
			_parent.ScalingChanged?.Invoke(scaling);
		}

		void IAvnWindowBaseEvents.RunRenderPriorityJobs()
		{
			Dispatcher.UIThread.RunJobs(DispatcherPriority.UiThreadRender);
		}

		void IAvnWindowBaseEvents.LostFocus()
		{
			_parent.LostFocus?.Invoke();
		}

		public AvnDragDropEffects DragEvent(AvnDragEventType type, AvnPoint position, AvnInputModifiers modifiers, AvnDragDropEffects effects, IAvnClipboard clipboard, IntPtr dataObjectHandle)
		{
			IDragDropDevice service = AvaloniaLocator.Current.GetService<IDragDropDevice>();
			IDataObject dataObject = null;
			if (dataObjectHandle != IntPtr.Zero)
			{
				dataObject = GCHandle.FromIntPtr(dataObjectHandle).Target as IDataObject;
			}
			using ClipboardDataObject clipboardDataObject = new ClipboardDataObject(clipboard);
			if (dataObject == null)
			{
				dataObject = clipboardDataObject;
			}
			RawDragEvent rawDragEvent = new RawDragEvent(service, (RawDragEventType)type, _parent._inputRoot, position.ToAvaloniaPoint(), dataObject, (DragDropEffects)effects, (RawInputModifiers)modifiers);
			_parent.Input(rawDragEvent);
			return (AvnDragDropEffects)rawDragEvent.Effects;
		}
	}

	protected readonly IAvaloniaNativeFactory _factory;

	protected IInputRoot _inputRoot;

	private IAvnWindowBase _native;

	private object _syncRoot = new object();

	private readonly MouseDevice _mouse;

	private readonly IKeyboardDevice _keyboard;

	private readonly ICursorFactory _cursorFactory;

	private Size _savedLogicalSize;

	private Size _lastRenderedLogicalSize;

	private double _savedScaling;

	private NativeControlHostImpl _nativeControlHost;

	private IStorageProvider _storageProvider;

	private PlatformBehaviorInhibition _platformBehaviorInhibition;

	private WindowTransparencyLevel _transparencyLevel = WindowTransparencyLevel.None;

	public IAvnWindowBase Native => _native;

	public Size ClientSize
	{
		get
		{
			if (_native != null)
			{
				AvnSize clientSize = _native.ClientSize;
				return new Size(clientSize.Width, clientSize.Height);
			}
			return default(Size);
		}
	}

	public unsafe Size? FrameSize
	{
		get
		{
			if (_native != null)
			{
				AvnSize avnSize = default(AvnSize);
				avnSize.Width = -1.0;
				avnSize.Height = -1.0;
				AvnSize avnSize2 = avnSize;
				_native.GetFrameSize(&avnSize2);
				if (!(avnSize2.Width < 0.0) || !(avnSize2.Height < 0.0))
				{
					return new Size(avnSize2.Width, avnSize2.Height);
				}
				return null;
			}
			return null;
		}
	}

	public IEnumerable<object> Surfaces { get; private set; }

	public INativeControlHostImpl NativeControlHost => _nativeControlHost;

	public Action LostFocus { get; set; }

	public Action<Rect> Paint { get; set; }

	public Action<Size, WindowResizeReason> Resized { get; set; }

	public Action Closed { get; set; }

	public IMouseDevice MouseDevice => _mouse;

	public Compositor Compositor => AvaloniaNativePlatform.Compositor;

	public PixelPoint Position
	{
		get
		{
			return _native?.Position.ToAvaloniaPixelPoint() ?? default(PixelPoint);
		}
		set
		{
			_native?.SetPosition(value.ToAvnPoint());
		}
	}

	public Size MaxAutoSizeHint => (from s in Screen.AllScreens
		select s.Bounds.Size.ToSize(1.0) into x
		orderby x.Width + x.Height descending
		select x).FirstOrDefault();

	public double RenderScaling => _native?.Scaling ?? 1.0;

	public double DesktopScaling => 1.0;

	public Action Deactivated { get; set; }

	public Action Activated { get; set; }

	public Action<PixelPoint> PositionChanged { get; set; }

	public Action<RawInputEventArgs> Input { get; set; }

	public Action<double> ScalingChanged { get; set; }

	public Action<WindowTransparencyLevel> TransparencyLevelChanged { get; set; }

	public IScreenImpl Screen { get; private set; }

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

	public AcrylicPlatformCompensationLevels AcrylicCompensationLevels { get; } = new AcrylicPlatformCompensationLevels(1.0, 0.0, 0.0);

	public IPlatformHandle Handle { get; private set; }

	internal WindowBaseImpl(IAvaloniaNativeFactory factory)
	{
		_factory = factory;
		_keyboard = AvaloniaLocator.Current.GetService<IKeyboardDevice>();
		_mouse = new MouseDevice();
		_cursorFactory = AvaloniaLocator.Current.GetService<ICursorFactory>();
	}

	protected void Init(IAvnWindowBase window, IAvnScreens screens)
	{
		_native = window;
		Surfaces = new object[3]
		{
			new GlPlatformSurface(window),
			new MetalPlatformSurface(window),
			this
		};
		Handle = new MacOSTopLevelWindowHandle(window);
		Screen = new ScreenImpl(screens);
		_savedLogicalSize = ClientSize;
		_savedScaling = RenderScaling;
		_nativeControlHost = new NativeControlHostImpl(_native.CreateNativeControlHost());
		_storageProvider = new SystemDialogs(this, _factory.CreateSystemDialogs());
		_platformBehaviorInhibition = new PlatformBehaviorInhibition(_factory.CreatePlatformBehaviorInhibition());
		Screen screen = Screen.AllScreens.OrderBy((Screen x) => x.Scaling).FirstOrDefault((Screen m) => m.Bounds.Contains(Position));
		Resize(new Size((double)screen.WorkingArea.Width * 0.75, (double)screen.WorkingArea.Height * 0.7), WindowResizeReason.Layout);
	}

	IFramebufferRenderTarget IFramebufferPlatformSurface.CreateFramebufferRenderTarget()
	{
		if (!Dispatcher.UIThread.CheckAccess())
		{
			throw new RenderTargetNotReadyException();
		}
		return new FramebufferRenderTarget(this, _native.CreateSoftwareRenderTarget());
	}

	public abstract IPopupImpl CreatePopup();

	public AutomationPeer GetAutomationPeer()
	{
		if (!(_inputRoot is Control element))
		{
			return null;
		}
		return ControlAutomationPeer.CreatePeerForElement(element);
	}

	public void Activate()
	{
		_native?.Activate();
	}

	public bool RawTextInputEvent(ulong timeStamp, string text)
	{
		if (_inputRoot == null)
		{
			return false;
		}
		Dispatcher.UIThread.RunJobs((int)DispatcherPriority.Input + 1);
		RawTextInputEventArgs rawTextInputEventArgs = new RawTextInputEventArgs(_keyboard, timeStamp, _inputRoot, text);
		Input?.Invoke(rawTextInputEventArgs);
		return rawTextInputEventArgs.Handled;
	}

	public bool RawKeyEvent(AvnRawKeyEventType type, ulong timeStamp, AvnInputModifiers modifiers, uint key)
	{
		if (_inputRoot == null)
		{
			return false;
		}
		Dispatcher.UIThread.RunJobs((int)DispatcherPriority.Input + 1);
		RawKeyEventArgs rawKeyEventArgs = new RawKeyEventArgs(_keyboard, timeStamp, _inputRoot, (RawKeyEventType)type, (Key)key, (RawInputModifiers)modifiers);
		Input?.Invoke(rawKeyEventArgs);
		return rawKeyEventArgs.Handled;
	}

	protected virtual bool ChromeHitTest(RawPointerEventArgs e)
	{
		return false;
	}

	public void RawMouseEvent(AvnRawMouseEventType type, ulong timeStamp, AvnInputModifiers modifiers, AvnPoint point, AvnVector delta)
	{
		if (_inputRoot == null)
		{
			return;
		}
		Dispatcher.UIThread.RunJobs((int)DispatcherPriority.Input + 1);
		switch (type)
		{
		case AvnRawMouseEventType.Wheel:
			Input?.Invoke(new RawMouseWheelEventArgs(_mouse, timeStamp, _inputRoot, point.ToAvaloniaPoint(), new Vector(delta.X, delta.Y), (RawInputModifiers)modifiers));
			return;
		case AvnRawMouseEventType.Magnify:
			Input?.Invoke(new RawPointerGestureEventArgs(_mouse, timeStamp, _inputRoot, RawPointerEventType.Magnify, point.ToAvaloniaPoint(), new Vector(delta.X, delta.Y), (RawInputModifiers)modifiers));
			return;
		case AvnRawMouseEventType.Rotate:
			Input?.Invoke(new RawPointerGestureEventArgs(_mouse, timeStamp, _inputRoot, RawPointerEventType.Rotate, point.ToAvaloniaPoint(), new Vector(delta.X, delta.Y), (RawInputModifiers)modifiers));
			return;
		case AvnRawMouseEventType.Swipe:
			Input?.Invoke(new RawPointerGestureEventArgs(_mouse, timeStamp, _inputRoot, RawPointerEventType.Swipe, point.ToAvaloniaPoint(), new Vector(delta.X, delta.Y), (RawInputModifiers)modifiers));
			return;
		}
		RawPointerEventArgs rawPointerEventArgs = new RawPointerEventArgs(_mouse, timeStamp, _inputRoot, (RawPointerEventType)type, point.ToAvaloniaPoint(), (RawInputModifiers)modifiers);
		if (!ChromeHitTest(rawPointerEventArgs))
		{
			Input?.Invoke(rawPointerEventArgs);
		}
	}

	public void Resize(Size clientSize, WindowResizeReason reason)
	{
		_native?.Resize(clientSize.Width, clientSize.Height, (AvnPlatformResizeReason)reason);
	}

	public virtual void Dispose()
	{
		_native?.Close();
		_native?.Dispose();
		_native = null;
		_nativeControlHost?.Dispose();
		_nativeControlHost = null;
		(Screen as ScreenImpl)?.Dispose();
		_mouse.Dispose();
	}

	public void Invalidate(Rect rect)
	{
		_native?.Invalidate(new AvnRect
		{
			Height = rect.Height,
			Width = rect.Width,
			X = rect.X,
			Y = rect.Y
		});
	}

	public void SetInputRoot(IInputRoot inputRoot)
	{
		_inputRoot = inputRoot;
	}

	public virtual void Show(bool activate, bool isDialog)
	{
		_native?.Show(activate.AsComBool(), isDialog.AsComBool());
	}

	public Point PointToClient(PixelPoint point)
	{
		return _native?.PointToClient(point.ToAvnPoint()).ToAvaloniaPoint() ?? default(Point);
	}

	public PixelPoint PointToScreen(Point point)
	{
		return _native?.PointToScreen(point.ToAvnPoint()).ToAvaloniaPixelPoint() ?? default(PixelPoint);
	}

	public void Hide()
	{
		_native?.Hide();
	}

	public void BeginMoveDrag(PointerPressedEventArgs e)
	{
		_native?.BeginMoveDrag();
	}

	public void SetTopmost(bool value)
	{
		_native?.SetTopMost(value.AsComBool());
	}

	public void SetCursor(ICursorImpl cursor)
	{
		if (_native != null)
		{
			AvaloniaNativeCursor avaloniaNativeCursor = cursor as AvaloniaNativeCursor;
			avaloniaNativeCursor = avaloniaNativeCursor ?? (_cursorFactory.GetCursor(StandardCursorType.Arrow) as AvaloniaNativeCursor);
			_native.SetCursor(avaloniaNativeCursor.Cursor);
		}
	}

	public void SetMinMaxSize(Size minSize, Size maxSize)
	{
		_native?.SetMinMaxSize(minSize.ToAvnSize(), maxSize.ToAvnSize());
	}

	public void BeginResizeDrag(WindowEdge edge, PointerPressedEventArgs e)
	{
	}

	internal void BeginDraggingSession(AvnDragDropEffects effects, AvnPoint point, IAvnClipboard clipboard, IAvnDndResultCallback callback, IntPtr sourceHandle)
	{
		_native?.BeginDragAndDropOperation(effects, point, clipboard, callback, sourceHandle);
	}

	public void SetTransparencyLevelHint(IReadOnlyList<WindowTransparencyLevel> transparencyLevels)
	{
		foreach (WindowTransparencyLevel transparencyLevel in transparencyLevels)
		{
			AvnWindowTransparencyMode? avnWindowTransparencyMode = null;
			if (transparencyLevel == WindowTransparencyLevel.None)
			{
				avnWindowTransparencyMode = AvnWindowTransparencyMode.Opaque;
			}
			if (transparencyLevel == WindowTransparencyLevel.Transparent)
			{
				avnWindowTransparencyMode = AvnWindowTransparencyMode.Transparent;
			}
			else if (transparencyLevel == WindowTransparencyLevel.AcrylicBlur)
			{
				avnWindowTransparencyMode = AvnWindowTransparencyMode.Blur;
			}
			if (avnWindowTransparencyMode.HasValue && transparencyLevel != TransparencyLevel)
			{
				_native?.SetTransparencyMode(avnWindowTransparencyMode.Value);
				TransparencyLevel = transparencyLevel;
				return;
			}
		}
		if (TransparencyLevel != WindowTransparencyLevel.None)
		{
			_native?.SetTransparencyMode(AvnWindowTransparencyMode.Opaque);
			TransparencyLevel = WindowTransparencyLevel.None;
		}
	}

	public void SetFrameThemeVariant(PlatformThemeVariant themeVariant)
	{
		_native.SetFrameThemeVariant((AvnPlatformThemeVariant)themeVariant);
	}

	public virtual object TryGetFeature(Type featureType)
	{
		if (featureType == typeof(INativeControlHostImpl))
		{
			return _nativeControlHost;
		}
		if (featureType == typeof(IStorageProvider))
		{
			return _storageProvider;
		}
		if (featureType == typeof(IPlatformBehaviorInhibition))
		{
			return _platformBehaviorInhibition;
		}
		if (featureType == typeof(IClipboard))
		{
			return AvaloniaLocator.Current.GetRequiredService<IClipboard>();
		}
		return null;
	}
}
