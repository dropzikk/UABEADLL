using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Controls.Diagnostics;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Diagnostics.ViewModels;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Markup.Xaml.MarkupExtensions.CompiledBindings;
using Avalonia.Markup.Xaml.XamlIl.Runtime;
using Avalonia.Platform;
using Avalonia.Reactive;
using Avalonia.Styling;
using Avalonia.Themes.Simple;
using Avalonia.VisualTree;
using CompiledAvaloniaXaml;

namespace Avalonia.Diagnostics.Views;

internal class MainWindow : Window, IStyleHost
{
	private readonly IDisposable? _inputSubscription;

	private readonly Dictionary<Popup, IDisposable> _frozenPopupStates;

	private AvaloniaObject? _root;

	private PixelPoint _lastPointerPosition;

	private static Action<object?>? _0021XamlIlPopulateOverride;

	public AvaloniaObject? Root
	{
		get
		{
			return _root;
		}
		set
		{
			if (_root != value)
			{
				if (_root is ICloseable closeable)
				{
					closeable.Closed -= RootClosed;
				}
				_root = value;
				if (_root is ICloseable closeable2)
				{
					closeable2.Closed += RootClosed;
					base.DataContext = new MainViewModel(_root);
				}
				else
				{
					base.DataContext = null;
				}
			}
		}
	}

	IStyleHost? IStyleHost.StylingParent => null;

	public MainWindow()
	{
		InitializeComponent();
		if (base.Theme == null && this.FindResource(typeof(Window)) is ControlTheme theme)
		{
			base.Theme = theme;
		}
		_inputSubscription = InputManager.Instance?.Process.Subscribe(delegate(RawInputEventArgs x)
		{
			if (x is RawPointerEventArgs rawPointerEventArgs)
			{
				_lastPointerPosition = ((Visual)x.Root).PointToScreen(rawPointerEventArgs.Position);
			}
			else if (x is RawKeyEventArgs { Type: RawKeyEventType.KeyDown } rawKeyEventArgs)
			{
				RawKeyDown(rawKeyEventArgs);
			}
		});
		_frozenPopupStates = new Dictionary<Popup, IDisposable>();
		EventHandler lh = null;
		lh = delegate
		{
			base.Opened -= lh;
			int? num = (base.DataContext as MainViewModel)?.StartupScreenIndex;
			if (num.HasValue)
			{
				int valueOrDefault = num.GetValueOrDefault();
				Screens screens = base.Screens;
				if (valueOrDefault > -1 && valueOrDefault < screens.ScreenCount)
				{
					Screen screen = screens.All[valueOrDefault];
					base.Position = screen.Bounds.TopLeft;
					base.WindowState = WindowState.Maximized;
				}
			}
		};
		base.Opened += lh;
	}

	protected override void OnClosed(EventArgs e)
	{
		base.OnClosed(e);
		_inputSubscription?.Dispose();
		foreach (KeyValuePair<Popup, IDisposable> frozenPopupState in _frozenPopupStates)
		{
			frozenPopupState.Value.Dispose();
		}
		_frozenPopupStates.Clear();
		if (_root is ICloseable closeable)
		{
			closeable.Closed -= RootClosed;
			_root = null;
		}
		((MainViewModel)base.DataContext)?.Dispose();
	}

	private void InitializeComponent()
	{
		_0021XamlIlPopulateTrampoline(this);
	}

	private Control? GetHoveredControl(TopLevel topLevel)
	{
		Point p = topLevel.PointToClient(_lastPointerPosition);
		return (Control)topLevel.GetVisualsAt(p, delegate(Visual x)
		{
			if (x is AdornerLayer || !x.IsVisible)
			{
				return false;
			}
			return !(x is IInputElement inputElement) || inputElement.IsHitTestVisible;
		}).FirstOrDefault();
	}

