using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Automation.Peers;
using Avalonia.Controls.Platform;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Reactive;

namespace Avalonia.Controls;

public class Window : WindowBase, IFocusScope, ILayoutRoot
{
	private readonly List<(Window child, bool isDialog)> _children = new List<(Window, bool)>();

	private bool _isExtendedIntoWindowDecorations;

	private Thickness _windowDecorationMargin;

	private Thickness _offScreenMargin;

	public static readonly StyledProperty<SizeToContent> SizeToContentProperty;

	public static readonly StyledProperty<bool> ExtendClientAreaToDecorationsHintProperty;

	public static readonly StyledProperty<ExtendClientAreaChromeHints> ExtendClientAreaChromeHintsProperty;

	public static readonly StyledProperty<double> ExtendClientAreaTitleBarHeightHintProperty;

	public static readonly DirectProperty<Window, bool> IsExtendedIntoWindowDecorationsProperty;

	public static readonly DirectProperty<Window, Thickness> WindowDecorationMarginProperty;

	public static readonly DirectProperty<Window, Thickness> OffScreenMarginProperty;

	public static readonly StyledProperty<SystemDecorations> SystemDecorationsProperty;

	public static readonly StyledProperty<bool> ShowActivatedProperty;

	public static readonly StyledProperty<bool> ShowInTaskbarProperty;

	public static readonly StyledProperty<WindowState> WindowStateProperty;

	public static readonly StyledProperty<string?> TitleProperty;

	public static readonly StyledProperty<WindowIcon?> IconProperty;

	public static readonly StyledProperty<WindowStartupLocation> WindowStartupLocationProperty;

	public static readonly StyledProperty<bool> CanResizeProperty;

	public static readonly RoutedEvent<RoutedEventArgs> WindowClosedEvent;

	public static readonly RoutedEvent<RoutedEventArgs> WindowOpenedEvent;

	private object? _dialogResult;

	private readonly Size _maxPlatformClientSize;

	private bool _shown;

	private bool _showingAsDialog;

	private bool _wasShownBefore;

	public new IWindowImpl? PlatformImpl => (IWindowImpl)base.PlatformImpl;

	public IReadOnlyList<Window> OwnedWindows => _children.Select<(Window, bool), Window>(((Window child, bool isDialog) x) => x.child).ToArray();

	public SizeToContent SizeToContent
	{
		get
		{
			return GetValue(SizeToContentProperty);
		}
		set
		{
			SetValue(SizeToContentProperty, value);
		}
	}

	public string? Title
	{
		get
		{
			return GetValue(TitleProperty);
		}
		set
		{
			SetValue(TitleProperty, value);
		}
	}

	public bool ExtendClientAreaToDecorationsHint
	{
		get
		{
			return GetValue(ExtendClientAreaToDecorationsHintProperty);
		}
		set
		{
			SetValue(ExtendClientAreaToDecorationsHintProperty, value);
		}
	}

	public ExtendClientAreaChromeHints ExtendClientAreaChromeHints
	{
		get
		{
			return GetValue(ExtendClientAreaChromeHintsProperty);
		}
		set
		{
			SetValue(ExtendClientAreaChromeHintsProperty, value);
		}
	}

	public double ExtendClientAreaTitleBarHeightHint
	{
		get
		{
			return GetValue(ExtendClientAreaTitleBarHeightHintProperty);
		}
		set
		{
			SetValue(ExtendClientAreaTitleBarHeightHintProperty, value);
		}
	}

	public bool IsExtendedIntoWindowDecorations
	{
		get
		{
			return _isExtendedIntoWindowDecorations;
		}
		private set
		{
			SetAndRaise(IsExtendedIntoWindowDecorationsProperty, ref _isExtendedIntoWindowDecorations, value);
		}
	}

	public Thickness WindowDecorationMargin
	{
		get
		{
			return _windowDecorationMargin;
		}
		private set
		{
			SetAndRaise(WindowDecorationMarginProperty, ref _windowDecorationMargin, value);
		}
	}

	public Thickness OffScreenMargin
	{
		get
		{
			return _offScreenMargin;
		}
		private set
		{
			SetAndRaise(OffScreenMarginProperty, ref _offScreenMargin, value);
		}
	}

	public SystemDecorations SystemDecorations
	{
		get
		{
			return GetValue(SystemDecorationsProperty);
		}
		set
		{
			SetValue(SystemDecorationsProperty, value);
		}
	}

