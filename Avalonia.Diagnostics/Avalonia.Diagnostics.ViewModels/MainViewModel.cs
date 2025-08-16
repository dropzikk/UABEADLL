using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Avalonia.Diagnostics.Controls;
using Avalonia.Diagnostics.Models;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Metadata;
using Avalonia.Reactive;
using Avalonia.Rendering;
using Avalonia.Threading;

namespace Avalonia.Diagnostics.ViewModels;

internal class MainViewModel : ViewModelBase, IDisposable
{
	private readonly AvaloniaObject _root;

	private readonly TreePageViewModel _logicalTree;

	private readonly TreePageViewModel _visualTree;

	private readonly EventsPageViewModel _events;

	private readonly IDisposable _pointerOverSubscription;

	private ViewModelBase? _content;

	private int _selectedTab;

	private string? _focusedControl;

	private IInputElement? _pointerOverElement;

	private bool _shouldVisualizeMarginPadding = true;

	private bool _freezePopups;

	private string? _pointerOverElementName;

	private IInputRoot? _pointerOverRoot;

	private IScreenshotHandler? _screenshotHandler;

	private bool _showPropertyType;

	private bool _showImplementedInterfaces;

	public bool FreezePopups
	{
		get
		{
			return _freezePopups;
		}
		set
		{
			RaiseAndSetIfChanged(ref _freezePopups, value, "FreezePopups");
		}
	}

	public bool ShouldVisualizeMarginPadding
	{
		get
		{
			return _shouldVisualizeMarginPadding;
		}
		set
		{
			RaiseAndSetIfChanged(ref _shouldVisualizeMarginPadding, value, "ShouldVisualizeMarginPadding");
		}
	}

	public bool ShowDirtyRectsOverlay
	{
		get
		{
			return GetDebugOverlay(RendererDebugOverlays.DirtyRects);
		}
		set
		{
			SetDebugOverlay(RendererDebugOverlays.DirtyRects, value, "ShowDirtyRectsOverlay");
		}
	}

	public bool ShowFpsOverlay
	{
		get
		{
			return GetDebugOverlay(RendererDebugOverlays.Fps);
		}
		set
		{
			SetDebugOverlay(RendererDebugOverlays.Fps, value, "ShowFpsOverlay");
		}
	}

	public bool ShowLayoutTimeGraphOverlay
	{
		get
		{
			return GetDebugOverlay(RendererDebugOverlays.LayoutTimeGraph);
		}
		set
		{
			SetDebugOverlay(RendererDebugOverlays.LayoutTimeGraph, value, "ShowLayoutTimeGraphOverlay");
		}
	}

	public bool ShowRenderTimeGraphOverlay
	{
		get
		{
			return GetDebugOverlay(RendererDebugOverlays.RenderTimeGraph);
		}
		set
		{
			SetDebugOverlay(RendererDebugOverlays.RenderTimeGraph, value, "ShowRenderTimeGraphOverlay");
		}
	}

	public ConsoleViewModel Console { get; }

	public ViewModelBase? Content
	{
		get
		{
			return _content;
		}
		private set
		{
			if (_content is TreePageViewModel treePageViewModel)
			{
				TreePageViewModel newTree = value as TreePageViewModel;
				if (newTree != null)
				{
					AvaloniaObject avaloniaObject = treePageViewModel?.SelectedNode?.Visual;
					Control control = avaloniaObject as Control;
					if (control != null)
					{
						DispatcherTimer.RunOnce(delegate
						{
							try
							{
								newTree.SelectControl(control);
							}
							catch
							{
							}
						}, TimeSpan.FromMilliseconds(0.0));
					}
				}
			}
			RaiseAndSetIfChanged(ref _content, value, "Content");
		}
	}

	public int SelectedTab
	{
		get
		{
			return _selectedTab;
		}
		set
		{
			_selectedTab = value;
			switch (value)
			{
			case 1:
				Content = _visualTree;
				break;
			case 2:
				Content = _events;
				break;
			default:
				Content = _logicalTree;
				break;
			}
			RaisePropertyChanged("SelectedTab");
		}
	}

