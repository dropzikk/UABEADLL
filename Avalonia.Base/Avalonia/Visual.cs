using System;
using System.Collections;
using System.Collections.Specialized;
using Avalonia.Collections;
using Avalonia.Data;
using Avalonia.Logging;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Metadata;
using Avalonia.Reactive;
using Avalonia.Rendering;
using Avalonia.Rendering.Composition;
using Avalonia.Rendering.Composition.Server;
using Avalonia.Utilities;
using Avalonia.VisualTree;

namespace Avalonia;

[UsableDuringInitialization]
public class Visual : StyledElement
{
	public static readonly DirectProperty<Visual, Rect> BoundsProperty;

	public static readonly StyledProperty<bool> ClipToBoundsProperty;

	public static readonly StyledProperty<Geometry?> ClipProperty;

	public static readonly StyledProperty<bool> IsVisibleProperty;

	public static readonly StyledProperty<double> OpacityProperty;

	public static readonly StyledProperty<IBrush?> OpacityMaskProperty;

	public static readonly StyledProperty<IEffect?> EffectProperty;

	public static readonly DirectProperty<Visual, bool> HasMirrorTransformProperty;

	public static readonly StyledProperty<ITransform?> RenderTransformProperty;

	public static readonly StyledProperty<RelativePoint> RenderTransformOriginProperty;

	public static readonly AttachedProperty<FlowDirection> FlowDirectionProperty;

	public static readonly DirectProperty<Visual, Visual?> VisualParentProperty;

	public static readonly StyledProperty<int> ZIndexProperty;

	private static readonly WeakEvent<IAffectsRender, EventArgs> InvalidatedWeakEvent;

	private Rect _bounds;

	private IRenderRoot? _visualRoot;

	private Visual? _visualParent;

	private bool _hasMirrorTransform;

	private TargetWeakEventSubscriber<Visual, EventArgs>? _affectsRenderWeakSubscriber;

	public Rect Bounds
	{
		get
		{
			return _bounds;
		}
		protected set
		{
			SetAndRaise(BoundsProperty, ref _bounds, value);
		}
	}

	public bool ClipToBounds
	{
		get
		{
			return GetValue(ClipToBoundsProperty);
		}
		set
		{
			SetValue(ClipToBoundsProperty, value);
		}
	}

	public Geometry? Clip
	{
		get
		{
			return GetValue(ClipProperty);
		}
		set
		{
			SetValue(ClipProperty, value);
		}
	}

	public bool IsEffectivelyVisible
	{
		get
		{
			for (Visual visual = this; visual != null; visual = visual.VisualParent)
			{
				if (!visual.IsVisible)
				{
					return false;
				}
			}
			return true;
		}
	}

	public bool IsVisible
	{
		get
		{
			return GetValue(IsVisibleProperty);
		}
		set
		{
			SetValue(IsVisibleProperty, value);
		}
	}

	public double Opacity
	{
		get
		{
			return GetValue(OpacityProperty);
		}
		set
		{
			SetValue(OpacityProperty, value);
		}
	}

	public IBrush? OpacityMask
	{
		get
		{
			return GetValue(OpacityMaskProperty);
		}
		set
		{
			SetValue(OpacityMaskProperty, value);
		}
	}

	public IEffect? Effect
	{
		get
		{
			return GetValue(EffectProperty);
		}
		set
		{
			SetValue(EffectProperty, value);
		}
	}

	public bool HasMirrorTransform
	{
		get
		{
			return _hasMirrorTransform;
		}
		protected set
		{
			SetAndRaise(HasMirrorTransformProperty, ref _hasMirrorTransform, value);
		}
	}

	public ITransform? RenderTransform
	{
		get
		{
			return GetValue(RenderTransformProperty);
		}
		set
		{
			SetValue(RenderTransformProperty, value);
		}
	}

	public RelativePoint RenderTransformOrigin
	{
		get
		{
			return GetValue(RenderTransformOriginProperty);
		}
		set
		{
			SetValue(RenderTransformOriginProperty, value);
		}
	}

	public FlowDirection FlowDirection
	{
		get
		{
			return GetValue(FlowDirectionProperty);
		}
		set
		{
			SetValue(FlowDirectionProperty, value);
		}
	}

	public int ZIndex
	{
		get
		{
			return GetValue(ZIndexProperty);
		}
		set
		{
			SetValue(ZIndexProperty, value);
		}
	}

	protected internal IAvaloniaList<Visual> VisualChildren { get; private set; }

	protected internal IRenderRoot? VisualRoot => _visualRoot ?? (this as IRenderRoot);

	internal CompositionDrawListVisual? CompositionVisual { get; private set; }

	internal CompositionVisual? ChildCompositionVisual { get; set; }

