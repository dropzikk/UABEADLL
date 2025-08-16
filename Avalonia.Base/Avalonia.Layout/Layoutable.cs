using System;
using Avalonia.Collections;
using Avalonia.Logging;
using Avalonia.Reactive;
using Avalonia.VisualTree;

namespace Avalonia.Layout;

public class Layoutable : Visual
{
	public static readonly DirectProperty<Layoutable, Size> DesiredSizeProperty;

	public static readonly StyledProperty<double> WidthProperty;

	public static readonly StyledProperty<double> HeightProperty;

	public static readonly StyledProperty<double> MinWidthProperty;

	public static readonly StyledProperty<double> MaxWidthProperty;

	public static readonly StyledProperty<double> MinHeightProperty;

	public static readonly StyledProperty<double> MaxHeightProperty;

	public static readonly StyledProperty<Thickness> MarginProperty;

	public static readonly StyledProperty<HorizontalAlignment> HorizontalAlignmentProperty;

	public static readonly StyledProperty<VerticalAlignment> VerticalAlignmentProperty;

	public static readonly StyledProperty<bool> UseLayoutRoundingProperty;

	private bool _measuring;

	private Size? _previousMeasure;

	private Rect? _previousArrange;

	private EventHandler<EffectiveViewportChangedEventArgs>? _effectiveViewportChanged;

	private EventHandler? _layoutUpdated;

	public double Width
	{
		get
		{
			return GetValue(WidthProperty);
		}
		set
		{
			SetValue(WidthProperty, value);
		}
	}

	public double Height
	{
		get
		{
			return GetValue(HeightProperty);
		}
		set
		{
			SetValue(HeightProperty, value);
		}
	}

	public double MinWidth
	{
		get
		{
			return GetValue(MinWidthProperty);
		}
		set
		{
			SetValue(MinWidthProperty, value);
		}
	}

	public double MaxWidth
	{
		get
		{
			return GetValue(MaxWidthProperty);
		}
		set
		{
			SetValue(MaxWidthProperty, value);
		}
	}

	public double MinHeight
	{
		get
		{
			return GetValue(MinHeightProperty);
		}
		set
		{
			SetValue(MinHeightProperty, value);
		}
	}

	public double MaxHeight
	{
		get
		{
			return GetValue(MaxHeightProperty);
		}
		set
		{
			SetValue(MaxHeightProperty, value);
		}
	}

	public Thickness Margin
	{
		get
		{
			return GetValue(MarginProperty);
		}
		set
		{
			SetValue(MarginProperty, value);
		}
	}

	public HorizontalAlignment HorizontalAlignment
	{
		get
		{
			return GetValue(HorizontalAlignmentProperty);
		}
		set
		{
			SetValue(HorizontalAlignmentProperty, value);
		}
	}

	public VerticalAlignment VerticalAlignment
	{
		get
		{
			return GetValue(VerticalAlignmentProperty);
		}
		set
		{
			SetValue(VerticalAlignmentProperty, value);
		}
	}

	public Size DesiredSize { get; private set; }

	public bool IsMeasureValid { get; private set; }

	public bool IsArrangeValid { get; private set; }

	public bool UseLayoutRounding
	{
		get
		{
			return GetValue(UseLayoutRoundingProperty);
		}
		set
		{
			SetValue(UseLayoutRoundingProperty, value);
		}
	}

	internal Size? PreviousMeasure => _previousMeasure;

	internal Rect? PreviousArrange => _previousArrange;

	public event EventHandler<EffectiveViewportChangedEventArgs>? EffectiveViewportChanged
	{
		add
		{
			if (_effectiveViewportChanged == null && base.VisualRoot is ILayoutRoot layoutRoot)
			{
				layoutRoot.LayoutManager.RegisterEffectiveViewportListener(this);
			}
			_effectiveViewportChanged = (EventHandler<EffectiveViewportChangedEventArgs>)Delegate.Combine(_effectiveViewportChanged, value);
		}
		remove
		{
			_effectiveViewportChanged = (EventHandler<EffectiveViewportChangedEventArgs>)Delegate.Remove(_effectiveViewportChanged, value);
			if (_effectiveViewportChanged == null && base.VisualRoot is ILayoutRoot layoutRoot)
			{
				layoutRoot.LayoutManager.UnregisterEffectiveViewportListener(this);
			}
		}
	}

