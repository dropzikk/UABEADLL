using System;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Rendering.Composition;
using Avalonia.Rendering.Composition.Animations;
using Avalonia.VisualTree;

namespace Avalonia.Controls.PullToRefresh;

internal class ScrollViewerIRefreshInfoProviderAdapter
{
	private const int MaxSearchDepth = 10;

	private const int InitialOffsetThreshold = 1;

	private PullDirection _refreshPullDirection;

	private ScrollViewer? _scrollViewer;

	private RefreshInfoProvider? _refreshInfoProvider;

	private PullGestureRecognizer? _pullGestureRecognizer;

	private InputElement? _interactionSource;

	private bool _isVisualizerInteractionSourceAttached;

	public ScrollViewerIRefreshInfoProviderAdapter(PullDirection pullDirection)
	{
		_refreshPullDirection = pullDirection;
	}

	public RefreshInfoProvider? AdaptFromTree(Visual root, Size? refreshVIsualizerSize)
	{
		if (root is ScrollViewer adaptee)
		{
			return Adapt(adaptee, refreshVIsualizerSize);
		}
		for (int i = 0; i < 10; i++)
		{
			ScrollViewer scrollViewer = AdaptFromTreeRecursiveHelper(root, i);
			if (scrollViewer != null)
			{
				return Adapt(scrollViewer, refreshVIsualizerSize);
			}
		}
		return null;
		static ScrollViewer? AdaptFromTreeRecursiveHelper(Visual root, int depth)
		{
			if (depth == 0)
			{
				foreach (Visual visualChild in root.VisualChildren)
				{
					if (visualChild is ScrollViewer result)
					{
						return result;
					}
				}
			}
			else
			{
				foreach (Visual visualChild2 in root.VisualChildren)
				{
					ScrollViewer scrollViewer2 = AdaptFromTreeRecursiveHelper(visualChild2, depth - 1);
					if (scrollViewer2 != null)
					{
						return scrollViewer2;
					}
				}
			}
			return null;
		}
	}

	public RefreshInfoProvider Adapt(ScrollViewer adaptee, Size? refreshVIsualizerSize)
	{
		if (adaptee == null)
		{
			throw new ArgumentNullException("adaptee", "Adaptee cannot be null");
		}
		if (_scrollViewer != null)
		{
			CleanUpScrollViewer();
		}
		if (_refreshInfoProvider != null && _interactionSource != null)
		{
			_interactionSource.RemoveHandler(Gestures.PullGestureEvent, _refreshInfoProvider.InteractingStateEntered);
			_interactionSource.RemoveHandler(Gestures.PullGestureEndedEvent, _refreshInfoProvider.InteractingStateExited);
		}
		_refreshInfoProvider = null;
		_scrollViewer = adaptee;
		if (_scrollViewer.Content == null)
		{
			throw new ArgumentException("Adaptee's content property cannot be null.", "adaptee");
		}
		if (!(adaptee.Content is Visual visual))
		{
			throw new ArgumentException("Adaptee's content property must be a Visual", "adaptee");
		}
		if (visual.GetVisualParent() == null)
		{
			_scrollViewer.Loaded += ScrollViewer_Loaded;
		}
		else
		{
			ScrollViewer_Loaded(null, null);
			if (!(visual.Parent is InputElement))
			{
				throw new ArgumentException("Adaptee's content's parent must be a InputElement", "adaptee");
			}
		}
		_refreshInfoProvider = new RefreshInfoProvider(_refreshPullDirection, refreshVIsualizerSize, ElementComposition.GetElementVisual(visual));
		_pullGestureRecognizer = new PullGestureRecognizer(_refreshPullDirection);
		if (_interactionSource != null)
		{
			_interactionSource.GestureRecognizers.Add(_pullGestureRecognizer);
			_interactionSource.AddHandler(Gestures.PullGestureEvent, _refreshInfoProvider.InteractingStateEntered);
			_interactionSource.AddHandler(Gestures.PullGestureEndedEvent, _refreshInfoProvider.InteractingStateExited);
			_isVisualizerInteractionSourceAttached = true;
		}
		_scrollViewer.PointerPressed += ScrollViewer_PointerPressed;
		_scrollViewer.PointerReleased += ScrollViewer_PointerReleased;
		_scrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;
		return _refreshInfoProvider;
	}

	private void ScrollViewer_ScrollChanged(object? sender, ScrollChangedEventArgs e)
	{
		if (_isVisualizerInteractionSourceAttached && _refreshInfoProvider != null && _refreshInfoProvider.IsInteractingForRefresh && !IsWithinOffsetThreashold())
		{
			_refreshInfoProvider.IsInteractingForRefresh = false;
		}
	}