	internal RenderOptions RenderOptions { get; set; }

	internal bool HasNonUniformZIndexChildren { get; private set; }

	internal bool IsAttachedToVisualTree => VisualRoot != null;

	internal Visual? VisualParent => _visualParent;

	protected virtual bool BypassFlowDirectionPolicies => false;

	public event EventHandler<VisualTreeAttachmentEventArgs>? AttachedToVisualTree;

	public event EventHandler<VisualTreeAttachmentEventArgs>? DetachedFromVisualTree;

	static Visual()
	{
		BoundsProperty = AvaloniaProperty.RegisterDirect("Bounds", (Visual o) => o.Bounds);
		ClipToBoundsProperty = AvaloniaProperty.Register<Visual, bool>("ClipToBounds", defaultValue: false);
		ClipProperty = AvaloniaProperty.Register<Visual, Geometry>("Clip");
		IsVisibleProperty = AvaloniaProperty.Register<Visual, bool>("IsVisible", defaultValue: true);
		OpacityProperty = AvaloniaProperty.Register<Visual, double>("Opacity", 1.0);
		OpacityMaskProperty = AvaloniaProperty.Register<Visual, IBrush>("OpacityMask");
		EffectProperty = AvaloniaProperty.Register<Visual, IEffect>("Effect");
		HasMirrorTransformProperty = AvaloniaProperty.RegisterDirect("HasMirrorTransform", (Visual o) => o.HasMirrorTransform, null, unsetValue: false);
		RenderTransformProperty = AvaloniaProperty.Register<Visual, ITransform>("RenderTransform");
		RenderTransformOriginProperty = AvaloniaProperty.Register<Visual, RelativePoint>("RenderTransformOrigin", RelativePoint.Center);
		FlowDirectionProperty = AvaloniaProperty.RegisterAttached<Visual, Visual, FlowDirection>("FlowDirection", FlowDirection.LeftToRight, inherits: true);
		VisualParentProperty = AvaloniaProperty.RegisterDirect("VisualParent", (Visual o) => o._visualParent);
		ZIndexProperty = AvaloniaProperty.Register<Visual, int>("ZIndex", 0);
		InvalidatedWeakEvent = WeakEvent.Register(delegate(IAffectsRender s, EventHandler h)
		{
			s.Invalidated += h;
		}, delegate(IAffectsRender s, EventHandler h)
		{
			s.Invalidated -= h;
		});
		AffectsRender<Visual>(new AvaloniaProperty[8] { BoundsProperty, ClipProperty, ClipToBoundsProperty, IsVisibleProperty, OpacityProperty, OpacityMaskProperty, EffectProperty, HasMirrorTransformProperty });
		RenderTransformProperty.Changed.Subscribe(RenderTransformChanged);
		ZIndexProperty.Changed.Subscribe(ZIndexChanged);
	}

	public Visual()
	{
		DisableTransitions();
		AvaloniaList<Visual> avaloniaList = new AvaloniaList<Visual>();
		avaloniaList.ResetBehavior = ResetBehavior.Remove;
		avaloniaList.Validate = delegate(Visual visual)
		{
			ValidateVisualChild(visual);
		};
		avaloniaList.CollectionChanged += VisualChildrenChanged;
		VisualChildren = avaloniaList;
	}

	public static FlowDirection GetFlowDirection(Visual visual)
	{
		return visual.GetValue(FlowDirectionProperty);
	}

	public static void SetFlowDirection(Visual visual, FlowDirection value)
	{
		visual.SetValue(FlowDirectionProperty, value);
	}

	public void InvalidateVisual()
	{
		VisualRoot?.Renderer.AddDirty(this);
	}

	public virtual void Render(DrawingContext context)
	{
		if (context == null)
		{
			throw new ArgumentNullException("context");
		}
	}