	public event EventHandler? LayoutUpdated
	{
		add
		{
			if (_layoutUpdated == null && base.VisualRoot is ILayoutRoot layoutRoot)
			{
				layoutRoot.LayoutManager.LayoutUpdated += LayoutManagedLayoutUpdated;
			}
			_layoutUpdated = (EventHandler)Delegate.Combine(_layoutUpdated, value);
		}
		remove
		{
			_layoutUpdated = (EventHandler)Delegate.Remove(_layoutUpdated, value);
			if (_layoutUpdated == null && base.VisualRoot is ILayoutRoot layoutRoot)
			{
				layoutRoot.LayoutManager.LayoutUpdated -= LayoutManagedLayoutUpdated;
			}
		}
	}

	static Layoutable()
	{
		DesiredSizeProperty = AvaloniaProperty.RegisterDirect("DesiredSize", (Layoutable o) => o.DesiredSize);
		WidthProperty = AvaloniaProperty.Register<Layoutable, double>("Width", double.NaN);
		HeightProperty = AvaloniaProperty.Register<Layoutable, double>("Height", double.NaN);
		MinWidthProperty = AvaloniaProperty.Register<Layoutable, double>("MinWidth", 0.0);
		MaxWidthProperty = AvaloniaProperty.Register<Layoutable, double>("MaxWidth", double.PositiveInfinity);
		MinHeightProperty = AvaloniaProperty.Register<Layoutable, double>("MinHeight", 0.0);
		MaxHeightProperty = AvaloniaProperty.Register<Layoutable, double>("MaxHeight", double.PositiveInfinity);
		MarginProperty = AvaloniaProperty.Register<Layoutable, Thickness>("Margin");
		HorizontalAlignmentProperty = AvaloniaProperty.Register<Layoutable, HorizontalAlignment>("HorizontalAlignment", HorizontalAlignment.Stretch);
		VerticalAlignmentProperty = AvaloniaProperty.Register<Layoutable, VerticalAlignment>("VerticalAlignment", VerticalAlignment.Stretch);
		UseLayoutRoundingProperty = AvaloniaProperty.Register<Layoutable, bool>("UseLayoutRounding", defaultValue: true, inherits: true);
		AffectsMeasure<Layoutable>(new AvaloniaProperty[9] { WidthProperty, HeightProperty, MinWidthProperty, MaxWidthProperty, MinHeightProperty, MaxHeightProperty, MarginProperty, HorizontalAlignmentProperty, VerticalAlignmentProperty });
	}

	public void UpdateLayout()
	{
		(this.GetVisualRoot() as ILayoutRoot)?.LayoutManager?.ExecuteLayoutPass();
	}

	public virtual void ApplyTemplate()
	{
	}

	public void Measure(Size availableSize)
	{
		if (double.IsNaN(availableSize.Width) || double.IsNaN(availableSize.Height))
		{
			throw new InvalidOperationException("Cannot call Measure using a size with NaN values.");
		}
		if (!IsMeasureValid || _previousMeasure != availableSize)
		{
			Size desiredSize = DesiredSize;
			Size size = default(Size);
			IsMeasureValid = true;
			try
			{
				_measuring = true;
				size = MeasureCore(availableSize);
			}
			finally
			{
				_measuring = false;
			}
			if (IsInvalidSize(size))
			{
				throw new InvalidOperationException("Invalid size returned for Measure.");
			}
			DesiredSize = size;
			_previousMeasure = availableSize;
			Logger.TryGet(LogEventLevel.Verbose, "Layout")?.Log(this, "Measure requested {DesiredSize}", DesiredSize);
			if (DesiredSize != desiredSize)
			{
				this.GetVisualParent<Layoutable>()?.ChildDesiredSizeChanged(this);
			}
		}
	}