	public bool ShowActivated
	{
		get
		{
			return GetValue(ShowActivatedProperty);
		}
		set
		{
			SetValue(ShowActivatedProperty, value);
		}
	}

	public bool ShowInTaskbar
	{
		get
		{
			return GetValue(ShowInTaskbarProperty);
		}
		set
		{
			SetValue(ShowInTaskbarProperty, value);
		}
	}

	public WindowState WindowState
	{
		get
		{
			return GetValue(WindowStateProperty);
		}
		set
		{
			SetValue(WindowStateProperty, value);
		}
	}

	public bool CanResize
	{
		get
		{
			return GetValue(CanResizeProperty);
		}
		set
		{
			SetValue(CanResizeProperty, value);
		}
	}

	public WindowIcon? Icon
	{
		get
		{
			return GetValue(IconProperty);
		}
		set
		{
			SetValue(IconProperty, value);
		}
	}

	public WindowStartupLocation WindowStartupLocation
	{
		get
		{
			return GetValue(WindowStartupLocationProperty);
		}
		set
		{
			SetValue(WindowStartupLocationProperty, value);
		}
	}

	public PixelPoint Position
	{
		get
		{
			return PlatformImpl?.Position ?? PixelPoint.Origin;
		}
		set
		{
			PlatformImpl?.Move(value);
		}
	}

	protected override Type StyleKeyOverride => typeof(Window);

	public event EventHandler<WindowClosingEventArgs>? Closing;

