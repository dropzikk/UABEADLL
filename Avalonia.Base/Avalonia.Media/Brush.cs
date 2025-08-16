using System;
using System.ComponentModel;
using Avalonia.Animation;
using Avalonia.Media.Immutable;
using Avalonia.Rendering.Composition;
using Avalonia.Rendering.Composition.Drawing;
using Avalonia.Rendering.Composition.Server;
using Avalonia.Rendering.Composition.Transport;

namespace Avalonia.Media;

[TypeConverter(typeof(BrushConverter))]
public abstract class Brush : Animatable, IBrush, ICompositionRenderResource<IBrush>, ICompositionRenderResource, ICompositorSerializable
{
	public static readonly StyledProperty<double> OpacityProperty = AvaloniaProperty.Register<Brush, double>("Opacity", 1.0);

	public static readonly StyledProperty<ITransform?> TransformProperty = AvaloniaProperty.Register<Brush, ITransform>("Transform");

	public static readonly StyledProperty<RelativePoint> TransformOriginProperty = AvaloniaProperty.Register<Brush, RelativePoint>("TransformOrigin");

	private CompositorResourceHolder<ServerCompositionSimpleBrush> _resource;

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

	public ITransform? Transform
	{
		get
		{
			return GetValue(TransformProperty);
		}
		set
		{
			SetValue(TransformProperty, value);
		}
	}

	public RelativePoint TransformOrigin
	{
		get
		{
			return GetValue(TransformOriginProperty);
		}
		set
		{
			SetValue(TransformOriginProperty, value);
		}
	}

	internal abstract Func<Compositor, ServerCompositionSimpleBrush> Factory { get; }

	public static IBrush Parse(string s)
	{
		if (s == null)
		{
			throw new ArgumentNullException("s");
		}
		if (s.Length > 0)
		{
			if (s[0] == '#')
			{
				return new ImmutableSolidColorBrush(Color.Parse(s));
			}
			ISolidColorBrush knownBrush = KnownColors.GetKnownBrush(s);
			if (knownBrush != null)
			{
				return knownBrush;
			}
		}
		throw new FormatException("Invalid brush string: '" + s + "'.");
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		if (change.Property == TransformProperty)
		{
			_resource.ProcessPropertyChangeNotification(change);
		}
		RegisterForSerialization();
		base.OnPropertyChanged(change);
	}

	private protected void RegisterForSerialization()
	{
		_resource.RegisterForInvalidationOnAllCompositors(this);
	}

	IBrush ICompositionRenderResource<IBrush>.GetForCompositor(Compositor c)
	{
		return _resource.GetForCompositor(c);
	}

	void ICompositionRenderResource.AddRefOnCompositor(Compositor c)
	{
		if (_resource.CreateOrAddRef(c, this, out ServerCompositionSimpleBrush _, Factory))
		{
			OnReferencedFromCompositor(c);
		}
	}

	private protected virtual void OnReferencedFromCompositor(Compositor c)
	{
		if (Transform is ICompositionRenderResource<ITransform> compositionRenderResource)
		{
			compositionRenderResource.AddRefOnCompositor(c);
		}
	}

	void ICompositionRenderResource.ReleaseOnCompositor(Compositor c)
	{
		if (_resource.Release(c))
		{
			OnUnreferencedFromCompositor(c);
		}
	}

	protected virtual void OnUnreferencedFromCompositor(Compositor c)
	{
		if (Transform is ICompositionRenderResource<ITransform> compositionRenderResource)
		{
			compositionRenderResource.ReleaseOnCompositor(c);
		}
	}

	SimpleServerObject? ICompositorSerializable.TryGetServer(Compositor c)
	{
		return _resource.TryGetForCompositor(c);
	}

	private protected virtual void SerializeChanges(Compositor c, BatchStreamWriter writer)
	{
		ServerCompositionSimpleBrush.SerializeAllChanges(writer, Opacity, TransformOrigin, Transform.GetServer(c));
	}

	void ICompositorSerializable.SerializeChanges(Compositor c, BatchStreamWriter writer)
	{
		SerializeChanges(c, writer);
	}
}
