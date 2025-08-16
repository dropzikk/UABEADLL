using System;
using Avalonia.Rendering.Composition.Drawing.Nodes;
using Avalonia.Rendering.Composition.Server;
using Avalonia.Rendering.Composition.Transport;
using Avalonia.Utilities;

namespace Avalonia.Rendering.Composition.Drawing;

internal class CompositionRenderData : ICompositorSerializable, IDisposable
{
	private readonly Compositor _compositor;

	private PooledInlineList<ICompositionRenderResource> _resources;

	private PooledInlineList<IRenderDataItem> _items;

	private bool _itemsSent;

	public ServerCompositionRenderData Server { get; }

	public CompositionRenderData(Compositor compositor)
	{
		_compositor = compositor;
		Server = new ServerCompositionRenderData(compositor.Server);
	}

	public void AddResource(ICompositionRenderResource resource)
	{
		_resources.Add(resource);
	}

	public void Add(IRenderDataItem item)
	{
		_items.Add(item);
	}

	public void Dispose()
	{
		if (!_itemsSent)
		{
			foreach (IRenderDataItem item in _items)
			{
				if (item is IDisposable disposable)
				{
					disposable.Dispose();
				}
			}
		}
		_items.Dispose();
		_itemsSent = false;
		foreach (ICompositionRenderResource resource in _resources)
		{
			resource.ReleaseOnCompositor(_compositor);
		}
		_resources.Dispose();
		_compositor.DisposeOnNextBatch(Server);
	}

	public SimpleServerObject TryGetServer(Compositor c)
	{
		return Server;
	}

	public void SerializeChanges(Compositor c, BatchStreamWriter writer)
	{
		writer.Write(_items.Count);
		foreach (IRenderDataItem item in _items)
		{
			writer.WriteObject(item);
		}
		_itemsSent = true;
	}

	public bool HitTest(Point pt)
	{
		foreach (IRenderDataItem item in _items)
		{
			if (item.HitTest(pt))
			{
				return true;
			}
		}
		return false;
	}
}