	public void SetAnimations(RefreshVisualizer refreshVisualizer)
	{
		CompositionVisual elementVisual = ElementComposition.GetElementVisual(refreshVisualizer);
		if (elementVisual != null)
		{
			Compositor compositor = elementVisual.Compositor;
			Vector3KeyFrameAnimation vector3KeyFrameAnimation = compositor.CreateVector3KeyFrameAnimation();
			vector3KeyFrameAnimation.Target = "Offset";
			vector3KeyFrameAnimation.InsertExpressionKeyFrame(1f, "this.FinalValue");
			vector3KeyFrameAnimation.Duration = TimeSpan.FromMilliseconds(150.0);
			ImplicitAnimationCollection implicitAnimationCollection = compositor.CreateImplicitAnimationCollection();
			implicitAnimationCollection["Offset"] = vector3KeyFrameAnimation;
			elementVisual.ImplicitAnimations = implicitAnimationCollection;
		}
		if (_scrollViewer != null)
		{
			CompositionVisual elementVisual2 = ElementComposition.GetElementVisual(_scrollViewer);
			if (elementVisual2 != null)
			{
				Compositor compositor2 = elementVisual2.Compositor;
				Vector3KeyFrameAnimation vector3KeyFrameAnimation2 = compositor2.CreateVector3KeyFrameAnimation();
				vector3KeyFrameAnimation2.Target = "Offset";
				vector3KeyFrameAnimation2.InsertExpressionKeyFrame(1f, "this.FinalValue");
				vector3KeyFrameAnimation2.Duration = TimeSpan.FromMilliseconds(150.0);
				ImplicitAnimationCollection implicitAnimationCollection2 = compositor2.CreateImplicitAnimationCollection();
				implicitAnimationCollection2["Offset"] = vector3KeyFrameAnimation2;
				elementVisual2.ImplicitAnimations = implicitAnimationCollection2;
			}
		}
	}

	private void ScrollViewer_Loaded(object? sender, RoutedEventArgs? e)
	{
		if (!(((_scrollViewer?.Content as Visual) ?? throw new ArgumentException("Adaptee's content property must be a Visual", "_scrollViewer")).Parent is InputElement element))
		{
			throw new ArgumentException("Adaptee's content parent must be an InputElement", "_scrollViewer");
		}
		MakeInteractionSource(element);
		if (_scrollViewer != null)
		{
			_scrollViewer.Loaded -= ScrollViewer_Loaded;
		}
	}

	private void MakeInteractionSource(InputElement? element)
	{
		_interactionSource = element;
		if (_pullGestureRecognizer != null && _refreshInfoProvider != null)
		{
			element?.GestureRecognizers.Add(_pullGestureRecognizer);
			_interactionSource?.AddHandler(Gestures.PullGestureEvent, _refreshInfoProvider.InteractingStateEntered);
			_interactionSource?.AddHandler(Gestures.PullGestureEndedEvent, _refreshInfoProvider.InteractingStateExited);
			_isVisualizerInteractionSourceAttached = true;
		}
	}

	private void ScrollViewer_PointerReleased(object? sender, PointerReleasedEventArgs e)
	{
		if (_refreshInfoProvider != null)
		{
			_refreshInfoProvider.IsInteractingForRefresh = false;
		}
	}

	private void ScrollViewer_PointerPressed(object? sender, PointerPressedEventArgs e)
	{
		if (_refreshInfoProvider != null)
		{
			_refreshInfoProvider.PeekingMode = !IsWithinOffsetThreashold();
		}
	}

	private bool IsWithinOffsetThreashold()
	{
		if (_scrollViewer != null)
		{
			Vector offset = _scrollViewer.Offset;
			switch (_refreshPullDirection)
			{
			case PullDirection.TopToBottom:
				return offset.Y < 1.0;
			case PullDirection.LeftToRight:
				return offset.X < 1.0;
			case PullDirection.RightToLeft:
				return offset.X > _scrollViewer.Extent.Width - _scrollViewer.Viewport.Width - 1.0;
			case PullDirection.BottomToTop:
				return offset.Y > _scrollViewer.Extent.Height - _scrollViewer.Viewport.Height - 1.0;
			}
		}
		return false;
	}

	private void CleanUpScrollViewer()
	{
		if (_scrollViewer != null)
		{
			_scrollViewer.PointerPressed -= ScrollViewer_PointerPressed;
			_scrollViewer.PointerReleased -= ScrollViewer_PointerReleased;
			_scrollViewer.ScrollChanged -= ScrollViewer_ScrollChanged;
		}
	}
}
