using System;
using System.Collections.Specialized;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Reactive;
using Avalonia.VisualTree;

namespace Avalonia.Controls.Primitives;

public class AdornerLayer : Canvas
{
	private class AdornedElementInfo
	{
		public IDisposable? Subscription { get; set; }

		public TransformedBounds? Bounds { get; set; }
	}

	public static readonly AttachedProperty<Visual?> AdornedElementProperty;

	public static readonly AttachedProperty<bool> IsClipEnabledProperty;

	public static readonly AttachedProperty<Control?> AdornerProperty;

	public static readonly StyledProperty<ITemplate<Control>?> DefaultFocusAdornerProperty;

	private static readonly AttachedProperty<AdornedElementInfo?> s_adornedElementInfoProperty;

	private static readonly AttachedProperty<AdornerLayer?> s_savedAdornerLayerProperty;

	public ITemplate<Control>? DefaultFocusAdorner
	{
		get
		{
			return GetValue(DefaultFocusAdornerProperty);
		}
		set
		{
			SetValue(DefaultFocusAdornerProperty, value);
		}
	}

	static AdornerLayer()
	{
		AdornedElementProperty = AvaloniaProperty.RegisterAttached<AdornerLayer, Visual, Visual>("AdornedElement");
		IsClipEnabledProperty = AvaloniaProperty.RegisterAttached<AdornerLayer, Visual, bool>("IsClipEnabled", defaultValue: true);
		AdornerProperty = AvaloniaProperty.RegisterAttached<AdornerLayer, Visual, Control>("Adorner");
		DefaultFocusAdornerProperty = AvaloniaProperty.Register<AdornerLayer, ITemplate<Control>>("DefaultFocusAdorner");
		s_adornedElementInfoProperty = AvaloniaProperty.RegisterAttached<AdornerLayer, Visual, AdornedElementInfo>("AdornedElementInfo");
		s_savedAdornerLayerProperty = AvaloniaProperty.RegisterAttached<Visual, Visual, AdornerLayer>("SavedAdornerLayer");
		AdornedElementProperty.Changed.Subscribe(AdornedElementChanged);
		AdornerProperty.Changed.Subscribe(AdornerChanged);
	}

	public AdornerLayer()
	{
		base.Children.CollectionChanged += ChildrenCollectionChanged;
	}

	public static Visual? GetAdornedElement(Visual adorner)
	{
		return adorner.GetValue(AdornedElementProperty);
	}

	public static void SetAdornedElement(Visual adorner, Visual adorned)
	{
		adorner.SetValue(AdornedElementProperty, adorned);
	}

	public static AdornerLayer? GetAdornerLayer(Visual visual)
	{
		return visual.FindAncestorOfType<VisualLayerManager>()?.AdornerLayer;
	}

	public static bool GetIsClipEnabled(Visual adorner)
	{
		return adorner.GetValue(IsClipEnabledProperty);
	}

	public static void SetIsClipEnabled(Visual adorner, bool isClipEnabled)
	{
		adorner.SetValue(IsClipEnabledProperty, isClipEnabled);
	}

	public static Control? GetAdorner(Visual visual)
	{
		return visual.GetValue(AdornerProperty);
	}

	public static void SetAdorner(Visual visual, Control? adorner)
	{
		visual.SetValue(AdornerProperty, adorner);
	}

	private static void AdornerChanged(AvaloniaPropertyChangedEventArgs<Control?> e)
	{
		if (!(e.Sender is Visual visual))
		{
			return;
		}
		Control valueOrDefault = e.OldValue.GetValueOrDefault();
		Control valueOrDefault2 = e.NewValue.GetValueOrDefault();
		if (!object.Equals(valueOrDefault, valueOrDefault2))
		{
			if (valueOrDefault != null)
			{
				visual.AttachedToVisualTree -= VisualOnAttachedToVisualTree;
				visual.DetachedFromVisualTree -= VisualOnDetachedFromVisualTree;
				Detach(visual, valueOrDefault);
			}
			if (valueOrDefault2 != null)
			{
				visual.AttachedToVisualTree += VisualOnAttachedToVisualTree;
				visual.DetachedFromVisualTree += VisualOnDetachedFromVisualTree;
				Attach(visual, valueOrDefault2);
			}
		}
	}

	private static void VisualOnAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
	{
		if (sender is Visual visual)
		{
			Control adorner = GetAdorner(visual);
			if (adorner != null)
			{
				Attach(visual, adorner);
			}
		}
	}

	private static void VisualOnDetachedFromVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
	{
		if (sender is Visual visual)
		{
			Control adorner = GetAdorner(visual);
			if (adorner != null)
			{
				Detach(visual, adorner);
			}
		}
	}

	private static void Attach(Visual visual, Control adorner)
	{
		AdornerLayer adornerLayer = GetAdornerLayer(visual);
		AddVisualAdorner(visual, adorner, adornerLayer);
		visual.SetValue(s_savedAdornerLayerProperty, adornerLayer);
	}

