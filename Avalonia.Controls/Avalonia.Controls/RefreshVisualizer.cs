using System;
using System.Numerics;
using Avalonia.Animation.Easings;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.PullToRefresh;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Reactive;
using Avalonia.Rendering.Composition;
using Avalonia.Rendering.Composition.Animations;

namespace Avalonia.Controls;

public class RefreshVisualizer : ContentControl
{
	private const int DefaultIndicatorSize = 24;

	private const float MinimumIndicatorOpacity = 0.4f;

	private const float ParallaxPositionRatio = 0.5f;

	private double _executingRatio = 0.8;

	private RefreshVisualizerState _refreshVisualizerState;

	private RefreshInfoProvider? _refreshInfoProvider;

	private IDisposable? _isInteractingSubscription;

	private IDisposable? _interactionRatioSubscription;

	private bool _isInteractingForRefresh;

	private Grid? _root;

	private Control? _content;

	private RefreshVisualizerOrientation _orientation;

	private float _startingRotationAngle;

	private double _interactionRatio;

	private bool _played;

	private ScalarKeyFrameAnimation? _rotateAnimation;

	internal static readonly StyledProperty<PullDirection> PullDirectionProperty = AvaloniaProperty.Register<RefreshVisualizer, PullDirection>("PullDirection", PullDirection.TopToBottom);

	public static readonly RoutedEvent<RefreshRequestedEventArgs> RefreshRequestedEvent = RoutedEvent.Register<RefreshVisualizer, RefreshRequestedEventArgs>("RefreshRequested", RoutingStrategies.Bubble);

	public static readonly DirectProperty<RefreshVisualizer, RefreshVisualizerState> RefreshVisualizerStateProperty = AvaloniaProperty.RegisterDirect("RefreshVisualizerState", (RefreshVisualizer s) => s.RefreshVisualizerState, null, RefreshVisualizerState.Idle);

	public static readonly DirectProperty<RefreshVisualizer, RefreshVisualizerOrientation> OrientationProperty = AvaloniaProperty.RegisterDirect("Orientation", (RefreshVisualizer s) => s.Orientation, delegate(RefreshVisualizer s, RefreshVisualizerOrientation o)
	{
		s.Orientation = o;
	}, RefreshVisualizerOrientation.Auto);

	internal static readonly DirectProperty<RefreshVisualizer, RefreshInfoProvider?> RefreshInfoProviderProperty = AvaloniaProperty.RegisterDirect("RefreshInfoProvider", (RefreshVisualizer s) => s.RefreshInfoProvider, delegate(RefreshVisualizer s, RefreshInfoProvider? o)
	{
		s.RefreshInfoProvider = o;
	});

	private bool IsPullDirectionVertical
	{
		get
		{
			if (PullDirection != 0)
			{
				return PullDirection == PullDirection.BottomToTop;
			}
			return true;
		}
	}

	private bool IsPullDirectionFar
	{
		get
		{
			if (PullDirection != PullDirection.BottomToTop)
			{
				return PullDirection == PullDirection.RightToLeft;
			}
			return true;
		}
	}

	protected RefreshVisualizerState RefreshVisualizerState
	{
		get
		{
			return _refreshVisualizerState;
		}
		private set
		{
			SetAndRaise(RefreshVisualizerStateProperty, ref _refreshVisualizerState, value);
			UpdateContent();
		}
	}

	public RefreshVisualizerOrientation Orientation
	{
		get
		{
			return _orientation;
		}
		set
		{
			SetAndRaise(OrientationProperty, ref _orientation, value);
		}
	}

	internal PullDirection PullDirection
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

