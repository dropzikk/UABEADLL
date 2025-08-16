using System;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Rendering.Composition;

namespace Avalonia.Controls.PullToRefresh;

internal class RefreshInfoProvider : Interactive
{
	internal const double DefaultExecutionRatio = 0.8;

	private readonly PullDirection _refreshPullDirection;

	private readonly Size _refreshVisualizerSize;

	private readonly CompositionVisual? _visual;

	private bool _isInteractingForRefresh;

	private double _interactionRatio;

	private bool _entered;

	public static readonly DirectProperty<RefreshInfoProvider, bool> IsInteractingForRefreshProperty = AvaloniaProperty.RegisterDirect("IsInteractingForRefresh", (RefreshInfoProvider s) => s.IsInteractingForRefresh, delegate(RefreshInfoProvider s, bool o)
	{
		s.IsInteractingForRefresh = o;
	}, unsetValue: false);

	public static readonly DirectProperty<RefreshInfoProvider, double> ExecutionRatioProperty = AvaloniaProperty.RegisterDirect("ExecutionRatio", (RefreshInfoProvider s) => s.ExecutionRatio, null, 0.0);

	public static readonly DirectProperty<RefreshInfoProvider, double> InteractionRatioProperty = AvaloniaProperty.RegisterDirect("InteractionRatio", (RefreshInfoProvider s) => s.InteractionRatio, delegate(RefreshInfoProvider s, double o)
	{
		s.InteractionRatio = o;
	}, 0.0);

	public static readonly RoutedEvent<RoutedEventArgs> RefreshStartedEvent = RoutedEvent.Register<RefreshInfoProvider, RoutedEventArgs>("RefreshStarted", RoutingStrategies.Bubble);

	public static readonly RoutedEvent<RoutedEventArgs> RefreshCompletedEvent = RoutedEvent.Register<RefreshInfoProvider, RoutedEventArgs>("RefreshCompleted", RoutingStrategies.Bubble);

	public bool PeekingMode { get; internal set; }

	public bool IsInteractingForRefresh
	{
		get
		{
			return _isInteractingForRefresh;
		}
		internal set
		{
			bool flag = value && !PeekingMode;
			if (flag != _isInteractingForRefresh)
			{
				SetAndRaise(IsInteractingForRefreshProperty, ref _isInteractingForRefresh, flag);
			}
		}
	}

	public double InteractionRatio
	{
		get
		{
			return _interactionRatio;
		}
		set
		{
			SetAndRaise(InteractionRatioProperty, ref _interactionRatio, value);
		}
	}

	public double ExecutionRatio => 0.8;

	internal CompositionVisual? Visual => _visual;

	public event EventHandler<RoutedEventArgs> RefreshStarted
	{
		add
		{
			AddHandler(RefreshStartedEvent, value);
		}
		remove
		{
			RemoveHandler(RefreshStartedEvent, value);
		}
	}

	public event EventHandler<RoutedEventArgs> RefreshCompleted
	{
		add
		{
			AddHandler(RefreshCompletedEvent, value);
		}
		remove
		{
			RemoveHandler(RefreshCompletedEvent, value);
		}
	}

	internal void InteractingStateEntered(object? sender, PullGestureEventArgs e)
	{
		if (!_entered)
		{
			IsInteractingForRefresh = true;
			_entered = true;
		}
		ValuesChanged(e.Delta);
	}

	internal void InteractingStateExited(object? sender, PullGestureEndedEventArgs e)
	{
		IsInteractingForRefresh = false;
		_entered = false;
		ValuesChanged(default(Vector));
	}

	public RefreshInfoProvider(PullDirection refreshPullDirection, Size? refreshVIsualizerSize, CompositionVisual? visual)
	{
		_refreshPullDirection = refreshPullDirection;
		_refreshVisualizerSize = refreshVIsualizerSize.GetValueOrDefault();
		_visual = visual;
	}

	public void OnRefreshStarted()
	{
		RaiseEvent(new RoutedEventArgs(RefreshStartedEvent));
	}

	public void OnRefreshCompleted()
	{
		RaiseEvent(new RoutedEventArgs(RefreshCompletedEvent));
	}

	internal void ValuesChanged(Vector value)
	{
		switch (_refreshPullDirection)
		{
		case PullDirection.TopToBottom:
		case PullDirection.BottomToTop:
			InteractionRatio = ((_refreshVisualizerSize.Height == 0.0) ? 1.0 : Math.Min(1.0, value.Y / _refreshVisualizerSize.Height));
			break;
		case PullDirection.LeftToRight:
		case PullDirection.RightToLeft:
			InteractionRatio = ((_refreshVisualizerSize.Height == 0.0) ? 1.0 : Math.Min(1.0, value.X / _refreshVisualizerSize.Width));
			break;
		}
	}
}
