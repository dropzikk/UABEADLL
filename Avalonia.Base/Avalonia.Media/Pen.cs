using System;
using Avalonia.Media.Immutable;
using Avalonia.Rendering.Composition;
using Avalonia.Rendering.Composition.Drawing;
using Avalonia.Rendering.Composition.Server;
using Avalonia.Rendering.Composition.Transport;
using Avalonia.Utilities;

namespace Avalonia.Media;

public sealed class Pen : AvaloniaObject, IPen, ICompositionRenderResource<IPen>, ICompositionRenderResource, ICompositorSerializable
{
	public static readonly StyledProperty<IBrush?> BrushProperty = AvaloniaProperty.Register<Pen, IBrush>("Brush");

	public static readonly StyledProperty<double> ThicknessProperty = AvaloniaProperty.Register<Pen, double>("Thickness", 1.0);

	public static readonly StyledProperty<IDashStyle?> DashStyleProperty = AvaloniaProperty.Register<Pen, IDashStyle>("DashStyle");

	public static readonly StyledProperty<PenLineCap> LineCapProperty = AvaloniaProperty.Register<Pen, PenLineCap>("LineCap", PenLineCap.Flat);

	public static readonly StyledProperty<PenLineJoin> LineJoinProperty = AvaloniaProperty.Register<Pen, PenLineJoin>("LineJoin", PenLineJoin.Bevel);

	public static readonly StyledProperty<double> MiterLimitProperty = AvaloniaProperty.Register<Pen, double>("MiterLimit", 10.0);

	private DashStyle? _subscribedToDashes;

	private TargetWeakEventSubscriber<Pen, EventArgs>? _weakSubscriber;

	private static readonly WeakEvent<DashStyle, EventArgs> InvalidatedWeakEvent = WeakEvent.Register(delegate(DashStyle s, EventHandler h)
	{
		s.Invalidated += h;
	}, delegate(DashStyle s, EventHandler h)
	{
		s.Invalidated -= h;
	});

	private CompositorResourceHolder<ServerCompositionSimplePen> _resource;

	public IBrush? Brush
	{
		get
		{
			return GetValue(BrushProperty);
		}
		set
		{
			SetValue(BrushProperty, value);
		}
	}

	public double Thickness
	{
		get
		{
			return GetValue(ThicknessProperty);
		}
		set
		{
			SetValue(ThicknessProperty, value);
		}
	}

	public IDashStyle? DashStyle
	{
		get
		{
			return GetValue(DashStyleProperty);
		}
		set
		{
			SetValue(DashStyleProperty, value);
		}
	}

	public PenLineCap LineCap
	{
		get
		{
			return GetValue(LineCapProperty);
		}
		set
		{
			SetValue(LineCapProperty, value);
		}
	}

	public PenLineJoin LineJoin
	{
		get
		{
			return GetValue(LineJoinProperty);
		}
		set
		{
			SetValue(LineJoinProperty, value);
		}
	}

	public double MiterLimit
	{
		get
		{
			return GetValue(MiterLimitProperty);
		}
		set
		{
			SetValue(MiterLimitProperty, value);
		}
	}

	public Pen()
	{
	}

	public Pen(uint color, double thickness = 1.0, IDashStyle? dashStyle = null, PenLineCap lineCap = PenLineCap.Flat, PenLineJoin lineJoin = PenLineJoin.Miter, double miterLimit = 10.0)
		: this(new SolidColorBrush(color), thickness, dashStyle, lineCap, lineJoin, miterLimit)
	{
	}

	public Pen(IBrush? brush, double thickness = 1.0, IDashStyle? dashStyle = null, PenLineCap lineCap = PenLineCap.Flat, PenLineJoin lineJoin = PenLineJoin.Miter, double miterLimit = 10.0)
	{
		Brush = brush;
		Thickness = thickness;
		LineCap = lineCap;
		LineJoin = lineJoin;
		MiterLimit = miterLimit;
		DashStyle = dashStyle;
	}

	public ImmutablePen ToImmutable()
	{
		return new ImmutablePen(Brush?.ToImmutable(), Thickness, DashStyle?.ToImmutable(), LineCap, LineJoin, MiterLimit);
	}

	private void RegisterForSerialization()
	{
		_resource.RegisterForInvalidationOnAllCompositors(this);
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		RegisterForSerialization();
		if (change.Property == BrushProperty)
		{
			_resource.ProcessPropertyChangeNotification(change);
		}
		if (change.Property == DashStyleProperty)
		{
			UpdateDashStyleSubscription();
		}
		base.OnPropertyChanged(change);
	}

	private void UpdateDashStyleSubscription()
	{
		DashStyle dashStyle = (_resource.IsAttached ? (DashStyle as DashStyle) : null);
		if (_subscribedToDashes == dashStyle)
		{
			return;
		}
		if (_subscribedToDashes != null && _weakSubscriber != null)
		{
			InvalidatedWeakEvent.Unsubscribe(_subscribedToDashes, _weakSubscriber);
			_subscribedToDashes = null;
		}
		if (dashStyle == null)
		{
			return;
		}
		if (_weakSubscriber == null)
		{
			_weakSubscriber = new TargetWeakEventSubscriber<Pen, EventArgs>(this, delegate(Pen target, object? _, WeakEvent ev, EventArgs _)
			{
				if (ev == InvalidatedWeakEvent)
				{
					target.RegisterForSerialization();
				}
			});
		}
		InvalidatedWeakEvent.Subscribe(dashStyle, _weakSubscriber);
		_subscribedToDashes = dashStyle;
	}

	IPen ICompositionRenderResource<IPen>.GetForCompositor(Compositor c)
	{
		return _resource.GetForCompositor(c);
	}

	void ICompositionRenderResource.AddRefOnCompositor(Compositor c)
	{
		if (_resource.CreateOrAddRef(c, this, out ServerCompositionSimplePen _, (Compositor c) => new ServerCompositionSimplePen(c.Server)))
		{
			(Brush as ICompositionRenderResource)?.AddRefOnCompositor(c);
			UpdateDashStyleSubscription();
		}
	}

	void ICompositionRenderResource.ReleaseOnCompositor(Compositor c)
	{
		if (_resource.Release(c))
		{
			(Brush as ICompositionRenderResource)?.ReleaseOnCompositor(c);
			UpdateDashStyleSubscription();
		}
	}

	SimpleServerObject? ICompositorSerializable.TryGetServer(Compositor c)
	{
		return _resource.TryGetForCompositor(c);
	}

	void ICompositorSerializable.SerializeChanges(Compositor c, BatchStreamWriter writer)
	{
		ServerCompositionSimplePen.SerializeAllChanges(writer, Brush.GetServer(c), DashStyle?.ToImmutable(), LineCap, LineJoin, MiterLimit, Thickness);
	}
}