	private static void Detach(Visual visual, Control adorner)
	{
		AdornerLayer value = visual.GetValue(s_savedAdornerLayerProperty);
		RemoveVisualAdorner(visual, adorner, value);
		visual.ClearValue(s_savedAdornerLayerProperty);
	}

	private static void AddVisualAdorner(Visual visual, Control? adorner, AdornerLayer? layer)
	{
		if (adorner != null && layer != null && !layer.Children.Contains(adorner))
		{
			SetAdornedElement(adorner, visual);
			SetIsClipEnabled(adorner, isClipEnabled: false);
			((ISetLogicalParent)adorner).SetParent((ILogical?)visual);
			layer.Children.Add(adorner);
		}
	}

	private static void RemoveVisualAdorner(Visual visual, Control? adorner, AdornerLayer? layer)
	{
		if (adorner != null && layer != null && layer.Children.Contains(adorner))
		{
			layer.Children.Remove(adorner);
			((ISetLogicalParent)adorner).SetParent((ILogical?)null);
		}
	}

	protected override Size MeasureOverride(Size availableSize)
	{
		foreach (Control child in base.Children)
		{
			AvaloniaObject avaloniaObject = child;
			if (avaloniaObject != null)
			{
				AdornedElementInfo value = avaloniaObject.GetValue(s_adornedElementInfoProperty);
				if (value != null && value.Bounds.HasValue)
				{
					child.Measure(value.Bounds.Value.Bounds.Size);
				}
				else
				{
					child.Measure(availableSize);
				}
			}
		}
		return default(Size);
	}

	protected override Size ArrangeOverride(Size finalSize)
	{
		foreach (Control child in base.Children)
		{
			AvaloniaObject avaloniaObject = child;
			if (avaloniaObject != null)
			{
				AdornedElementInfo value = avaloniaObject.GetValue(s_adornedElementInfoProperty);
				bool value2 = avaloniaObject.GetValue(IsClipEnabledProperty);
				if (value != null && value.Bounds.HasValue)
				{
					child.RenderTransform = new MatrixTransform(value.Bounds.Value.Transform);
					child.RenderTransformOrigin = new RelativePoint(new Point(0.0, 0.0), RelativeUnit.Absolute);
					UpdateClip(child, value.Bounds.Value, value2);
					child.Arrange(value.Bounds.Value.Bounds);
				}
				else
				{
					ArrangeChild(child, finalSize);
				}
			}
		}
		return finalSize;
	}

	private static void AdornedElementChanged(AvaloniaPropertyChangedEventArgs<Visual?> e)
	{
		Visual visual = (Visual)e.Sender;
		Visual valueOrDefault = e.NewValue.GetValueOrDefault();
		visual.GetVisualParent<AdornerLayer>()?.UpdateAdornedElement(visual, valueOrDefault);
	}

	private void UpdateClip(Control control, TransformedBounds bounds, bool isEnabled)
	{
		if (!isEnabled)
		{
			control.Clip = null;
			return;
		}
		RectangleGeometry rectangleGeometry = control.Clip as RectangleGeometry;
		if (rectangleGeometry == null)
		{
			rectangleGeometry = (RectangleGeometry)(control.Clip = new RectangleGeometry());
		}
		Rect rect = bounds.Bounds;
		if (bounds.Transform.HasInverse)
		{
			rect = bounds.Clip.TransformToAABB(bounds.Transform.Invert());
		}
		rectangleGeometry.Rect = rect;
	}

	private void ChildrenCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
	{
		if (e.Action == NotifyCollectionChangedAction.Add)
		{
			foreach (Visual newItem in e.NewItems)
			{
				UpdateAdornedElement(newItem, newItem.GetValue(AdornedElementProperty));
			}
		}
		InvalidateArrange();
	}

	private void UpdateAdornedElement(Visual adorner, Visual? adorned)
	{
		if (adorner.CompositionVisual != null)
		{
			adorner.CompositionVisual.AdornedVisual = adorned?.CompositionVisual;
			adorner.CompositionVisual.AdornerIsClipped = GetIsClipEnabled(adorner);
		}
		AdornedElementInfo info = adorner.GetValue(s_adornedElementInfoProperty);
		if (info != null)
		{
			info.Subscription.Dispose();
			if (adorned == null)
			{
				adorner.ClearValue(s_adornedElementInfoProperty);
			}
		}
		if (adorned == null)
		{
			return;
		}
		if (info == null)
		{
			info = new AdornedElementInfo();
			adorner.SetValue(s_adornedElementInfoProperty, info);
		}
		if (adorner.CompositionVisual != null)
		{
			info.Subscription = adorned.GetObservable(Visual.BoundsProperty).Subscribe(delegate
			{
				info.Bounds = new TransformedBounds(new Rect(adorned.Bounds.Size), new Rect(adorned.Bounds.Size), Matrix.Identity);
				InvalidateMeasure();
			});
		}
	}
}