	private static List<PopupRoot> GetPopupRoots(TopLevel root)
	{
		List<PopupRoot> popupRoots = new List<PopupRoot>();
		foreach (Control item in root.GetVisualDescendants().OfType<Control>())
		{
			if (item is Popup { Host: PopupRoot host })
			{
				popupRoots.Add(host);
			}
			ProcessProperty<FlyoutBase>(item, Control.ContextFlyoutProperty);
			ProcessProperty<ContextMenu>(item, Control.ContextMenuProperty);
			ProcessProperty<FlyoutBase>(item, FlyoutBase.AttachedFlyoutProperty);
			ProcessProperty<ToolTip>(item, ToolTipDiagnostics.ToolTipProperty);
			ProcessProperty<FlyoutBase>(item, Button.FlyoutProperty);
		}
		return popupRoots;
		void ProcessProperty<T>(Control control, AvaloniaProperty<T> property) where T : notnull
		{
			if (control.GetValue(property) is IPopupHostProvider { PopupHost: PopupRoot popupHost })
			{
				popupRoots.Add(popupHost);
			}
		}
	}

	private void RawKeyDown(RawKeyEventArgs e)
	{
		MainViewModel mainViewModel = (MainViewModel)base.DataContext;
		if (mainViewModel == null)
		{
			return;
		}
		TopLevel topLevel = mainViewModel.PointerOverRoot as TopLevel;
		if (topLevel == null)
		{
			return;
		}
		if (topLevel is PopupRoot { ParentTopLevel: not null } popupRoot)
		{
			topLevel = popupRoot.ParentTopLevel;
		}
		switch (e.Modifiers)
		{
		case RawInputModifiers.Control:
			if (e.Key != Key.LeftShift && e.Key != Key.RightShift)
			{
				break;
			}
			goto case RawInputModifiers.Control | RawInputModifiers.Shift;
		case RawInputModifiers.Shift:
			if (e.Key != Key.LeftCtrl && e.Key != Key.RightCtrl)
			{
				break;
			}
			goto case RawInputModifiers.Control | RawInputModifiers.Shift;
		case RawInputModifiers.Control | RawInputModifiers.Shift:
		{
			Control control = null;
			foreach (PopupRoot popupRoot2 in GetPopupRoots(topLevel))
			{
				control = GetHoveredControl(popupRoot2);
				if (control != null)
				{
					break;
				}
			}
			if (control == null)
			{
				control = GetHoveredControl(topLevel);
			}
			if (control != null)
			{
				mainViewModel.SelectControl(control);
			}
			break;
		}
		case RawInputModifiers.Alt | RawInputModifiers.Control:
			if (e.Key != Key.F)
			{
				break;
			}
			mainViewModel.FreezePopups = !mainViewModel.FreezePopups;
			{
				foreach (PopupRoot popupRoot3 in GetPopupRoots(topLevel))
				{
					if (!(popupRoot3.Parent is Popup popup))
					{
						continue;
					}
					IDisposable value;
					if (mainViewModel.FreezePopups)
					{
						IDisposable disposable = popup.SetValue(Popup.IsLightDismissEnabledProperty, !mainViewModel.FreezePopups, BindingPriority.Animation);
						if (disposable != null)
						{
							_frozenPopupStates[popup] = disposable;
						}
					}
					else if (_frozenPopupStates.TryGetValue(popup, out value))
					{
						value.Dispose();
						_frozenPopupStates.Remove(popup);
					}
				}
				break;
			}
		case RawInputModifiers.Alt:
			if (e.Key == Key.S || e.Key == Key.D)
			{
				mainViewModel.EnableSnapshotStyles(e.Key == Key.S);
			}
			break;
		case RawInputModifiers.Alt | RawInputModifiers.Shift:
			break;
		}
	}

	private void RootClosed(object? sender, EventArgs e)
	{
		Close();
	}

	public void SetOptions(DevToolsOptions options)
	{
		(base.DataContext as MainViewModel)?.SetOptions(options);
		ThemeVariant themeVariant = options.ThemeVariant;
		if ((object)themeVariant != null)
		{
			base.RequestedThemeVariant = themeVariant;
		}
	}

	internal void SelectedControl(Control? control)
	{
		if (control != null)
		{
			(base.DataContext as MainViewModel)?.SelectControl(control);
		}
	}