	protected static void AffectsRender<T>(params AvaloniaProperty[] properties) where T : Visual
	{
		AnonymousObserver<AvaloniaPropertyChangedEventArgs> observer = new AnonymousObserver<AvaloniaPropertyChangedEventArgs>(delegate(AvaloniaPropertyChangedEventArgs e)
		{
			if (e.Sender is T val)
			{
				val.InvalidateVisual();
			}
		});
		AnonymousObserver<AvaloniaPropertyChangedEventArgs> observer2 = new AnonymousObserver<AvaloniaPropertyChangedEventArgs>(delegate(AvaloniaPropertyChangedEventArgs e)
		{
			if (e.Sender is T val2)
			{
				if (e.OldValue is IAffectsRender target2 && val2._affectsRenderWeakSubscriber != null)
				{
					InvalidatedWeakEvent.Unsubscribe(target2, val2._affectsRenderWeakSubscriber);
				}
				if (e.NewValue is IAffectsRender target3)
				{
					if (val2._affectsRenderWeakSubscriber == null)
					{
						val2._affectsRenderWeakSubscriber = new TargetWeakEventSubscriber<Visual, EventArgs>(val2, delegate(Visual target, object? _, WeakEvent _, EventArgs _)
						{
							target.InvalidateVisual();
						});
					}
					InvalidatedWeakEvent.Subscribe(target3, val2._affectsRenderWeakSubscriber);
				}
				val2.InvalidateVisual();
			}
		});
		foreach (AvaloniaProperty avaloniaProperty in properties)
		{
			if (avaloniaProperty.CanValueAffectRender())
			{
				avaloniaProperty.Changed.Subscribe(observer2);
			}
			else
			{
				avaloniaProperty.Changed.Subscribe(observer);
			}
		}
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (!(change.Property == FlowDirectionProperty))
		{
			return;
		}
		InvalidateMirrorTransform();
		foreach (Visual visualChild in VisualChildren)
		{
			visualChild.InvalidateMirrorTransform();
		}
	}

	protected override void LogicalChildrenCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
	{
		base.LogicalChildrenCollectionChanged(sender, e);
		VisualRoot?.Renderer.RecalculateChildren(this);
	}

	protected virtual void OnAttachedToVisualTreeCore(VisualTreeAttachmentEventArgs e)
	{
		Logger.TryGet(LogEventLevel.Verbose, "Visual")?.Log(this, "Attached to visual tree");
		_visualRoot = e.Root;
		if (RenderTransform is IMutableTransform mutableTransform)
		{
			mutableTransform.Changed += RenderTransformChanged;
		}
		EnableTransitions();
		if (_visualRoot.Renderer is IRendererWithCompositor rendererWithCompositor)
		{
			AttachToCompositor(rendererWithCompositor.Compositor);
		}
		InvalidateMirrorTransform();
		OnAttachedToVisualTree(e);
		this.AttachedToVisualTree?.Invoke(this, e);
		InvalidateVisual();
		_visualRoot.Renderer.RecalculateChildren(_visualParent);
		if (ZIndex != 0)
		{
			Visual visualParent = VisualParent;
			if (visualParent != null)
			{
				visualParent.HasNonUniformZIndexChildren = true;
			}
		}
		IAvaloniaList<Visual> visualChildren = VisualChildren;
		int count = visualChildren.Count;
		for (int i = 0; i < count; i++)
		{
			Visual visual = visualChildren[i];
			if (visual != null && visual._visualRoot != e.Root)
			{
				visual.OnAttachedToVisualTreeCore(e);
			}
		}
	}

	private protected virtual CompositionDrawListVisual CreateCompositionVisual(Compositor compositor)
	{
		return new CompositionDrawListVisual(compositor, new ServerCompositionDrawListVisual(compositor.Server, this), this);
	}

	internal CompositionVisual AttachToCompositor(Compositor compositor)
	{
		if (CompositionVisual == null || CompositionVisual.Compositor != compositor)
		{
			CompositionVisual = CreateCompositionVisual(compositor);
		}
		return CompositionVisual;
	}

	protected virtual void OnDetachedFromVisualTreeCore(VisualTreeAttachmentEventArgs e)
	{
		Logger.TryGet(LogEventLevel.Verbose, "Visual")?.Log(this, "Detached from visual tree");
		_visualRoot = null;
		if (RenderTransform is IMutableTransform mutableTransform)
		{
			mutableTransform.Changed -= RenderTransformChanged;
		}
		DisableTransitions();
		OnDetachedFromVisualTree(e);
		if (CompositionVisual != null)
		{
			if (ChildCompositionVisual != null)
			{
				CompositionVisual.Children.Remove(ChildCompositionVisual);
			}
			CompositionVisual.DrawList = null;
			CompositionVisual = null;
		}
		this.DetachedFromVisualTree?.Invoke(this, e);
		e.Root.Renderer.AddDirty(this);
		IAvaloniaList<Visual> visualChildren = VisualChildren;
		int count = visualChildren.Count;
		for (int i = 0; i < count; i++)
		{
			visualChildren[i]?.OnDetachedFromVisualTreeCore(e);
		}
	}

	protected virtual void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
	{
	}

	protected virtual void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
	{
	}

	protected virtual void OnVisualParentChanged(Visual? oldParent, Visual? newParent)
	{
		RaisePropertyChanged(VisualParentProperty, oldParent, newParent);
	}

	internal override ParametrizedLogger? GetBindingWarningLogger(AvaloniaProperty property, Exception? e)
	{
		if (!((ILogical)this).IsAttachedToLogicalTree)
		{
			return null;
		}
		if (e is BindingChainException ex && string.IsNullOrEmpty(ex.ExpressionErrorPoint) && base.DataContext == null)
		{
			return null;
		}
		return Logger.TryGet(LogEventLevel.Warning, "Binding");
	}