	public void Arrange(Rect rect)
	{
		if (IsInvalidRect(rect))
		{
			throw new InvalidOperationException("Invalid Arrange rectangle.");
		}
		if (!IsMeasureValid)
		{
			Measure(_previousMeasure ?? rect.Size);
		}
		if (!IsArrangeValid || _previousArrange != rect)
		{
			Logger.TryGet(LogEventLevel.Verbose, "Layout")?.Log(this, "Arrange to {Rect} ", rect);
			IsArrangeValid = true;
			ArrangeCore(rect);
			_previousArrange = rect;
		}
	}

	public void InvalidateMeasure()
	{
		if (IsMeasureValid)
		{
			Logger.TryGet(LogEventLevel.Verbose, "Layout")?.Log(this, "Invalidated measure");
			IsMeasureValid = false;
			IsArrangeValid = false;
			if (base.IsAttachedToVisualTree)
			{
				(base.VisualRoot as ILayoutRoot)?.LayoutManager.InvalidateMeasure(this);
				InvalidateVisual();
			}
			OnMeasureInvalidated();
		}
	}

	public void InvalidateArrange()
	{
		if (IsArrangeValid)
		{
			Logger.TryGet(LogEventLevel.Verbose, "Layout")?.Log(this, "Invalidated arrange");
			IsArrangeValid = false;
			(base.VisualRoot as ILayoutRoot)?.LayoutManager?.InvalidateArrange(this);
			InvalidateVisual();
		}
	}

	internal void ChildDesiredSizeChanged(Layoutable control)
	{
		if (!_measuring)
		{
			InvalidateMeasure();
		}
	}

	internal void RaiseEffectiveViewportChanged(EffectiveViewportChangedEventArgs e)
	{
		_effectiveViewportChanged?.Invoke(this, e);
	}

	protected static void AffectsMeasure<T>(params AvaloniaProperty[] properties) where T : Layoutable
	{
		AnonymousObserver<AvaloniaPropertyChangedEventArgs> observer = new AnonymousObserver<AvaloniaPropertyChangedEventArgs>(delegate(AvaloniaPropertyChangedEventArgs e)
		{
			(e.Sender as T)?.InvalidateMeasure();
		});
		for (int i = 0; i < properties.Length; i++)
		{
			properties[i].Changed.Subscribe(observer);
		}
	}

	protected static void AffectsArrange<T>(params AvaloniaProperty[] properties) where T : Layoutable
	{
		AnonymousObserver<AvaloniaPropertyChangedEventArgs> observer = new AnonymousObserver<AvaloniaPropertyChangedEventArgs>(delegate(AvaloniaPropertyChangedEventArgs e)
		{
			(e.Sender as T)?.InvalidateArrange();
		});
		for (int i = 0; i < properties.Length; i++)
		{
			properties[i].Changed.Subscribe(observer);
		}
	}

	protected virtual Size MeasureCore(Size availableSize)
	{
		if (base.IsVisible)
		{
			Thickness thickness = Margin;
			bool useLayoutRounding = UseLayoutRounding;
			double num = 1.0;
			if (useLayoutRounding)
			{
				num = LayoutHelper.GetLayoutScale(this);
				thickness = LayoutHelper.RoundLayoutThickness(thickness, num, num);
			}
			ApplyStyling();
			ApplyTemplate();
			Size availableSize2 = LayoutHelper.ApplyLayoutConstraints(this, availableSize.Deflate(thickness));
			Size size = MeasureOverride(availableSize2);
			double val = size.Width;
			double val2 = size.Height;
			double width = Width;
			if (!double.IsNaN(width))
			{
				val = width;
			}
			val = Math.Min(val, MaxWidth);
			val = Math.Max(val, MinWidth);
			double height = Height;
			if (!double.IsNaN(height))
			{
				val2 = height;
			}
			val2 = Math.Min(val2, MaxHeight);
			val2 = Math.Max(val2, MinHeight);
			if (useLayoutRounding)
			{
				(val, val2) = (Size)(ref LayoutHelper.RoundLayoutSizeUp(new Size(val, val2), num, num));
			}
			val = Math.Min(val, availableSize.Width);
			val2 = Math.Min(val2, availableSize.Height);
			return NonNegative(new Size(val, val2).Inflate(thickness));
		}
		return default(Size);
	}

