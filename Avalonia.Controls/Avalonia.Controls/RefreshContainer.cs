using System;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.PullToRefresh;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Reactive;

namespace Avalonia.Controls;

public class RefreshContainer : ContentControl
{
	internal const int DefaultPullDimensionSize = 100;

	private bool _hasDefaultRefreshInfoProviderAdapter;

	private ScrollViewerIRefreshInfoProviderAdapter? _refreshInfoProviderAdapter;

	private RefreshInfoProvider? _refreshInfoProvider;

	private IDisposable? _visualizerSizeSubscription;

	private Grid? _visualizerPresenter;

	private RefreshVisualizer? _refreshVisualizer;

	private bool _hasDefaultRefreshVisualizer;

	public static readonly RoutedEvent<RefreshRequestedEventArgs> RefreshRequestedEvent = RoutedEvent.Register<RefreshContainer, RefreshRequestedEventArgs>("RefreshRequested", RoutingStrategies.Bubble);

	internal static readonly DirectProperty<RefreshContainer, ScrollViewerIRefreshInfoProviderAdapter?> RefreshInfoProviderAdapterProperty = AvaloniaProperty.RegisterDirect("RefreshInfoProviderAdapter", (RefreshContainer s) => s.RefreshInfoProviderAdapter, delegate(RefreshContainer s, ScrollViewerIRefreshInfoProviderAdapter? o)
	{
		s.RefreshInfoProviderAdapter = o;
	});

	public static readonly DirectProperty<RefreshContainer, RefreshVisualizer?> VisualizerProperty = AvaloniaProperty.RegisterDirect("Visualizer", (RefreshContainer s) => s.Visualizer, delegate(RefreshContainer s, RefreshVisualizer? o)
	{
		s.Visualizer = o;
	});

	public static readonly StyledProperty<PullDirection> PullDirectionProperty = AvaloniaProperty.Register<RefreshContainer, PullDirection>("PullDirection", PullDirection.TopToBottom);

	internal ScrollViewerIRefreshInfoProviderAdapter? RefreshInfoProviderAdapter
	{
		get
		{
			return _refreshInfoProviderAdapter;
		}
		set
		{
			_hasDefaultRefreshInfoProviderAdapter = false;
			SetAndRaise(RefreshInfoProviderAdapterProperty, ref _refreshInfoProviderAdapter, value);
		}
	}

	public RefreshVisualizer? Visualizer
	{
		get
		{
			return _refreshVisualizer;
		}
		set
		{
			if (_refreshVisualizer != null)
			{
				_visualizerSizeSubscription?.Dispose();
				_refreshVisualizer.RefreshRequested -= Visualizer_RefreshRequested;
			}
			SetAndRaise(VisualizerProperty, ref _refreshVisualizer, value);
		}
	}

	public PullDirection PullDirection
	{
		get
		{
			return GetValue(PullDirectionProperty);
		}
		set
		{
			SetValue(PullDirectionProperty, value);
		}
	}

	public event EventHandler<RefreshRequestedEventArgs>? RefreshRequested
	{
		add
		{
			AddHandler(RefreshRequestedEvent, value);
		}
		remove
		{
			RemoveHandler(RefreshRequestedEvent, value);
		}
	}

	public RefreshContainer()
	{
		_hasDefaultRefreshInfoProviderAdapter = true;
		_refreshInfoProviderAdapter = new ScrollViewerIRefreshInfoProviderAdapter(PullDirection);
		RaisePropertyChanged(RefreshInfoProviderAdapterProperty, null, _refreshInfoProviderAdapter);
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		base.OnApplyTemplate(e);
		_visualizerPresenter = e.NameScope.Find<Grid>("PART_RefreshVisualizerPresenter");
		if (_refreshVisualizer == null)
		{
			_hasDefaultRefreshVisualizer = true;
			Visualizer = new RefreshVisualizer();
		}
		else
		{
			_hasDefaultRefreshVisualizer = false;
			RaisePropertyChanged(VisualizerProperty, null, _refreshVisualizer);
		}
		OnPullDirectionChanged();
	}

	private void OnVisualizerSizeChanged(Rect obj)
	{
		if (_hasDefaultRefreshInfoProviderAdapter)
		{
			_refreshInfoProviderAdapter = new ScrollViewerIRefreshInfoProviderAdapter(PullDirection);
			RaisePropertyChanged(RefreshInfoProviderAdapterProperty, null, _refreshInfoProviderAdapter);
		}
	}