	public string? FocusedControl
	{
		get
		{
			return _focusedControl;
		}
		private set
		{
			RaiseAndSetIfChanged(ref _focusedControl, value, "FocusedControl");
		}
	}

	public IInputRoot? PointerOverRoot
	{
		get
		{
			return _pointerOverRoot;
		}
		private set
		{
			RaiseAndSetIfChanged(ref _pointerOverRoot, value, "PointerOverRoot");
		}
	}

	public IInputElement? PointerOverElement
	{
		get
		{
			return _pointerOverElement;
		}
		private set
		{
			RaiseAndSetIfChanged(ref _pointerOverElement, value, "PointerOverElement");
			PointerOverElementName = value?.GetType()?.Name;
		}
	}

	public string? PointerOverElementName
	{
		get
		{
			return _pointerOverElementName;
		}
		private set
		{
			RaiseAndSetIfChanged(ref _pointerOverElementName, value, "PointerOverElementName");
		}
	}

	public int? StartupScreenIndex { get; private set; }

	public bool ShowImplementedInterfaces
	{
		get
		{
			return _showImplementedInterfaces;
		}
		private set
		{
			RaiseAndSetIfChanged(ref _showImplementedInterfaces, value, "ShowImplementedInterfaces");
		}
	}

	public bool ShowDetailsPropertyType
	{
		get
		{
			return _showPropertyType;
		}
		private set
		{
			RaiseAndSetIfChanged(ref _showPropertyType, value, "ShowDetailsPropertyType");
		}
	}

	public MainViewModel(AvaloniaObject root)
	{
		_root = root;
		TreeNode[] nodes = LogicalTreeNode.Create(root);
		_logicalTree = new TreePageViewModel(this, nodes);
		nodes = VisualTreeNode.Create(root);
		_visualTree = new TreePageViewModel(this, nodes);
		_events = new EventsPageViewModel(this);
		UpdateFocusedControl();
		if (KeyboardDevice.Instance != null)
		{
			KeyboardDevice.Instance.PropertyChanged += KeyboardPropertyChanged;
		}
		SelectedTab = 0;
		if (root is TopLevel topLevel)
		{
			_pointerOverRoot = topLevel;
			_pointerOverSubscription = topLevel.GetObservable(TopLevel.PointerOverElementProperty).Subscribe(delegate(IInputElement x)
			{
				PointerOverElement = x;
			});
		}
		else
		{
			_pointerOverSubscription = InputManager.Instance.PreProcess.Subscribe(delegate(RawInputEventArgs e)
			{
				if (e is RawPointerEventArgs rawPointerEventArgs)
				{
					PointerOverRoot = rawPointerEventArgs.Root;
					PointerOverElement = rawPointerEventArgs.Root.InputHitTest(rawPointerEventArgs.Position);
				}
			});
		}
		Console = new ConsoleViewModel(UpdateConsoleContext);
	}

	public void ToggleVisualizeMarginPadding()
	{
		ShouldVisualizeMarginPadding = !ShouldVisualizeMarginPadding;
	}

	private IRenderer? TryGetRenderer()
	{
		AvaloniaObject root = _root;
		if (!(root is TopLevel topLevel))
		{
			if (root is Avalonia.Diagnostics.Controls.Application application)
			{
				return application.RendererRoot;
			}
			return null;
		}
		return topLevel.Renderer;
	}

	private bool GetDebugOverlay(RendererDebugOverlays overlay)
	{
		return ((TryGetRenderer()?.Diagnostics.DebugOverlays ?? RendererDebugOverlays.None) & overlay) != 0;
	}

