using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Platform;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Input.Raw;
using Avalonia.Input.TextInput;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Logging;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using Avalonia.Reactive;
using Avalonia.Rendering;
using Avalonia.Rendering.Composition;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.Utilities;

namespace Avalonia.Controls;

[TemplatePart("PART_TransparencyFallback", typeof(Border))]
public abstract class TopLevel : ContentControl, IInputRoot, IInputElement, ILayoutRoot, IRenderRoot, ICloseable, IStyleHost, ILogicalRoot, ILogical, ITextInputMethodRoot
{
	private sealed class LayoutDiagnosticBridge : IDisposable
	{
		private readonly RendererDiagnostics _diagnostics;

		private readonly LayoutManager _layoutManager;

		private bool _isHandling;

		public LayoutDiagnosticBridge(RendererDiagnostics diagnostics, LayoutManager layoutManager)
		{
			_diagnostics = diagnostics;
			_layoutManager = layoutManager;
			diagnostics.PropertyChanged += OnDiagnosticsPropertyChanged;
		}

		public void SetupBridge()
		{
			bool flag = (_diagnostics.DebugOverlays & RendererDebugOverlays.LayoutTimeGraph) != 0;
			if (flag != _isHandling)
			{
				_isHandling = flag;
				_layoutManager.LayoutPassTimed = (flag ? ((Action<LayoutPassTiming>)delegate(LayoutPassTiming timing)
				{
					_diagnostics.LastLayoutPassTiming = timing;
				}) : null);
			}
		}