	protected virtual Size MeasureOverride(Size availableSize)
	{
		double num = 0.0;
		double num2 = 0.0;
		IAvaloniaList<Visual> visualChildren = base.VisualChildren;
		int count = visualChildren.Count;
		for (int i = 0; i < count; i++)
		{
			if (visualChildren[i] is Layoutable layoutable)
			{
				layoutable.Measure(availableSize);
				num = Math.Max(num, layoutable.DesiredSize.Width);
				num2 = Math.Max(num2, layoutable.DesiredSize.Height);
			}
		}
		return new Size(num, num2);
	}

	protected virtual void ArrangeCore(Rect finalRect)
	{
		if (base.IsVisible)
		{
			bool useLayoutRounding = UseLayoutRounding;
			double layoutScale = LayoutHelper.GetLayoutScale(this);
			Thickness thickness = Margin;
			double num = finalRect.X + thickness.Left;
			double num2 = finalRect.Y + thickness.Top;
			if (useLayoutRounding)
			{
				thickness = LayoutHelper.RoundLayoutThickness(thickness, layoutScale, layoutScale);
			}
			Size size = new Size(Math.Max(0.0, finalRect.Width - thickness.Left - thickness.Right), Math.Max(0.0, finalRect.Height - thickness.Top - thickness.Bottom));
			HorizontalAlignment horizontalAlignment = HorizontalAlignment;
			VerticalAlignment verticalAlignment = VerticalAlignment;
			Size constraints = size;
			if (horizontalAlignment != 0)
			{
				constraints = constraints.WithWidth(Math.Min(constraints.Width, DesiredSize.Width - thickness.Left - thickness.Right));
			}
			if (verticalAlignment != 0)
			{
				constraints = constraints.WithHeight(Math.Min(constraints.Height, DesiredSize.Height - thickness.Top - thickness.Bottom));
			}
			constraints = LayoutHelper.ApplyLayoutConstraints(this, constraints);
			if (useLayoutRounding)
			{
				constraints = LayoutHelper.RoundLayoutSizeUp(constraints, layoutScale, layoutScale);
				size = LayoutHelper.RoundLayoutSizeUp(size, layoutScale, layoutScale);
			}
			constraints = ArrangeOverride(constraints).Constrain(constraints);
			switch (horizontalAlignment)
			{
			case HorizontalAlignment.Stretch:
			case HorizontalAlignment.Center:
				num += (size.Width - constraints.Width) / 2.0;
				break;
			case HorizontalAlignment.Right:
				num += size.Width - constraints.Width;
				break;
			}
			switch (verticalAlignment)
			{
			case VerticalAlignment.Stretch:
			case VerticalAlignment.Center:
				num2 += (size.Height - constraints.Height) / 2.0;
				break;
			case VerticalAlignment.Bottom:
				num2 += size.Height - constraints.Height;
				break;
			}
			if (useLayoutRounding)
			{
				num = LayoutHelper.RoundLayoutValue(num, layoutScale);
				num2 = LayoutHelper.RoundLayoutValue(num2, layoutScale);
			}
			base.Bounds = new Rect(num, num2, constraints.Width, constraints.Height);
		}
	}

	protected virtual Size ArrangeOverride(Size finalSize)
	{
		Rect rect = new Rect(finalSize);
		IAvaloniaList<Visual> visualChildren = base.VisualChildren;
		int count = visualChildren.Count;
		for (int i = 0; i < count; i++)
		{
			if (visualChildren[i] is Layoutable layoutable)
			{
				layoutable.Arrange(rect);
			}
		}
		return finalSize;
	}

	internal sealed override void InvalidateStyles(bool recurse)
	{
		base.InvalidateStyles(recurse);
		InvalidateMeasure();
	}