	private void Visualizer_RefreshRequested(object? sender, RefreshRequestedEventArgs e)
	{
		RefreshRequestedEventArgs refreshRequestedEventArgs = new RefreshRequestedEventArgs(e.GetDeferral(), RefreshRequestedEvent);
		RaiseEvent(refreshRequestedEventArgs);
		refreshRequestedEventArgs.DecrementCount();
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (change.Property == RefreshInfoProviderAdapterProperty)
		{
			if (_refreshVisualizer == null)
			{
				return;
			}
			if (_refreshInfoProvider != null)
			{
				_refreshVisualizer.RefreshInfoProvider = _refreshInfoProvider;
			}
			else if (RefreshInfoProviderAdapter != null && _refreshVisualizer != null)
			{
				_refreshInfoProvider = RefreshInfoProviderAdapter?.AdaptFromTree(this, _refreshVisualizer.Bounds.Size);
				if (_refreshInfoProvider != null)
				{
					_refreshVisualizer.RefreshInfoProvider = _refreshInfoProvider;
					RefreshInfoProviderAdapter?.SetAnimations(_refreshVisualizer);
				}
			}
		}
		else if (change.Property == VisualizerProperty)
		{
			if (_visualizerPresenter != null)
			{
				_visualizerPresenter.Children.Clear();
				if (_refreshVisualizer != null)
				{
					_visualizerPresenter.Children.Add(_refreshVisualizer);
				}
			}
			if (_refreshVisualizer != null)
			{
				_refreshVisualizer.RefreshRequested += Visualizer_RefreshRequested;
				_visualizerSizeSubscription = _refreshVisualizer.GetObservable(Visual.BoundsProperty).Subscribe(OnVisualizerSizeChanged);
			}
		}
		else if (change.Property == PullDirectionProperty)
		{
			OnPullDirectionChanged();
		}
	}

	private void OnPullDirectionChanged()
	{
		if (_visualizerPresenter == null || _refreshVisualizer == null)
		{
			return;
		}
		switch (PullDirection)
		{
		case PullDirection.TopToBottom:
			_visualizerPresenter.VerticalAlignment = VerticalAlignment.Top;
			_visualizerPresenter.HorizontalAlignment = HorizontalAlignment.Stretch;
			if (_hasDefaultRefreshVisualizer)
			{
				_refreshVisualizer.PullDirection = PullDirection.TopToBottom;
				_refreshVisualizer.Height = 100.0;
				_refreshVisualizer.Width = double.NaN;
			}
			break;
		case PullDirection.BottomToTop:
			_visualizerPresenter.VerticalAlignment = VerticalAlignment.Bottom;
			_visualizerPresenter.HorizontalAlignment = HorizontalAlignment.Stretch;
			if (_hasDefaultRefreshVisualizer)
			{
				_refreshVisualizer.PullDirection = PullDirection.BottomToTop;
				_refreshVisualizer.Height = 100.0;
				_refreshVisualizer.Width = double.NaN;
			}
			break;
		case PullDirection.LeftToRight:
			_visualizerPresenter.VerticalAlignment = VerticalAlignment.Stretch;
			_visualizerPresenter.HorizontalAlignment = HorizontalAlignment.Left;
			if (_hasDefaultRefreshVisualizer)
			{
				_refreshVisualizer.PullDirection = PullDirection.LeftToRight;
				_refreshVisualizer.Width = 100.0;
				_refreshVisualizer.Height = double.NaN;
			}
			break;
		case PullDirection.RightToLeft:
			_visualizerPresenter.VerticalAlignment = VerticalAlignment.Stretch;
			_visualizerPresenter.HorizontalAlignment = HorizontalAlignment.Right;
			if (_hasDefaultRefreshVisualizer)
			{
				_refreshVisualizer.PullDirection = PullDirection.RightToLeft;
				_refreshVisualizer.Width = 100.0;
				_refreshVisualizer.Height = double.NaN;
			}
			break;
		}
		if (_hasDefaultRefreshInfoProviderAdapter && _hasDefaultRefreshVisualizer && _refreshVisualizer.Bounds.Height == 100.0 && _refreshVisualizer.Bounds.Width == 100.0)
		{
			_refreshInfoProviderAdapter = new ScrollViewerIRefreshInfoProviderAdapter(PullDirection);
			RaisePropertyChanged(RefreshInfoProviderAdapterProperty, null, _refreshInfoProviderAdapter);
		}
	}

	public void RequestRefresh()
	{
		_refreshVisualizer?.RequestRefresh();
	}
}