		private void OnDiagnosticsPropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "DebugOverlays")
			{
				SetupBridge();
			}
		}

		public void Dispose()
		{
			_diagnostics.PropertyChanged -= OnDiagnosticsPropertyChanged;
			_layoutManager.LayoutPassTimed = null;
		}
	}

	public static readonly DirectProperty<TopLevel, Size> ClientSizeProperty;

	public static readonly DirectProperty<TopLevel, Size?> FrameSizeProperty;

	public static readonly StyledProperty<IInputElement?> PointerOverElementProperty;

	public static readonly StyledProperty<IReadOnlyList<WindowTransparencyLevel>> TransparencyLevelHintProperty;

	public static readonly DirectProperty<TopLevel, WindowTransparencyLevel> ActualTransparencyLevelProperty;

	public static readonly StyledProperty<IBrush> TransparencyBackgroundFallbackProperty;

	public static readonly StyledProperty<ThemeVariant> ActualThemeVariantProperty;

	public static readonly StyledProperty<ThemeVariant?> RequestedThemeVariantProperty;

	public static readonly AttachedProperty<SolidColorBrush?> SystemBarColorProperty;

	public static readonly RoutedEvent<RoutedEventArgs> BackRequestedEvent;

	private static readonly WeakEvent<IResourceHost, ResourcesChangedEventArgs> ResourcesChangedWeakEvent;

	private readonly IInputManager? _inputManager;

	private readonly IAccessKeyHandler? _accessKeyHandler;

	private readonly IKeyboardNavigationHandler? _keyboardNavigationHandler;

	private readonly IGlobalStyles? _globalStyles;

	private readonly IThemeVariantHost? _applicationThemeHost;

	private readonly PointerOverPreProcessor? _pointerOverPreProcessor;

	private readonly IDisposable? _pointerOverPreProcessorSubscription;

	private readonly IDisposable? _backGestureSubscription;

	private Size _clientSize;

	private Size? _frameSize;

	private WindowTransparencyLevel _actualTransparencyLevel;

	private ILayoutManager? _layoutManager;

	private Border? _transparencyFallbackBorder;

	private TargetWeakEventSubscriber<TopLevel, ResourcesChangedEventArgs>? _resourcesChangesSubscriber;

	private IStorageProvider? _storageProvider;

	private LayoutDiagnosticBridge? _layoutDiagnosticBridge;

	internal IHitTester? HitTesterOverride;

	public Size ClientSize
	{
		get
		{
			return _clientSize;
		}
		protected set
		{
			SetAndRaise(ClientSizeProperty, ref _clientSize, value);
		}
	}

	public Size? FrameSize
	{
		get
		{
			return _frameSize;
		}
		protected set
		{
			SetAndRaise(FrameSizeProperty, ref _frameSize, value);
		}
	}

	public IReadOnlyList<WindowTransparencyLevel> TransparencyLevelHint
	{
		get
		{
			return GetValue(TransparencyLevelHintProperty);
		}
		set
		{
			SetValue(TransparencyLevelHintProperty, value);
		}
	}

	public WindowTransparencyLevel ActualTransparencyLevel
	{
		get
		{
			return _actualTransparencyLevel;
		}
		private set
		{
			SetAndRaise(ActualTransparencyLevelProperty, ref _actualTransparencyLevel, value);
		}
	}

	public IBrush TransparencyBackgroundFallback
	{
		get
		{
			return GetValue(TransparencyBackgroundFallbackProperty);
		}
		set
		{
			SetValue(TransparencyBackgroundFallbackProperty, value);
		}
	}

	public ThemeVariant? RequestedThemeVariant
	{
		get
		{
			return GetValue(RequestedThemeVariantProperty);
		}
		set
		{
			SetValue(RequestedThemeVariantProperty, value);
		}
	}

	internal ILayoutManager LayoutManager
	{
		get
		{
			if (_layoutManager == null)
			{
				_layoutManager = CreateLayoutManager();
				if (_layoutManager is LayoutManager layoutManager)
				{
					_layoutDiagnosticBridge = new LayoutDiagnosticBridge(Renderer.Diagnostics, layoutManager);
					_layoutDiagnosticBridge.SetupBridge();
				}
			}
			return _layoutManager;
		}
	}

	ILayoutManager ILayoutRoot.LayoutManager => LayoutManager;

	public ITopLevelImpl? PlatformImpl { get; private set; }

	internal CompositingRenderer Renderer { get; }

	internal IHitTester HitTester => HitTesterOverride ?? Renderer;

	IRenderer IRenderRoot.Renderer => Renderer;

	IHitTester IRenderRoot.HitTester => HitTester;

	public RendererDiagnostics RendererDiagnostics => Renderer.Diagnostics;

	internal PixelPoint? LastPointerPosition => _pointerOverPreProcessor?.LastPosition;

	internal IAccessKeyHandler AccessKeyHandler => _accessKeyHandler;

	IKeyboardNavigationHandler IInputRoot.KeyboardNavigationHandler => _keyboardNavigationHandler;

	IInputElement? IInputRoot.PointerOverElement
	{
		get
		{
			return GetValue(PointerOverElementProperty);
		}
		set
		{
			SetValue(PointerOverElementProperty, value);
		}
	}

	bool IInputRoot.ShowAccessKeys
	{
		get
		{
			return GetValue(AccessText.ShowAccessKeyProperty);
		}
		set
		{
			SetValue(AccessText.ShowAccessKeyProperty, value);
		}
	}

	double ILayoutRoot.LayoutScaling => PlatformImpl?.RenderScaling ?? 1.0;

	public double RenderScaling => PlatformImpl?.RenderScaling ?? 1.0;

	IStyleHost IStyleHost.StylingParent => _globalStyles;

	public IStorageProvider StorageProvider => _storageProvider ?? (_storageProvider = AvaloniaLocator.Current.GetService<IStorageProviderFactory>()?.CreateProvider(this) ?? PlatformImpl?.TryGetFeature<IStorageProvider>() ?? new NoopStorageProvider());

	public IInsetsManager? InsetsManager => PlatformImpl?.TryGetFeature<IInsetsManager>();

	public IClipboard? Clipboard => PlatformImpl?.TryGetFeature<IClipboard>();

	public IFocusManager? FocusManager => AvaloniaLocator.Current.GetService<IFocusManager>();

	public IPlatformSettings? PlatformSettings => AvaloniaLocator.Current.GetService<IPlatformSettings>();

	protected override bool BypassFlowDirectionPolicies => true;

	ITextInputMethodImpl? ITextInputMethodRoot.InputMethod => PlatformImpl?.TryGetFeature<ITextInputMethodImpl>();

	public event EventHandler? Opened;

	public event EventHandler? Closed;

	public event EventHandler? ScalingChanged;

	public event EventHandler<RoutedEventArgs> BackRequested
	{
		add
		{
			AddHandler(BackRequestedEvent, value);
		}
		remove
		{
			RemoveHandler(BackRequestedEvent, value);
		}
	}

	static TopLevel()
	{
		ClientSizeProperty = AvaloniaProperty.RegisterDirect("ClientSize", (TopLevel o) => o.ClientSize);
		FrameSizeProperty = AvaloniaProperty.RegisterDirect("FrameSize", (TopLevel o) => o.FrameSize, null, null);
		PointerOverElementProperty = AvaloniaProperty.Register<TopLevel, IInputElement>("PointerOverElement");
		TransparencyLevelHintProperty = AvaloniaProperty.Register<TopLevel, IReadOnlyList<WindowTransparencyLevel>>("TransparencyLevelHint", Array.Empty<WindowTransparencyLevel>());
		ActualTransparencyLevelProperty = AvaloniaProperty.RegisterDirect("ActualTransparencyLevel", (TopLevel o) => o.ActualTransparencyLevel, null, WindowTransparencyLevel.None);
		TransparencyBackgroundFallbackProperty = AvaloniaProperty.Register<TopLevel, IBrush>("TransparencyBackgroundFallback", Brushes.White);
		ActualThemeVariantProperty = ThemeVariantScope.ActualThemeVariantProperty.AddOwner<TopLevel>();
		RequestedThemeVariantProperty = ThemeVariantScope.RequestedThemeVariantProperty.AddOwner<TopLevel>();
		SystemBarColorProperty = AvaloniaProperty.RegisterAttached<TopLevel, Control, SolidColorBrush>("SystemBarColor", null, inherits: true);
		BackRequestedEvent = RoutedEvent.Register<TopLevel, RoutedEventArgs>("BackRequested", RoutingStrategies.Bubble);
		ResourcesChangedWeakEvent = WeakEvent.Register(delegate(IResourceHost s, EventHandler<ResourcesChangedEventArgs> h)
		{
			s.ResourcesChanged += h;
		}, delegate(IResourceHost s, EventHandler<ResourcesChangedEventArgs> h)
		{
			s.ResourcesChanged -= h;
		});
		KeyboardNavigation.TabNavigationProperty.OverrideDefaultValue<TopLevel>(KeyboardNavigationMode.Cycle);
		Layoutable.AffectsMeasure<TopLevel>(new AvaloniaProperty[1] { ClientSizeProperty });
		SystemBarColorProperty.Changed.AddClassHandler(delegate(Control view, AvaloniaPropertyChangedEventArgs e)
		{
			if (e.NewValue is SolidColorBrush solidColorBrush)
			{
				if (view.Parent is TopLevel topLevel)
				{
					IInsetsManager insetsManager = topLevel.InsetsManager;
					if (insetsManager != null)
					{
						insetsManager.SystemBarColor = solidColorBrush.Color;
					}
				}
				if (view is TopLevel topLevel2)
				{
					IInsetsManager insetsManager2 = topLevel2.InsetsManager;
					if (insetsManager2 != null)
					{
						insetsManager2.SystemBarColor = solidColorBrush.Color;
					}
				}
			}
		});
	}

	public TopLevel(ITopLevelImpl impl)
		: this(impl, AvaloniaLocator.Current)
	{
	}

	public TopLevel(ITopLevelImpl impl, IAvaloniaDependencyResolver? dependencyResolver)
	{
		TopLevel topLevel = this;
		PlatformImpl = impl ?? throw new InvalidOperationException("Could not create window implementation: maybe no windowing subsystem was initialized?");
		_actualTransparencyLevel = PlatformImpl.TransparencyLevel;
		if (dependencyResolver == null)
		{
			dependencyResolver = AvaloniaLocator.Current;
		}
		_accessKeyHandler = TryGetService<IAccessKeyHandler>(dependencyResolver);
		_inputManager = TryGetService<IInputManager>(dependencyResolver);
		_keyboardNavigationHandler = TryGetService<IKeyboardNavigationHandler>(dependencyResolver);
		_globalStyles = TryGetService<IGlobalStyles>(dependencyResolver);
		_applicationThemeHost = TryGetService<IThemeVariantHost>(dependencyResolver);
		Renderer = new CompositingRenderer(this, impl.Compositor, () => impl.Surfaces);
		Renderer.SceneInvalidated += SceneInvalidated;
		impl.SetInputRoot(this);
		impl.Closed = HandleClosed;
		impl.Input = HandleInput;
		impl.Paint = HandlePaint;
		impl.Resized = HandleResized;
		impl.ScalingChanged = HandleScalingChanged;
		impl.TransparencyLevelChanged = HandleTransparencyLevelChanged;
		_keyboardNavigationHandler?.SetOwner(this);
		_accessKeyHandler?.SetOwner(this);
		if (_globalStyles != null)
		{
			_globalStyles.GlobalStylesAdded += ((IStyleHost)this).StylesAdded;
			_globalStyles.GlobalStylesRemoved += ((IStyleHost)this).StylesRemoved;
		}
		if (_applicationThemeHost != null)
		{
			SetValue(ActualThemeVariantProperty, _applicationThemeHost.ActualThemeVariant, BindingPriority.Template);
			_applicationThemeHost.ActualThemeVariantChanged += GlobalActualThemeVariantChanged;
		}
		ClientSize = impl.ClientSize;
		FrameSize = impl.FrameSize;
		(from x in this.GetObservable(PointerOverElementProperty)
			select (x as InputElement)?.GetObservable(InputElement.CursorProperty) ?? Observable.Empty<Cursor>()).Switch().Subscribe(delegate(Cursor cursor)
		{
			topLevel.PlatformImpl?.SetCursor(cursor?.PlatformImpl);
		});
		if (((IStyleHost)this).StylingParent is IResourceHost target2)
		{
			_resourcesChangesSubscriber = new TargetWeakEventSubscriber<TopLevel, ResourcesChangedEventArgs>(this, delegate(TopLevel target, object? _, WeakEvent _, ResourcesChangedEventArgs e)
			{
				((ILogical)target).NotifyResourcesChanged(e);
			});
			ResourcesChangedWeakEvent.Subscribe(target2, _resourcesChangesSubscriber);
		}
		ITopLevelImpl topLevelImpl = impl;
		topLevelImpl.LostFocus = (Action)Delegate.Combine(topLevelImpl.LostFocus, new Action(PlatformImpl_LostFocus));
		_pointerOverPreProcessor = new PointerOverPreProcessor(this);
		_pointerOverPreProcessorSubscription = _inputManager?.PreProcess.Subscribe(_pointerOverPreProcessor);
		ISystemNavigationManagerImpl systemNavigationManagerImpl = impl.TryGetFeature<ISystemNavigationManagerImpl>();
		if (systemNavigationManagerImpl != null)
		{
			systemNavigationManagerImpl.BackRequested += delegate(object? _, RoutedEventArgs e)
			{
				e.RoutedEvent = BackRequestedEvent;
				topLevel.RaiseEvent(e);
			};
		}
		_backGestureSubscription = _inputManager?.PreProcess.Subscribe(delegate(RawInputEventArgs e)
		{
			bool flag = false;
			if (e is RawKeyEventArgs { Type: RawKeyEventType.KeyDown } rawKeyEventArgs)
			{
				List<KeyGesture> list = topLevel.PlatformSettings?.HotkeyConfiguration.Back;
				if (list != null)
				{
					KeyEventArgs keyEvent = new KeyEventArgs
					{
						KeyModifiers = (KeyModifiers)rawKeyEventArgs.Modifiers,
						Key = rawKeyEventArgs.Key
					};
					flag = list.Any((KeyGesture key) => key.Matches(keyEvent));
				}
			}
			else if (e is RawPointerEventArgs rawPointerEventArgs)
			{
				flag = rawPointerEventArgs.Type == RawPointerEventType.XButton1Down;
			}
			if (flag)
			{
				RoutedEventArgs routedEventArgs = new RoutedEventArgs(BackRequestedEvent);
				topLevel.RaiseEvent(routedEventArgs);
				e.Handled = routedEventArgs.Handled;
			}
		});
	}

	public IPlatformHandle? TryGetPlatformHandle()
	{
		return (PlatformImpl as IWindowBaseImpl)?.Handle;
	}

	public static void SetSystemBarColor(Control control, SolidColorBrush? color)
	{
		control.SetValue(SystemBarColorProperty, color);
	}

	public static SolidColorBrush? GetSystemBarColor(Control control)
	{
		return control.GetValue(SystemBarColorProperty);
	}

	Point IRenderRoot.PointToClient(PixelPoint p)
	{
		return PlatformImpl?.PointToClient(p) ?? default(Point);
	}

	PixelPoint IRenderRoot.PointToScreen(Point p)
	{
		return PlatformImpl?.PointToScreen(p) ?? default(PixelPoint);
	}

	public static TopLevel? GetTopLevel(Visual? visual)
	{
		return visual?.VisualRoot as TopLevel;
	}

	public async Task<IDisposable> RequestPlatformInhibition(PlatformInhibitionType type, string reason)
	{
		IPlatformBehaviorInhibition platformBehaviorInhibition = PlatformImpl?.TryGetFeature<IPlatformBehaviorInhibition>();
		if (platformBehaviorInhibition == null)
		{
			return Disposable.Create(delegate
			{
			});
		}
		if (type == PlatformInhibitionType.AppSleep)
		{
			await platformBehaviorInhibition.SetInhibitAppSleep(inhibitAppSleep: true, reason);
			return Disposable.Create(delegate
			{
				platformBehaviorInhibition.SetInhibitAppSleep(inhibitAppSleep: false, reason).Wait();
			});
		}
		return Disposable.Create(delegate
		{
		});
	}

	public void RequestAnimationFrame(Action<TimeSpan> action)
	{
		Dispatcher.UIThread.VerifyAccess();
		MediaContext.Instance.RequestAnimationFrame(action);
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (change.Property == TransparencyLevelHintProperty)
		{
			if (PlatformImpl != null)
			{
				PlatformImpl.SetTransparencyLevelHint(change.GetNewValue<IReadOnlyList<WindowTransparencyLevel>>() ?? Array.Empty<WindowTransparencyLevel>());
			}
		}
		else if (change.Property == ActualThemeVariantProperty)
		{
			ThemeVariant themeVariant = change.GetNewValue<ThemeVariant>() ?? ThemeVariant.Default;
			PlatformImpl?.SetFrameThemeVariant(((PlatformThemeVariant?)themeVariant).GetValueOrDefault());
		}
	}

	private protected virtual ILayoutManager CreateLayoutManager()
	{
		return new LayoutManager(this);
	}

	private void HandlePaint(Rect rect)
	{
		Renderer.Paint(rect);
	}

	protected void StartRendering()
	{
		MediaContext.Instance.AddTopLevel(this, LayoutManager, Renderer);
	}

	protected void StopRendering()
	{
		MediaContext.Instance.RemoveTopLevel(this);
	}

	private protected virtual void HandleClosed()
	{
		Renderer.SceneInvalidated -= SceneInvalidated;
		Renderer.Dispose();
		PlatformImpl = null;
		if (_globalStyles != null)
		{
			_globalStyles.GlobalStylesAdded -= ((IStyleHost)this).StylesAdded;
			_globalStyles.GlobalStylesRemoved -= ((IStyleHost)this).StylesRemoved;
		}
		if (_applicationThemeHost != null)
		{
			_applicationThemeHost.ActualThemeVariantChanged -= GlobalActualThemeVariantChanged;
		}
		_layoutDiagnosticBridge?.Dispose();
		_layoutDiagnosticBridge = null;
		_pointerOverPreProcessor?.OnCompleted();
		_pointerOverPreProcessorSubscription?.Dispose();
		_backGestureSubscription?.Dispose();
		LogicalTreeAttachmentEventArgs e = new LogicalTreeAttachmentEventArgs(this, this, null);
		((ILogical)this).NotifyDetachedFromLogicalTree(e);
		VisualTreeAttachmentEventArgs e2 = new VisualTreeAttachmentEventArgs(this, this);
		OnDetachedFromVisualTreeCore(e2);
		OnClosed(EventArgs.Empty);
		LayoutManager.Dispose();
	}

	internal virtual void HandleResized(Size clientSize, WindowResizeReason reason)
	{
		ClientSize = clientSize;
		FrameSize = PlatformImpl.FrameSize;
		base.Width = clientSize.Width;
		base.Height = clientSize.Height;
		LayoutManager.ExecuteLayoutPass();
		Renderer.Resized(clientSize);
	}

	private void HandleScalingChanged(double scaling)
	{
		LayoutHelper.InvalidateSelfAndChildrenMeasure(this);
		this.ScalingChanged?.Invoke(this, EventArgs.Empty);
	}

	private void HandleTransparencyLevelChanged(WindowTransparencyLevel transparencyLevel)
	{
		if (_transparencyFallbackBorder != null)
		{
			if (transparencyLevel == WindowTransparencyLevel.None)
			{
				_transparencyFallbackBorder.Background = TransparencyBackgroundFallback;
			}
			else
			{
				_transparencyFallbackBorder.Background = null;
			}
		}
		ActualTransparencyLevel = transparencyLevel;
	}

	protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
	{
		base.OnAttachedToVisualTree(e);
		throw new InvalidOperationException("Control '" + GetType().Name + "' is a top level control and cannot be added as a child.");
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		base.OnApplyTemplate(e);
		if (PlatformImpl != null)
		{
			_transparencyFallbackBorder = e.NameScope.Find<Border>("PART_TransparencyFallback");
			HandleTransparencyLevelChanged(PlatformImpl.TransparencyLevel);
		}
	}

	protected virtual void OnOpened(EventArgs e)
	{
		FrameSize = PlatformImpl?.FrameSize;
		this.Opened?.Invoke(this, e);
	}

	protected virtual void OnClosed(EventArgs e)
	{
		this.Closed?.Invoke(this, e);
	}

	private T? TryGetService<T>(IAvaloniaDependencyResolver resolver) where T : class
	{
		T? service = resolver.GetService<T>();
		if (service == null)
		{
			ParametrizedLogger? parametrizedLogger = Logger.TryGet(LogEventLevel.Warning, "Control");
			if (!parametrizedLogger.HasValue)
			{
				return service;
			}
			parametrizedLogger.GetValueOrDefault().Log(this, "Could not create {Service} : maybe Application.RegisterServices() wasn't called?", typeof(T));
		}
		return service;
	}

	private void HandleInput(RawInputEventArgs e)
	{
		if (PlatformImpl != null)
		{
			if (e is RawPointerEventArgs rawPointerEventArgs)
			{
				rawPointerEventArgs.InputHitTestResult = this.InputHitTest(rawPointerEventArgs.Position);
			}
			_inputManager?.ProcessInput(e);
		}
		else
		{
			Logger.TryGet(LogEventLevel.Warning, "Control")?.Log(this, "PlatformImpl is null, couldn't handle input.");
		}
	}

	private void GlobalActualThemeVariantChanged(object? sender, EventArgs e)
	{
		SetValue(ActualThemeVariantProperty, ((IThemeVariantHost)sender).ActualThemeVariant, BindingPriority.Template);
	}

	private void SceneInvalidated(object? sender, SceneInvalidatedEventArgs e)
	{
		_pointerOverPreProcessor?.SceneInvalidated(e.DirtyRect);
	}

	private void PlatformImpl_LostFocus()
	{
		Visual visual = (Visual)(FocusManager?.GetFocusedElement());
		if (visual != null)
		{
			while (visual.VisualParent != null)
			{
				visual = visual.VisualParent;
			}
			if (visual == this)
			{
				KeyboardDevice.Instance?.SetFocusedElement(null, NavigationMethod.Unspecified, KeyModifiers.None);
			}
		}
	}

	protected internal override void InvalidateMirrorTransform()
	{
	}
}