	internal RefreshInfoProvider? RefreshInfoProvider
	{
		get
		{
			return _refreshInfoProvider;
		}
		set
		{
			if (_refreshInfoProvider != null)
			{
				_refreshInfoProvider.RenderTransform = null;
			}
			SetAndRaise(RefreshInfoProviderProperty, ref _refreshInfoProvider, value);
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

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		base.OnApplyTemplate(e);
		base.ClipToBounds = false;
		_root = e.NameScope.Find<Grid>("PART_Root");
		if (_root != null)
		{
			_content = base.Content as Control;
			if (_content == null)
			{
				_content = new PathIcon
				{
					Height = 24.0,
					Width = 24.0,
					Name = "PART_Icon"
				};
				_content.Loaded += delegate
				{
					CompositionVisual elementVisual = ElementComposition.GetElementVisual(_content);
					if (elementVisual != null)
					{
						Compositor compositor = elementVisual.Compositor;
						elementVisual.Opacity = 0f;
						ScalarKeyFrameAnimation scalarKeyFrameAnimation = compositor.CreateScalarKeyFrameAnimation();
						scalarKeyFrameAnimation.Target = "RotationAngle";
						scalarKeyFrameAnimation.InsertExpressionKeyFrame(1f, "this.FinalValue", new LinearEasing());
						scalarKeyFrameAnimation.Duration = TimeSpan.FromMilliseconds(100.0);
						ScalarKeyFrameAnimation scalarKeyFrameAnimation2 = compositor.CreateScalarKeyFrameAnimation();
						scalarKeyFrameAnimation2.Target = "Opacity";
						scalarKeyFrameAnimation2.InsertExpressionKeyFrame(1f, "this.FinalValue", new LinearEasing());
						scalarKeyFrameAnimation2.Duration = TimeSpan.FromMilliseconds(100.0);
						Vector3KeyFrameAnimation vector3KeyFrameAnimation = compositor.CreateVector3KeyFrameAnimation();
						vector3KeyFrameAnimation.Target = "Offset";
						vector3KeyFrameAnimation.InsertExpressionKeyFrame(1f, "this.FinalValue", new LinearEasing());
						vector3KeyFrameAnimation.Duration = TimeSpan.FromMilliseconds(150.0);
						Vector3KeyFrameAnimation vector3KeyFrameAnimation2 = compositor.CreateVector3KeyFrameAnimation();
						vector3KeyFrameAnimation2.Target = "Scale";
						vector3KeyFrameAnimation2.InsertExpressionKeyFrame(1f, "this.FinalValue", new LinearEasing());
						vector3KeyFrameAnimation2.Duration = TimeSpan.FromMilliseconds(100.0);
						ImplicitAnimationCollection implicitAnimationCollection = compositor.CreateImplicitAnimationCollection();
						implicitAnimationCollection["RotationAngle"] = scalarKeyFrameAnimation;
						implicitAnimationCollection["Offset"] = vector3KeyFrameAnimation;
						implicitAnimationCollection["Scale"] = vector3KeyFrameAnimation2;
						implicitAnimationCollection["Opacity"] = scalarKeyFrameAnimation2;
						elementVisual.ImplicitAnimations = implicitAnimationCollection;
						UpdateContent();
					}
				};
				SetCurrentValue(ContentControl.ContentProperty, _content);
			}
			else
			{
				RaisePropertyChanged(ContentControl.ContentProperty, null, base.Content, BindingPriority.Style, isEffectiveValue: false);
			}
		}
		OnOrientationChanged();
		UpdateContent();
	}

	protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
	{
		base.OnAttachedToVisualTree(e);
		UpdateContent();
	}

	private void UpdateContent()
	{
		if (_content == null || _root == null)
		{
			return;
		}
		Grid root = _root;
		CompositionVisual compositionVisual = _refreshInfoProvider?.Visual;
		CompositionVisual elementVisual = ElementComposition.GetElementVisual(_content);
		CompositionVisual elementVisual2 = ElementComposition.GetElementVisual(this);
		if (compositionVisual == null || elementVisual == null || elementVisual2 == null)
		{
			return;
		}
		elementVisual.CenterPoint = new Vector3D(_content.Bounds.Width / 2.0, _content.Bounds.Height / 2.0, 0.0);
		Vector3D vector3D2;
		switch (RefreshVisualizerState)
		{
		case RefreshVisualizerState.Idle:
			_played = false;
			if (_rotateAnimation != null)
			{
				_rotateAnimation.IterationBehavior = AnimationIterationBehavior.Count;
				_rotateAnimation = null;
			}
			elementVisual.Opacity = 0.4f;
			elementVisual.RotationAngle = _startingRotationAngle;
			elementVisual2.Offset = (IsPullDirectionVertical ? new Vector3D(elementVisual2.Offset.X, 0.0, 0.0) : new Vector3D(0.0, elementVisual2.Offset.Y, 0.0));
			_content.InvalidateMeasure();
			break;
		case RefreshVisualizerState.Interacting:
			_played = false;
			elementVisual.Opacity = 0.4f;
			elementVisual.RotationAngle = (float)((double)_startingRotationAngle + _interactionRatio * 2.0 * Math.PI);
			vector3D2 = default(Vector3D);
			vector3D2 = (compositionVisual.Offset = ((!IsPullDirectionVertical) ? new Vector3D(_interactionRatio * (double)((!IsPullDirectionFar) ? 1 : (-1)) * root.Bounds.Width, 0.0, 0.0) : new Vector3D(0.0, _interactionRatio * (double)((!IsPullDirectionFar) ? 1 : (-1)) * root.Bounds.Height, 0.0)));
			elementVisual2.Offset = (IsPullDirectionVertical ? new Vector3D(elementVisual2.Offset.X, vector3D2.Y, 0.0) : new Vector3D(vector3D2.X, elementVisual2.Offset.Y, 0.0));
			break;
		case RefreshVisualizerState.Pending:
			elementVisual.Opacity = 1f;
			elementVisual.RotationAngle = _startingRotationAngle + (float)Math.PI * 2f;
			vector3D2 = (compositionVisual.Offset = ((!IsPullDirectionVertical) ? new Vector3D(_interactionRatio * (double)((!IsPullDirectionFar) ? 1 : (-1)) * root.Bounds.Width, 0.0, 0.0) : new Vector3D(0.0, _interactionRatio * (double)((!IsPullDirectionFar) ? 1 : (-1)) * root.Bounds.Height, 0.0)));
			elementVisual2.Offset = (IsPullDirectionVertical ? new Vector3D(elementVisual2.Offset.X, vector3D2.Y, 0.0) : new Vector3D(vector3D2.X, elementVisual2.Offset.Y, 0.0));
			if (!_played)
			{
				_played = true;
				Vector3KeyFrameAnimation vector3KeyFrameAnimation = elementVisual.Compositor.CreateVector3KeyFrameAnimation();
				vector3KeyFrameAnimation.Target = "Scale";
				vector3KeyFrameAnimation.InsertKeyFrame(0.5f, new Vector3(1.5f, 1.5f, 1f));
				vector3KeyFrameAnimation.InsertKeyFrame(1f, new Vector3(1f, 1f, 1f));
				vector3KeyFrameAnimation.Duration = TimeSpan.FromSeconds(0.3);
				elementVisual.StartAnimation("Scale", vector3KeyFrameAnimation);
			}
			break;
		case RefreshVisualizerState.Refreshing:
		{
			_rotateAnimation = elementVisual.Compositor.CreateScalarKeyFrameAnimation();
			_rotateAnimation.Target = "RotationAngle";
			_rotateAnimation.InsertKeyFrame(0f, _startingRotationAngle, new LinearEasing());
			_rotateAnimation.InsertKeyFrame(1f, _startingRotationAngle + (float)Math.PI * 2f, new LinearEasing());
			_rotateAnimation.IterationBehavior = AnimationIterationBehavior.Forever;
			_rotateAnimation.StopBehavior = AnimationStopBehavior.LeaveCurrentValue;
			_rotateAnimation.Duration = TimeSpan.FromSeconds(0.5);
			elementVisual.StartAnimation("RotationAngle", _rotateAnimation);
			elementVisual.Opacity = 1f;
			float num = (float)((_refreshInfoProvider != null) ? ((1.0 - _refreshInfoProvider.ExecutionRatio) * 0.5) : 1.0) * (IsPullDirectionFar ? (-1f) : 1f);
			vector3D2 = (compositionVisual.Offset = ((!IsPullDirectionVertical) ? new Vector3D(_executingRatio * (double)((!IsPullDirectionFar) ? 1 : (-1)) * root.Bounds.Width, 0.0, 0.0) : new Vector3D(0.0, _executingRatio * (double)((!IsPullDirectionFar) ? 1 : (-1)) * root.Bounds.Height, 0.0)));
			elementVisual.Offset += (IsPullDirectionVertical ? new Vector3D(0.0, (double)num * root.Bounds.Height, 0.0) : new Vector3D((double)num * root.Bounds.Width, 0.0, 0.0));
			elementVisual2.Offset = (IsPullDirectionVertical ? new Vector3D(elementVisual2.Offset.X, vector3D2.Y, 0.0) : new Vector3D(vector3D2.X, elementVisual2.Offset.Y, 0.0));
			break;
		}
		case RefreshVisualizerState.Peeking:
			elementVisual.Opacity = 1f;
			elementVisual.RotationAngle = _startingRotationAngle;
			break;
		}
	}

	public void RequestRefresh()
	{
		RefreshVisualizerState = RefreshVisualizerState.Refreshing;
		RefreshInfoProvider?.OnRefreshStarted();
		RaiseRefreshRequested();
	}

	private void RefreshCompleted()
	{
		RefreshVisualizerState = RefreshVisualizerState.Idle;
		RefreshInfoProvider?.OnRefreshCompleted();
	}

	private void RaiseRefreshRequested()
	{
		RefreshRequestedEventArgs refreshRequestedEventArgs = new RefreshRequestedEventArgs(RefreshCompleted, RefreshRequestedEvent);
		refreshRequestedEventArgs.IncrementCount();
		RaiseEvent(refreshRequestedEventArgs);
		refreshRequestedEventArgs.DecrementCount();
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (change.Property == RefreshInfoProviderProperty)
		{
			OnRefreshInfoProviderChanged();
		}
		else if (change.Property == ContentControl.ContentProperty)
		{
			if (_root != null && _content != null)
			{
				_root.Children.Insert(0, _content);
				_content.VerticalAlignment = VerticalAlignment.Center;
				_content.HorizontalAlignment = HorizontalAlignment.Center;
			}
			UpdateContent();
		}
		else if (change.Property == OrientationProperty)
		{
			OnOrientationChanged();
			UpdateContent();
		}
		else if (change.Property == Visual.BoundsProperty)
		{
			switch (PullDirection)
			{
			case PullDirection.TopToBottom:
				base.RenderTransform = new TranslateTransform(0.0, 0.0 - base.Bounds.Height);
				break;
			case PullDirection.BottomToTop:
				base.RenderTransform = new TranslateTransform(0.0, base.Bounds.Height);
				break;
			case PullDirection.LeftToRight:
				base.RenderTransform = new TranslateTransform(0.0 - base.Bounds.Width, 0.0);
				break;
			case PullDirection.RightToLeft:
				base.RenderTransform = new TranslateTransform(base.Bounds.Width, 0.0);
				break;
			}
			UpdateContent();
		}
		else if (change.Property == PullDirectionProperty)
		{
			OnOrientationChanged();
			UpdateContent();
		}
	}

	private void OnOrientationChanged()
	{
		switch (_orientation)
		{
		case RefreshVisualizerOrientation.Auto:
			switch (PullDirection)
			{
			case PullDirection.TopToBottom:
			case PullDirection.BottomToTop:
				_startingRotationAngle = 0f;
				break;
			case PullDirection.LeftToRight:
				_startingRotationAngle = -(float)Math.PI / 2f;
				break;
			case PullDirection.RightToLeft:
				_startingRotationAngle = (float)Math.PI / 2f;
				break;
			}
			break;
		case RefreshVisualizerOrientation.Normal:
			_startingRotationAngle = 0f;
			break;
		case RefreshVisualizerOrientation.Rotate90DegreesCounterclockwise:
			_startingRotationAngle = (float)Math.PI / 2f;
			break;
		case RefreshVisualizerOrientation.Rotate270DegreesCounterclockwise:
			_startingRotationAngle = -(float)Math.PI / 2f;
			break;
		}
	}

	private void OnRefreshInfoProviderChanged()
	{
		_isInteractingSubscription?.Dispose();
		_isInteractingSubscription = null;
		_interactionRatioSubscription?.Dispose();
		_interactionRatioSubscription = null;
		if (RefreshInfoProvider != null)
		{
			_isInteractingSubscription = RefreshInfoProvider.GetObservable(Avalonia.Controls.PullToRefresh.RefreshInfoProvider.IsInteractingForRefreshProperty).Subscribe(InteractingForRefreshObserver);
			_interactionRatioSubscription = RefreshInfoProvider.GetObservable(Avalonia.Controls.PullToRefresh.RefreshInfoProvider.InteractionRatioProperty).Subscribe(InteractionRatioObserver);
			_executingRatio = RefreshInfoProvider.ExecutionRatio;
		}
		else
		{
			_executingRatio = 1.0;
		}
	}

	private void InteractionRatioObserver(double obj)
	{
		bool flag = _interactionRatio == 0.0;
		_interactionRatio = obj;
		if (_isInteractingForRefresh)
		{
			if (RefreshVisualizerState == RefreshVisualizerState.Idle)
			{
				if (flag)
				{
					if (_interactionRatio > _executingRatio)
					{
						RefreshVisualizerState = RefreshVisualizerState.Pending;
					}
					else if (_interactionRatio > 0.0)
					{
						RefreshVisualizerState = RefreshVisualizerState.Interacting;
					}
				}
				else if (_interactionRatio > 0.0)
				{
					RefreshVisualizerState = RefreshVisualizerState.Peeking;
				}
			}
			else if (RefreshVisualizerState == RefreshVisualizerState.Interacting)
			{
				if (_interactionRatio <= 0.0)
				{
					RefreshVisualizerState = RefreshVisualizerState.Idle;
				}
				else if (_interactionRatio > _executingRatio)
				{
					RefreshVisualizerState = RefreshVisualizerState.Pending;
				}
				else
				{
					UpdateContent();
				}
			}
			else if (RefreshVisualizerState == RefreshVisualizerState.Pending)
			{
				if (_interactionRatio <= _executingRatio)
				{
					RefreshVisualizerState = RefreshVisualizerState.Interacting;
				}
				else if (_interactionRatio <= 0.0)
				{
					RefreshVisualizerState = RefreshVisualizerState.Idle;
				}
				else
				{
					UpdateContent();
				}
			}
		}
		else if (RefreshVisualizerState != RefreshVisualizerState.Refreshing)
		{
			if (_interactionRatio > 0.0)
			{
				RefreshVisualizerState = RefreshVisualizerState.Peeking;
			}
			else
			{
				RefreshVisualizerState = RefreshVisualizerState.Idle;
			}
		}
	}

	private void InteractingForRefreshObserver(bool obj)
	{
		_isInteractingForRefresh = obj;
		if (!_isInteractingForRefresh)
		{
			switch (_refreshVisualizerState)
			{
			case RefreshVisualizerState.Pending:
				RequestRefresh();
				break;
			default:
				RefreshVisualizerState = RefreshVisualizerState.Idle;
				break;
			case RefreshVisualizerState.Refreshing:
				break;
			}
		}
	}
}