	static void _0021XamlIlPopulate(IServiceProvider? P_0, MainWindow? P_1)
	{
		CompiledAvaloniaXaml.XamlIlContext.Context<MainWindow> context = new CompiledAvaloniaXaml.XamlIlContext.Context<MainWindow>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FDiagnostics_002FViews_002FMainWindow_002Examl.Singleton }, "avares://Avalonia.Diagnostics/Diagnostics/Views/MainWindow.xaml");
		context.RootObject = P_1;
		context.IntermediateRoot = P_1;
		((ISupportInitialize)P_1).BeginInit();
		context.PushParent(P_1);
		P_1.Title = "Avalonia DevTools";
		P_1.DataTemplates.Add(new ViewLocator());
		P_1.Styles.Add(new SimpleTheme(context));
		P_1.Styles.Add(_0021AvaloniaResources.Build_003A_002FThemes_002FSimple_002Examl(XamlIlRuntimeHelpers.CreateRootServiceProviderV3(context)));
		P_1.Styles.Add(_0021AvaloniaResources.Build_003A_002FDiagnostics_002FControls_002FThicknessEditor_002Eaxaml(XamlIlRuntimeHelpers.CreateRootServiceProviderV3(context)));
		P_1.Styles.Add(_0021AvaloniaResources.Build_003A_002FDiagnostics_002FControls_002FFilterTextBox_002Eaxaml(XamlIlRuntimeHelpers.CreateRootServiceProviderV3(context)));
		P_1.Styles.Add(_0021AvaloniaResources.Build_003A_002FDiagnostics_002FControls_002FBrushEditor_002Eaxaml(XamlIlRuntimeHelpers.CreateRootServiceProviderV3(context)));
		P_1.Styles.Add(_0021AvaloniaResources.Build_003A_002FThemes_002FSimple_002FSimple_002Examl(XamlIlRuntimeHelpers.CreateRootServiceProviderV3(context)));
		List<KeyBinding> keyBindings = P_1.KeyBindings;
		KeyBinding keyBinding;
		KeyBinding item = (keyBinding = new KeyBinding());
		context.PushParent(keyBinding);
		keyBinding.Gesture = KeyGesture.Parse("F8");
		StyledProperty<ICommand> commandProperty = KeyBinding.CommandProperty;
		CompiledBindingExtension compiledBindingExtension = new CompiledBindingExtension(new CompiledBindingPathBuilder().Command("Shot", CompiledAvaloniaXaml.XamlIlTrampolines.Avalonia_002EDiagnostics_003AAvalonia_002EDiagnostics_002EViewModels_002EMainViewModel_002BShot_1_0021CommandExecuteTrampoline, CompiledAvaloniaXaml.XamlIlTrampolines.Avalonia_002EDiagnostics_003AAvalonia_002EDiagnostics_002EViewModels_002EMainViewModel_002BCanShot_0021CommandCanExecuteTrampoline, new string[2] { "SelectedNode", "Content" }).Build());
		context.ProvideTargetProperty = KeyBinding.CommandProperty;
		CompiledBindingExtension binding = compiledBindingExtension.ProvideValue(context);
		context.ProvideTargetProperty = null;
		keyBinding.Bind(commandProperty, binding);
		context.PopParent();
		keyBindings.Add(item);
		MainView mainView;
		MainView mainView2 = (mainView = new MainView());
		((ISupportInitialize)mainView2).BeginInit();
		P_1.Content = mainView2;
		((ISupportInitialize)mainView).EndInit();
		context.PopParent();
		((ISupportInitialize)P_1).EndInit();
		if (P_1 is StyledElement styled)
		{
			NameScope.SetNameScope(styled, context.AvaloniaNameScope);
		}
		context.AvaloniaNameScope.Complete();
	}

	private static void _0021XamlIlPopulateTrampoline(MainWindow? P_0)
	{
		if (_0021XamlIlPopulateOverride != null)
		{
			_0021XamlIlPopulateOverride(P_0);
		}
		else
		{
			_0021XamlIlPopulate(XamlIlRuntimeHelpers.CreateRootServiceProviderV3(null), P_0);
		}
	}
}
