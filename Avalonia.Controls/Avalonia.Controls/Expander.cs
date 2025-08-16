using System;
using System.Threading;
using Avalonia.Animation;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Threading;

namespace Avalonia.Controls;

[PseudoClasses(new string[] { ":expanded", ":up", ":down", ":left", ":right" })]
public class Expander : HeaderedContentControl
{
	public static readonly StyledProperty<IPageTransition?> ContentTransitionProperty = AvaloniaProperty.Register<Expander, IPageTransition>("ContentTransition");

	public static readonly StyledProperty<ExpandDirection> ExpandDirectionProperty = AvaloniaProperty.Register<Expander, ExpandDirection>("ExpandDirection", ExpandDirection.Down);

	public static readonly StyledProperty<bool> IsExpandedProperty = AvaloniaProperty.Register<Expander, bool>("IsExpanded", defaultValue: false, inherits: false, BindingMode.TwoWay, null, CoerceIsExpanded);

	public static readonly RoutedEvent<RoutedEventArgs> CollapsedEvent = RoutedEvent.Register<Expander, RoutedEventArgs>("Collapsed", RoutingStrategies.Bubble);

	public static readonly RoutedEvent<CancelRoutedEventArgs> CollapsingEvent = RoutedEvent.Register<Expander, CancelRoutedEventArgs>("Collapsing", RoutingStrategies.Bubble);

	public static readonly RoutedEvent<RoutedEventArgs> ExpandedEvent = RoutedEvent.Register<Expander, RoutedEventArgs>("Expanded", RoutingStrategies.Bubble);

	public static readonly RoutedEvent<CancelRoutedEventArgs> ExpandingEvent = RoutedEvent.Register<Expander, CancelRoutedEventArgs>("Expanding", RoutingStrategies.Bubble);

	private bool _ignorePropertyChanged;

	private CancellationTokenSource? _lastTransitionCts;

	public IPageTransition? ContentTransition
	{
		get
		{
			return GetValue(ContentTransitionProperty);
		}
		set
		{
			SetValue(ContentTransitionProperty, value);
		}
	}

	public ExpandDirection ExpandDirection
	{
		get
		{
			return GetValue(ExpandDirectionProperty);
		}
		set
		{
			SetValue(ExpandDirectionProperty, value);
		}
	}

	public bool IsExpanded
	{
		get
		{
			return GetValue(IsExpandedProperty);
		}
		set
		{
			SetValue(IsExpandedProperty, value);
		}
	}

	public event EventHandler<RoutedEventArgs>? Collapsed
	{
		add
		{
			AddHandler(CollapsedEvent, value);
		}
		remove
		{
			RemoveHandler(CollapsedEvent, value);
		}
	}

	public event EventHandler<CancelRoutedEventArgs>? Collapsing
	{
		add
		{
			AddHandler(CollapsingEvent, value);
		}
		remove
		{
			RemoveHandler(CollapsingEvent, value);
		}
	}

	public event EventHandler<RoutedEventArgs>? Expanded
	{
		add
		{
			AddHandler(ExpandedEvent, value);
		}
		remove
		{
			RemoveHandler(ExpandedEvent, value);
		}
	}

	public event EventHandler<CancelRoutedEventArgs>? Expanding
	{
		add
		{
			AddHandler(ExpandingEvent, value);
		}
		remove
		{
			RemoveHandler(ExpandingEvent, value);
		}
	}

	public Expander()
	{
		UpdatePseudoClasses();
	}

	protected virtual void OnCollapsed(RoutedEventArgs eventArgs)
	{
		RaiseEvent(eventArgs);
	}

	protected virtual void OnCollapsing(CancelRoutedEventArgs eventArgs)
	{
		RaiseEvent(eventArgs);
	}

	protected virtual void OnExpanded(RoutedEventArgs eventArgs)
	{
		RaiseEvent(eventArgs);
	}

	protected virtual void OnExpanding(CancelRoutedEventArgs eventArgs)
	{
		RaiseEvent(eventArgs);
	}

	private async void StartContentTransition()
	{
		if (base.Content != null && ContentTransition != null)
		{
			Visual presenter = base.Presenter;
			if (presenter != null)
			{
				bool forward = ExpandDirection == ExpandDirection.Left || ExpandDirection == ExpandDirection.Up;
				_lastTransitionCts?.Cancel();
				_lastTransitionCts = new CancellationTokenSource();
				if (!IsExpanded)
				{
					await ContentTransition.Start(presenter, null, forward, _lastTransitionCts.Token);
				}
				else
				{
					await ContentTransition.Start(null, presenter, forward, _lastTransitionCts.Token);
				}
			}
		}
		Dispatcher.UIThread.Post(delegate
		{
			if (IsExpanded)
			{
				OnExpanded(new RoutedEventArgs(ExpandedEvent, this));
			}
			else
			{
				OnCollapsed(new RoutedEventArgs(CollapsedEvent, this));
			}
		});
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (!_ignorePropertyChanged)
		{
			if (change.Property == ExpandDirectionProperty)
			{
				UpdatePseudoClasses();
			}
			else if (change.Property == IsExpandedProperty)
			{
				StartContentTransition();
				UpdatePseudoClasses();
			}
		}
	}

	private void UpdatePseudoClasses()
	{
		ExpandDirection expandDirection = ExpandDirection;
		base.PseudoClasses.Set(":up", expandDirection == ExpandDirection.Up);
		base.PseudoClasses.Set(":down", expandDirection == ExpandDirection.Down);
		base.PseudoClasses.Set(":left", expandDirection == ExpandDirection.Left);
		base.PseudoClasses.Set(":right", expandDirection == ExpandDirection.Right);
		base.PseudoClasses.Set(":expanded", IsExpanded);
	}

	protected virtual bool OnCoerceIsExpanded(bool value)
	{
		CancelRoutedEventArgs cancelRoutedEventArgs;
		if (value)
		{
			cancelRoutedEventArgs = new CancelRoutedEventArgs(ExpandingEvent, this);
			OnExpanding(cancelRoutedEventArgs);
		}
		else
		{
			cancelRoutedEventArgs = new CancelRoutedEventArgs(CollapsingEvent, this);
			OnCollapsing(cancelRoutedEventArgs);
		}
		if (cancelRoutedEventArgs.Cancel)
		{
			_ignorePropertyChanged = true;
			RaisePropertyChanged(IsExpandedProperty, value, !value, BindingPriority.LocalValue, isEffectiveValue: true);
			_ignorePropertyChanged = false;
			return !value;
		}
		return value;
	}

	private static bool CoerceIsExpanded(AvaloniaObject instance, bool value)
	{
		if (instance is Expander expander)
		{
			return expander.OnCoerceIsExpanded(value);
		}
		return value;
	}
}
