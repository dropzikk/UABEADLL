using System;
using Avalonia.Animation;
using Avalonia.Media.Immutable;
using Avalonia.Rendering.Composition;
using Avalonia.Rendering.Composition.Drawing;
using Avalonia.Rendering.Composition.Server;
using Avalonia.Rendering.Composition.Transport;

namespace Avalonia.Media;

public abstract class Transform : Animatable, IMutableTransform, ITransform, ICompositionRenderResource<ITransform>, ICompositionRenderResource, ICompositorSerializable
{
	private CompositorResourceHolder<ServerCompositionSimpleTransform> _resource;

	public abstract Matrix Value { get; }

	public event EventHandler? Changed;

	internal Transform()
	{
	}

	public static Transform Parse(string s)
	{
		return new MatrixTransform(Matrix.Parse(s));
	}

	protected void RaiseChanged()
	{
		this.Changed?.Invoke(this, EventArgs.Empty);
	}

	public ImmutableTransform ToImmutable()
	{
		return new ImmutableTransform(Value);
	}

	public override string ToString()
	{
		return Value.ToString();
	}

	ITransform ICompositionRenderResource<ITransform>.GetForCompositor(Compositor c)
	{
		return _resource.GetForCompositor(c);
	}

	SimpleServerObject? ICompositorSerializable.TryGetServer(Compositor c)
	{
		return _resource.TryGetForCompositor(c);
	}

	void ICompositionRenderResource.AddRefOnCompositor(Compositor c)
	{
		_resource.CreateOrAddRef(c, this, out ServerCompositionSimpleTransform _, (Compositor cc) => new ServerCompositionSimpleTransform(cc.Server));
	}

	void ICompositionRenderResource.ReleaseOnCompositor(Compositor c)
	{
		_resource.Release(c);
	}

	void ICompositorSerializable.SerializeChanges(Compositor c, BatchStreamWriter writer)
	{
		ServerCompositionSimpleTransform.SerializeAllChanges(writer, Value);
	}
}