	protected override void OnAttachedToVisualTreeCore(VisualTreeAttachmentEventArgs e)
	{
		base.OnAttachedToVisualTreeCore(e);
		if (e.Root is ILayoutRoot layoutRoot)
		{
			if (_layoutUpdated != null)
			{
				layoutRoot.LayoutManager.LayoutUpdated += LayoutManagedLayoutUpdated;
			}
			if (_effectiveViewportChanged != null)
			{
				layoutRoot.LayoutManager.RegisterEffectiveViewportListener(this);
			}
		}
	}

	protected override void OnDetachedFromVisualTreeCore(VisualTreeAttachmentEventArgs e)
	{
		if (e.Root is ILayoutRoot layoutRoot)
		{
			if (_layoutUpdated != null)
			{
				layoutRoot.LayoutManager.LayoutUpdated -= LayoutManagedLayoutUpdated;
			}
			if (_effectiveViewportChanged != null)
			{
				layoutRoot.LayoutManager.UnregisterEffectiveViewportListener(this);
			}
		}
		base.OnDetachedFromVisualTreeCore(e);
	}

	protected virtual void OnMeasureInvalidated()
	{
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (!(change.Property == Visual.IsVisibleProperty))
		{
			return;
		}
		DesiredSize = default(Size);
		this.GetVisualParent<Layoutable>()?.ChildDesiredSizeChanged(this);
		if (!change.GetNewValue<bool>())
		{
			return;
		}
		InvalidateMeasure();
		if (base.VisualRoot is ILayoutRoot layoutRoot)
		{
			int count = base.VisualChildren.Count;
			for (int i = 0; i < count; i++)
			{
				(base.VisualChildren[i] as Layoutable)?.AncestorBecameVisible(layoutRoot.LayoutManager);
			}
		}
	}

	protected sealed override void OnVisualParentChanged(Visual? oldParent, Visual? newParent)
	{
		LayoutHelper.InvalidateSelfAndChildrenMeasure(this);
		base.OnVisualParentChanged(oldParent, newParent);
	}

	private protected override void OnControlThemeChanged()
	{
		base.OnControlThemeChanged();
		InvalidateMeasure();
	}

	internal override void OnTemplatedParentControlThemeChanged()
	{
		base.OnTemplatedParentControlThemeChanged();
		InvalidateMeasure();
	}

	private void AncestorBecameVisible(ILayoutManager layoutManager)
	{
		if (base.IsVisible)
		{
			if (!IsMeasureValid)
			{
				layoutManager.InvalidateMeasure(this);
				InvalidateVisual();
			}
			else if (!IsArrangeValid)
			{
				layoutManager.InvalidateArrange(this);
				InvalidateVisual();
			}
			int count = base.VisualChildren.Count;
			for (int i = 0; i < count; i++)
			{
				(base.VisualChildren[i] as Layoutable)?.AncestorBecameVisible(layoutManager);
			}
		}
	}

	private void LayoutManagedLayoutUpdated(object? sender, EventArgs e)
	{
		_layoutUpdated?.Invoke(this, e);
	}

	private static bool IsInvalidRect(Rect rect)
	{
		if (!(rect.Width < 0.0) && !(rect.Height < 0.0) && !double.IsInfinity(rect.X) && !double.IsInfinity(rect.Y) && !double.IsInfinity(rect.Width) && !double.IsInfinity(rect.Height) && !double.IsNaN(rect.X) && !double.IsNaN(rect.Y) && !double.IsNaN(rect.Width))
		{
			return double.IsNaN(rect.Height);
		}
		return true;
	}

	private static bool IsInvalidSize(Size size)
	{
		if (!(size.Width < 0.0) && !(size.Height < 0.0) && !double.IsInfinity(size.Width) && !double.IsInfinity(size.Height) && !double.IsNaN(size.Width))
		{
			return double.IsNaN(size.Height);
		}
		return true;
	}

	private static Size NonNegative(Size size)
	{
		return new Size(Math.Max(size.Width, 0.0), Math.Max(size.Height, 0.0));
	}
}