	private static void RenderTransformChanged(AvaloniaPropertyChangedEventArgs<ITransform?> e)
	{
		Visual visual = e.Sender as Visual;
		if (visual?.VisualRoot != null)
		{
			var (transform, transform2) = e.GetOldAndNewValue<ITransform>();
			if (transform is Transform transform3)
			{
				transform3.Changed -= visual.RenderTransformChanged;
			}
			if (transform2 is Transform transform4)
			{
				transform4.Changed += visual.RenderTransformChanged;
			}
			visual.InvalidateVisual();
		}
	}

	private static void ValidateVisualChild(Visual c)
	{
		if (c == null)
		{
			throw new ArgumentNullException("c", "Cannot add null to VisualChildren.");
		}
		if (c.VisualParent != null)
		{
			throw new InvalidOperationException("The control already has a visual parent.");
		}
	}

	private static void ZIndexChanged(AvaloniaPropertyChangedEventArgs e)
	{
		Visual obj = e.Sender as Visual;
		Visual visual = obj?.VisualParent;
		if ((obj == null || obj.ZIndex != 0) && visual != null)
		{
			Visual visual2 = visual;
			visual2.HasNonUniformZIndexChildren = true;
		}
		obj?.InvalidateVisual();
		visual?.VisualRoot?.Renderer.RecalculateChildren(visual);
	}

	private void RenderTransformChanged(object? sender, EventArgs e)
	{
		InvalidateVisual();
	}

	private void SetVisualParent(Visual? value)
	{
		if (_visualParent == value)
		{
			return;
		}
		Visual visualParent = _visualParent;
		_visualParent = value;
		if (_visualRoot != null)
		{
			VisualTreeAttachmentEventArgs e = new VisualTreeAttachmentEventArgs(visualParent, _visualRoot);
			OnDetachedFromVisualTreeCore(e);
		}
		if (!(_visualParent is IRenderRoot))
		{
			Visual? visualParent2 = _visualParent;
			if (visualParent2 == null || !visualParent2.IsAttachedToVisualTree)
			{
				goto IL_0080;
			}
		}
		IRenderRoot root = this.FindAncestorOfType<IRenderRoot>() ?? throw new AvaloniaInternalException("Visual is atached to visual tree but root could not be found.");
		VisualTreeAttachmentEventArgs e2 = new VisualTreeAttachmentEventArgs(_visualParent, root);
		OnAttachedToVisualTreeCore(e2);
		goto IL_0080;
		IL_0080:
		OnVisualParentChanged(visualParent, value);
	}

	private void VisualChildrenChanged(object? sender, NotifyCollectionChangedEventArgs e)
	{
		switch (e.Action)
		{
		case NotifyCollectionChangedAction.Add:
			SetVisualParent(e.NewItems, this);
			break;
		case NotifyCollectionChangedAction.Remove:
			SetVisualParent(e.OldItems, null);
			break;
		case NotifyCollectionChangedAction.Replace:
			SetVisualParent(e.OldItems, null);
			SetVisualParent(e.NewItems, this);
			break;
		}
	}

	private static void SetVisualParent(IList children, Visual? parent)
	{
		int count = children.Count;
		for (int i = 0; i < count; i++)
		{
			((Visual)children[i]).SetVisualParent(parent);
		}
	}

	internal override void OnTemplatedParentControlThemeChanged()
	{
		base.OnTemplatedParentControlThemeChanged();
		int count = VisualChildren.Count;
		AvaloniaObject templatedParent = base.TemplatedParent;
		for (int i = 0; i < count; i++)
		{
			StyledElement styledElement = VisualChildren[i];
			if (styledElement != null && styledElement.TemplatedParent == templatedParent)
			{
				styledElement.OnTemplatedParentControlThemeChanged();
			}
		}
	}

	protected internal virtual void InvalidateMirrorTransform()
	{
		FlowDirection flowDirection = FlowDirection;
		FlowDirection flowDirection2 = FlowDirection.LeftToRight;
		bool bypassFlowDirectionPolicies = BypassFlowDirectionPolicies;
		bool flag = false;
		Visual visualParent = VisualParent;
		if (visualParent != null)
		{
			flowDirection2 = visualParent.FlowDirection;
			flag = visualParent.BypassFlowDirectionPolicies;
		}
		bool num = flowDirection == FlowDirection.RightToLeft && !bypassFlowDirectionPolicies;
		bool flag2 = flowDirection2 == FlowDirection.RightToLeft && !flag;
		bool hasMirrorTransform = num != flag2;
		HasMirrorTransform = hasMirrorTransform;
	}
}