	static Window()
	{
		SizeToContentProperty = AvaloniaProperty.Register<Window, SizeToContent>("SizeToContent", SizeToContent.Manual);
		ExtendClientAreaToDecorationsHintProperty = AvaloniaProperty.Register<Window, bool>("ExtendClientAreaToDecorationsHint", defaultValue: false);
		ExtendClientAreaChromeHintsProperty = AvaloniaProperty.Register<Window, ExtendClientAreaChromeHints>("ExtendClientAreaChromeHints", ExtendClientAreaChromeHints.Default);
		ExtendClientAreaTitleBarHeightHintProperty = AvaloniaProperty.Register<Window, double>("ExtendClientAreaTitleBarHeightHint", -1.0);
		IsExtendedIntoWindowDecorationsProperty = AvaloniaProperty.RegisterDirect("IsExtendedIntoWindowDecorations", (Window o) => o.IsExtendedIntoWindowDecorations, null, unsetValue: false);
		WindowDecorationMarginProperty = AvaloniaProperty.RegisterDirect("WindowDecorationMargin", (Window o) => o.WindowDecorationMargin);
		OffScreenMarginProperty = AvaloniaProperty.RegisterDirect("OffScreenMargin", (Window o) => o.OffScreenMargin);
		SystemDecorationsProperty = AvaloniaProperty.Register<Window, SystemDecorations>("SystemDecorations", SystemDecorations.Full);
		ShowActivatedProperty = AvaloniaProperty.Register<Window, bool>("ShowActivated", defaultValue: true);
		ShowInTaskbarProperty = AvaloniaProperty.Register<Window, bool>("ShowInTaskbar", defaultValue: true);
		WindowStateProperty = AvaloniaProperty.Register<Window, WindowState>("WindowState", WindowState.Normal);
		TitleProperty = AvaloniaProperty.Register<Window, string>("Title", "Window");
		IconProperty = AvaloniaProperty.Register<Window, WindowIcon>("Icon");
		WindowStartupLocationProperty = AvaloniaProperty.Register<Window, WindowStartupLocation>("WindowStartupLocation", WindowStartupLocation.Manual);
		CanResizeProperty = AvaloniaProperty.Register<Window, bool>("CanResize", defaultValue: true);
		WindowClosedEvent = RoutedEvent.Register<Window, RoutedEventArgs>("WindowClosed", RoutingStrategies.Direct);
		WindowOpenedEvent = RoutedEvent.Register<Window, RoutedEventArgs>("WindowOpened", RoutingStrategies.Direct);
		TemplatedControl.BackgroundProperty.OverrideDefaultValue(typeof(Window), Brushes.White);
		TitleProperty.Changed.AddClassHandler(delegate(Window s, AvaloniaPropertyChangedEventArgs e)
		{
			s.PlatformImpl?.SetTitle((string)e.NewValue);
		});
		ShowInTaskbarProperty.Changed.AddClassHandler(delegate(Window w, AvaloniaPropertyChangedEventArgs e)
		{
			w.PlatformImpl?.ShowTaskbarIcon((bool)e.NewValue);
		});
		IconProperty.Changed.AddClassHandler(delegate(Window s, AvaloniaPropertyChangedEventArgs e)
		{
			s.PlatformImpl?.SetIcon(((WindowIcon)e.NewValue)?.PlatformImpl);
		});
		CanResizeProperty.Changed.AddClassHandler(delegate(Window w, AvaloniaPropertyChangedEventArgs e)
		{
			w.PlatformImpl?.CanResize((bool)e.NewValue);
		});
		WindowStateProperty.Changed.AddClassHandler(delegate(Window w, AvaloniaPropertyChangedEventArgs e)
		{
			if (w.PlatformImpl != null)
			{
				w.PlatformImpl.WindowState = (WindowState)e.NewValue;
			}
		});
		ExtendClientAreaToDecorationsHintProperty.Changed.AddClassHandler(delegate(Window w, AvaloniaPropertyChangedEventArgs e)
		{
			if (w.PlatformImpl != null)
			{
				w.PlatformImpl.SetExtendClientAreaToDecorationsHint((bool)e.NewValue);
			}
		});
		ExtendClientAreaChromeHintsProperty.Changed.AddClassHandler(delegate(Window w, AvaloniaPropertyChangedEventArgs e)
		{
			if (w.PlatformImpl != null)
			{
				w.PlatformImpl.SetExtendClientAreaChromeHints((ExtendClientAreaChromeHints)e.NewValue);
			}
		});
		ExtendClientAreaTitleBarHeightHintProperty.Changed.AddClassHandler(delegate(Window w, AvaloniaPropertyChangedEventArgs e)
		{
			if (w.PlatformImpl != null)
			{
				w.PlatformImpl.SetExtendClientAreaTitleBarHeightHint((double)e.NewValue);
			}
		});
		Layoutable.MinWidthProperty.Changed.AddClassHandler(delegate(Window w, AvaloniaPropertyChangedEventArgs e)
		{
			w.PlatformImpl?.SetMinMaxSize(new Size((double)e.NewValue, w.MinHeight), new Size(w.MaxWidth, w.MaxHeight));
		});
		Layoutable.MinHeightProperty.Changed.AddClassHandler(delegate(Window w, AvaloniaPropertyChangedEventArgs e)
		{
			w.PlatformImpl?.SetMinMaxSize(new Size(w.MinWidth, (double)e.NewValue), new Size(w.MaxWidth, w.MaxHeight));
		});
		Layoutable.MaxWidthProperty.Changed.AddClassHandler(delegate(Window w, AvaloniaPropertyChangedEventArgs e)
		{
			w.PlatformImpl?.SetMinMaxSize(new Size(w.MinWidth, w.MinHeight), new Size((double)e.NewValue, w.MaxHeight));
		});
		Layoutable.MaxHeightProperty.Changed.AddClassHandler(delegate(Window w, AvaloniaPropertyChangedEventArgs e)
		{
			w.PlatformImpl?.SetMinMaxSize(new Size(w.MinWidth, w.MinHeight), new Size(w.MaxWidth, (double)e.NewValue));
		});
	}

	public Window()
		: this(PlatformManager.CreateWindow())
	{
	}

	public Window(IWindowImpl impl)
		: base(impl)
	{
		impl.Closing = HandleClosing;
		impl.GotInputWhenDisabled = OnGotInputWhenDisabled;
		impl.WindowStateChanged = HandleWindowStateChanged;
		_maxPlatformClientSize = PlatformImpl?.MaxAutoSizeHint ?? default(Size);
		impl.ExtendClientAreaToDecorationsChanged = ExtendClientAreaToDecorationsChanged;
		this.GetObservable(TopLevel.ClientSizeProperty).Skip(1).Subscribe(delegate(Size x)
		{
			PlatformImpl?.Resize(x);
		});
		PlatformImpl?.ShowTaskbarIcon(ShowInTaskbar);
	}

	public void BeginMoveDrag(PointerPressedEventArgs e)
	{
		PlatformImpl?.BeginMoveDrag(e);
	}

	public void BeginResizeDrag(WindowEdge edge, PointerPressedEventArgs e)
	{
		PlatformImpl?.BeginResizeDrag(edge, e);
	}