	private void SetDebugOverlay(RendererDebugOverlays overlay, bool enable, [CallerMemberName] string? propertyName = null)
	{
		IRenderer renderer = TryGetRenderer();
		if (renderer != null)
		{
			RendererDebugOverlays debugOverlays = renderer.Diagnostics.DebugOverlays;
			RendererDebugOverlays rendererDebugOverlays = (enable ? (debugOverlays | overlay) : (debugOverlays & ~overlay));
			if (debugOverlays != rendererDebugOverlays)
			{
				renderer.Diagnostics.DebugOverlays = rendererDebugOverlays;
				RaisePropertyChanged(propertyName);
			}
		}
	}

	public void ToggleDirtyRectsOverlay()
	{
		ShowDirtyRectsOverlay = !ShowDirtyRectsOverlay;
	}

	public void ToggleFpsOverlay()
	{
		ShowFpsOverlay = !ShowFpsOverlay;
	}

	public void ToggleLayoutTimeGraphOverlay()
	{
		ShowLayoutTimeGraphOverlay = !ShowLayoutTimeGraphOverlay;
	}

	public void ToggleRenderTimeGraphOverlay()
	{
		ShowRenderTimeGraphOverlay = !ShowRenderTimeGraphOverlay;
	}

	private void UpdateConsoleContext(ConsoleContext context)
	{
		context.root = _root;
		if (Content is TreePageViewModel treePageViewModel)
		{
			context.e = treePageViewModel.SelectedNode?.Visual;
		}
	}

	public void SelectControl(Control control)
	{
		(Content as TreePageViewModel)?.SelectControl(control);
	}

	public void EnableSnapshotStyles(bool enable)
	{
		if (Content is TreePageViewModel { Details: not null } treePageViewModel)
		{
			treePageViewModel.Details.SnapshotStyles = enable;
		}
	}

	public void Dispose()
	{
		if (KeyboardDevice.Instance != null)
		{
			KeyboardDevice.Instance.PropertyChanged -= KeyboardPropertyChanged;
		}
		_pointerOverSubscription.Dispose();
		_logicalTree.Dispose();
		_visualTree.Dispose();
		IRenderer renderer = TryGetRenderer();
		if (renderer != null)
		{
			renderer.Diagnostics.DebugOverlays = RendererDebugOverlays.None;
		}
	}

	private void UpdateFocusedControl()
	{
		FocusedControl = KeyboardDevice.Instance?.FocusedElement?.GetType().Name;
	}

	private void KeyboardPropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName == "FocusedElement")
		{
			UpdateFocusedControl();
		}
	}

	public void RequestTreeNavigateTo(Control control, bool isVisualTree)
	{
		TreePageViewModel treePageViewModel = (isVisualTree ? _visualTree : _logicalTree);
		if (treePageViewModel.FindNode(control) != null)
		{
			SelectedTab = (isVisualTree ? 1 : 0);
			treePageViewModel.SelectControl(control);
		}
	}

	[DependsOn("SelectedNode")]
	[DependsOn("Content")]
	public bool CanShot(object? parameter)
	{
		if (Content is TreePageViewModel { SelectedNode: not null } treePageViewModel && treePageViewModel.SelectedNode.Visual is Visual visual)
		{
			return visual.VisualRoot != null;
		}
		return false;
	}

	public async void Shot(object? parameter)
	{
		if ((Content as TreePageViewModel)?.SelectedNode?.Visual is Control control && _screenshotHandler != null)
		{
			try
			{
				await _screenshotHandler.Take(control);
			}
			catch (Exception)
			{
			}
		}
	}

	public void SetOptions(DevToolsOptions options)
	{
		_screenshotHandler = options.ScreenshotHandler;
		StartupScreenIndex = options.StartupScreenIndex;
		ShowImplementedInterfaces = options.ShowImplementedInterfaces;
	}

	public void ToggleShowImplementedInterfaces(object parameter)
	{
		ShowImplementedInterfaces = !ShowImplementedInterfaces;
		if (Content is TreePageViewModel treePageViewModel)
		{
			treePageViewModel.UpdatePropertiesView();
		}
	}

	public void ToggleShowDetailsPropertyType(object parameter)
	{
		ShowDetailsPropertyType = !ShowDetailsPropertyType;
	}
}