	public void Close()
	{
		CloseCore(WindowCloseReason.WindowClosing, isProgrammatic: true);
	}

	public void Close(object? dialogResult)
	{
		_dialogResult = dialogResult;
		CloseCore(WindowCloseReason.WindowClosing, isProgrammatic: true);
	}

	internal void CloseCore(WindowCloseReason reason, bool isProgrammatic)
	{
		bool flag = true;
		try
		{
			if (ShouldCancelClose(new WindowClosingEventArgs(reason, isProgrammatic)))
			{
				flag = false;
			}
		}
		finally
		{
			if (flag)
			{
				CloseInternal();
			}
		}
	}

	private protected virtual bool HandleClosing(WindowCloseReason reason)
	{
		if (!ShouldCancelClose(new WindowClosingEventArgs(reason, isProgrammatic: false)))
		{
			CloseInternal();
			return false;
		}
		return true;
	}

	private void CloseInternal()
	{
		(Window, bool)[] array = _children.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Item1.CloseInternal();
		}
		if (base.Owner is Window window)
		{
			window.RemoveChild(this);
		}
		base.Owner = null;
		PlatformImpl?.Dispose();
		_showingAsDialog = false;
	}

	private bool ShouldCancelClose(WindowClosingEventArgs args)
	{
		bool flag = true;
		if (_children.Count > 0)
		{
			WindowClosingEventArgs args2 = ((args.CloseReason == WindowCloseReason.WindowClosing) ? new WindowClosingEventArgs(WindowCloseReason.OwnerWindowClosing, args.IsProgrammatic) : args);
			(Window, bool)[] array = _children.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Item1.ShouldCancelClose(args2))
				{
					flag = false;
				}
			}
		}
		if (flag)
		{
			OnClosing(args);
			return args.Cancel;
		}
		return true;
	}

	private void HandleWindowStateChanged(WindowState state)
	{
		WindowState = state;
		if (state == WindowState.Minimized)
		{
			StopRendering();
		}
		else
		{
			StartRendering();
		}
	}

	protected virtual void ExtendClientAreaToDecorationsChanged(bool isExtended)
	{
		IsExtendedIntoWindowDecorations = isExtended;
		WindowDecorationMargin = PlatformImpl?.ExtendedMargins ?? default(Thickness);
		OffScreenMargin = PlatformImpl?.OffScreenMargin ?? default(Thickness);
	}

	public override void Hide()
	{
		using (FreezeVisibilityChangeHandling())
		{
			if (!_shown)
			{
				return;
			}
			StopRendering();
			if (base.Owner is Window window)
			{
				window.RemoveChild(this);
			}
			if (_children.Count > 0)
			{
				(Window, bool)[] array = _children.ToArray();
				for (int i = 0; i < array.Length; i++)
				{
					array[i].Item1.Hide();
				}
			}
			base.Owner = null;
			PlatformImpl?.Hide();
			base.IsVisible = false;
			_shown = false;
		}
	}

	public override void Show()
	{
		ShowCore(null);
	}

	protected override void IsVisibleChanged(AvaloniaPropertyChangedEventArgs e)
	{
		if (base.IgnoreVisibilityChanges)
		{
			return;
		}
		bool newValue = e.GetNewValue<bool>();
		if (_shown != newValue)
		{
			if (!_shown)
			{
				Show();
			}
			else if (_showingAsDialog)
			{
				Close(false);
			}
			else
			{
				Hide();
			}
		}
	}

	public void Show(Window owner)
	{
		if (owner == null)
		{
			throw new ArgumentNullException("owner", "Showing a child window requires valid parent.");
		}
		ShowCore(owner);
	}

	private void EnsureStateBeforeShow()
	{
		if (PlatformImpl == null)
		{
			throw new InvalidOperationException("Cannot re-show a closed window.");
		}
	}

	private void EnsureParentStateBeforeShow(Window owner)
	{
		if (owner.PlatformImpl == null)
		{
			throw new InvalidOperationException("Cannot show a window with a closed owner.");
		}
		if (owner == this)
		{
			throw new InvalidOperationException("A Window cannot be its own owner.");
		}
		if (!owner.IsVisible)
		{
			throw new InvalidOperationException("Cannot show window with non-visible owner.");
		}
	}

	private void ShowCore(Window? owner)
	{
		using (FreezeVisibilityChangeHandling())
		{
			EnsureStateBeforeShow();
			if (owner != null)
			{
				EnsureParentStateBeforeShow(owner);
			}
			if (!_shown)
			{
				RaiseEvent(new RoutedEventArgs(WindowOpenedEvent));
				EnsureInitialized();
				ApplyStyling();
				_shown = true;
				base.IsVisible = true;
				Size size = new Size(double.IsNaN(base.Width) ? Math.Max(base.MinWidth, base.ClientSize.Width) : base.Width, double.IsNaN(base.Height) ? Math.Max(base.MinHeight, base.ClientSize.Height) : base.Height);
				if (size != base.ClientSize)
				{
					PlatformImpl?.Resize(size, WindowResizeReason.Layout);
				}
				base.LayoutManager.ExecuteInitialLayoutPass();
				if (PlatformImpl != null && owner?.PlatformImpl != null)
				{
					PlatformImpl.SetParent(owner.PlatformImpl);
				}
				base.Owner = owner;
				owner?.AddChild(this, isDialog: false);
				SetWindowStartupLocation(owner);
				StartRendering();
				PlatformImpl?.Show(ShowActivated, isDialog: false);
				OnOpened(EventArgs.Empty);
				_wasShownBefore = true;
			}
		}
	}

	public Task ShowDialog(Window owner)
	{
		return ShowDialog<object>(owner);
	}

	public Task<TResult> ShowDialog<TResult>(Window owner)
	{
		using (FreezeVisibilityChangeHandling())
		{
			EnsureStateBeforeShow();
			if (owner == null)
			{
				throw new ArgumentNullException("owner");
			}
			EnsureParentStateBeforeShow(owner);
			if (_shown)
			{
				throw new InvalidOperationException("The window is already being shown.");
			}
			RaiseEvent(new RoutedEventArgs(WindowOpenedEvent));
			EnsureInitialized();
			ApplyStyling();
			_shown = true;
			_showingAsDialog = true;
			base.IsVisible = true;
			Size size = new Size(double.IsNaN(base.Width) ? base.ClientSize.Width : base.Width, double.IsNaN(base.Height) ? base.ClientSize.Height : base.Height);
			if (size != base.ClientSize)
			{
				PlatformImpl?.Resize(size, WindowResizeReason.Layout);
			}
			base.LayoutManager.ExecuteInitialLayoutPass();
			TaskCompletionSource<TResult> result = new TaskCompletionSource<TResult>();
			PlatformImpl?.SetParent(owner.PlatformImpl);
			base.Owner = owner;
			owner.AddChild(this, isDialog: true);
			SetWindowStartupLocation(owner);
			StartRendering();
			PlatformImpl?.Show(ShowActivated, isDialog: true);
			Observable.FromEventPattern(delegate(EventHandler x)
			{
				base.Closed += x;
			}, delegate(EventHandler x)
			{
				base.Closed -= x;
			}).Take(1).Subscribe(delegate
			{
				owner.Activate();
				result.SetResult((TResult)(_dialogResult ?? ((object)default(TResult))));
			});
			OnOpened(EventArgs.Empty);
			return result.Task;
		}
	}

	private void UpdateEnabled()
	{
		bool enabled = true;
		foreach (var child in _children)
		{
			if (child.isDialog)
			{
				enabled = false;
				break;
			}
		}
		PlatformImpl?.SetEnabled(enabled);
	}

	private void AddChild(Window window, bool isDialog)
	{
		_children.Add((window, isDialog));
		UpdateEnabled();
	}

	private void RemoveChild(Window window)
	{
		for (int num = _children.Count - 1; num >= 0; num--)
		{
			if (_children[num].child == window)
			{
				_children.RemoveAt(num);
			}
		}
		UpdateEnabled();
	}

	private void OnGotInputWhenDisabled()
	{
		Window window = null;
		foreach (var child in _children)
		{
			var (window2, _) = child;
			if (child.isDialog)
			{
				window = window2;
				break;
			}
		}
		if (window != null)
		{
			window.OnGotInputWhenDisabled();
		}
		else
		{
			Activate();
		}
	}

	private void SetWindowStartupLocation(Window? owner = null)
	{
		if (_wasShownBefore)
		{
			return;
		}
		WindowStartupLocation windowStartupLocation = WindowStartupLocation;
		if (windowStartupLocation == WindowStartupLocation.CenterOwner && (owner == null || base.Owner is Window { WindowState: WindowState.Minimized }))
		{
			windowStartupLocation = WindowStartupLocation.CenterScreen;
		}
		double scale = owner?.DesktopScaling ?? PlatformImpl?.DesktopScaling ?? 1.0;
		PixelRect rect = (base.FrameSize.HasValue ? new PixelRect(PixelSize.FromSize(base.FrameSize.Value, scale)) : new PixelRect(PixelSize.FromSize(base.ClientSize, scale)));
		switch (windowStartupLocation)
		{
		case WindowStartupLocation.CenterScreen:
		{
			Screen screen = null;
			if (owner != null)
			{
				screen = base.Screens.ScreenFromWindow(owner) ?? base.Screens.ScreenFromPoint(owner.Position);
			}
			if (screen == null)
			{
				screen = base.Screens.ScreenFromPoint(Position);
			}
			if (screen != null)
			{
				Position = screen.WorkingArea.CenterRect(rect).Position;
			}
			break;
		}
		case WindowStartupLocation.CenterOwner:
		{
			Size size = owner.FrameSize ?? owner.ClientSize;
			Position = new PixelRect(owner.Position, PixelSize.FromSize(size, scale)).CenterRect(rect).Position;
			break;
		}
		}
	}

	protected override Size MeasureOverride(Size availableSize)
	{
		SizeToContent sizeToContent = SizeToContent;
		Size clientSize = base.ClientSize;
		Size availableSize2 = clientSize;
		Size size = PlatformImpl?.MaxAutoSizeHint ?? Size.Infinity;
		if (base.MaxWidth > 0.0 && base.MaxWidth < size.Width)
		{
			size = size.WithWidth(base.MaxWidth);
		}
		if (base.MaxHeight > 0.0 && base.MaxHeight < size.Height)
		{
			size = size.WithHeight(base.MaxHeight);
		}
		if (sizeToContent.HasAllFlags(SizeToContent.Width))
		{
			availableSize2 = availableSize2.WithWidth(size.Width);
		}
		if (sizeToContent.HasAllFlags(SizeToContent.Height))
		{
			availableSize2 = availableSize2.WithHeight(size.Height);
		}
		Size result = base.MeasureOverride(availableSize2);
		if (!sizeToContent.HasAllFlags(SizeToContent.Width))
		{
			result = (double.IsInfinity(availableSize.Width) ? result.WithWidth(clientSize.Width) : result.WithWidth(availableSize.Width));
		}
		if (!sizeToContent.HasAllFlags(SizeToContent.Height))
		{
			result = (double.IsInfinity(availableSize.Height) ? result.WithHeight(clientSize.Height) : result.WithHeight(availableSize.Height));
		}
		return result;
	}

	protected sealed override Size ArrangeSetBounds(Size size)
	{
		PlatformImpl?.Resize(size, WindowResizeReason.Layout);
		return base.ClientSize;
	}

	private protected sealed override void HandleClosed()
	{
		RaiseEvent(new RoutedEventArgs(WindowClosedEvent));
		base.HandleClosed();
		if (base.Owner is Window window)
		{
			window.RemoveChild(this);
		}
		base.Owner = null;
	}

	internal override void HandleResized(Size clientSize, WindowResizeReason reason)
	{
		if (base.ClientSize != clientSize || double.IsNaN(base.Width) || double.IsNaN(base.Height))
		{
			SizeToContent sizeToContent = SizeToContent;
			if ((sizeToContent != 0 && CanResize && reason == WindowResizeReason.Unspecified) || reason == WindowResizeReason.User)
			{
				if (clientSize.Width != base.ClientSize.Width)
				{
					sizeToContent &= ~SizeToContent.Width;
				}
				if (clientSize.Height != base.ClientSize.Height)
				{
					sizeToContent &= ~SizeToContent.Height;
				}
				SizeToContent = sizeToContent;
			}
			base.Width = clientSize.Width;
			base.Height = clientSize.Height;
		}
		base.HandleResized(clientSize, reason);
	}

	protected virtual void OnClosing(WindowClosingEventArgs e)
	{
		this.Closing?.Invoke(this, e);
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (change.Property == SystemDecorationsProperty)
		{
			SystemDecorations item = change.GetOldAndNewValue<SystemDecorations>().newValue;
			PlatformImpl?.SetSystemDecorations(item);
		}
	}

	protected override AutomationPeer OnCreateAutomationPeer()
	{
		return new WindowAutomationPeer(this);
	}
}
