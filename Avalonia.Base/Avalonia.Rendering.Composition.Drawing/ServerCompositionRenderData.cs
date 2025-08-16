using System;
using System.Collections.Generic;
using Avalonia.Platform;
using Avalonia.Rendering.Composition.Drawing.Nodes;
using Avalonia.Rendering.Composition.Server;
using Avalonia.Rendering.Composition.Transport;
using Avalonia.Threading;
using Avalonia.Utilities;

namespace Avalonia.Rendering.Composition.Drawing;

internal class ServerCompositionRenderData : SimpleServerRenderResource
{
	private class Collector : IRenderDataServerResourcesCollector
	{
		public readonly HashSet<IServerRenderResource> Resources = new HashSet<IServerRenderResource>();

		public void AddRenderDataServerResource(object? obj)
		{
			if (obj is IServerRenderResource item)
			{
				Resources.Add(item);
			}
		}
	}

	private PooledInlineList<IRenderDataItem> _items;

	private PooledInlineList<IServerRenderResource> _referencedResources;

	private Rect? _bounds;

	private bool _boundsValid;

	private static readonly ThreadSafeObjectPool<Collector> s_resourceHashSetPool = new ThreadSafeObjectPool<Collector>();

	public Rect? Bounds
	{
		get
		{
			if (!_boundsValid)
			{
				_bounds = CalculateRenderBounds();
				_boundsValid = true;
			}
			return _bounds;
		}
	}

	public ServerCompositionRenderData(ServerCompositor compositor)
		: base(compositor)
	{
	}

	protected override void DeserializeChangesCore(BatchStreamReader reader, TimeSpan committedAt)
	{
		Reset();
		int num = reader.Read<int>();
		_items.EnsureCapacity(num);
		for (int i = 0; i < num; i++)
		{
			_items.Add(reader.ReadObject<IRenderDataItem>());
		}
		Collector obj = s_resourceHashSetPool.Get();
		CollectResources(_items, obj);
		foreach (IServerRenderResource resource in obj.Resources)
		{
			_referencedResources.Add(resource);
			resource.AddObserver(this);
		}
		obj.Resources.Clear();
		s_resourceHashSetPool.ReturnAndSetNull(ref obj);
		base.DeserializeChangesCore(reader, committedAt);
	}

	private static void CollectResources(PooledInlineList<IRenderDataItem> items, IRenderDataServerResourcesCollector collector)
	{
		foreach (IRenderDataItem item in items)
		{
			if (item is IRenderDataItemWithServerResources renderDataItemWithServerResources)
			{
				renderDataItemWithServerResources.Collect(collector);
			}
			else if (item is RenderDataPushNode renderDataPushNode)
			{
				CollectResources(renderDataPushNode.Children, collector);
			}
		}
	}

	private Rect? CalculateRenderBounds()
	{
		Rect? rect = null;
		foreach (IRenderDataItem item in _items)
		{
			rect = Rect.Union(rect, item.Bounds);
		}
		return ApplyRenderBoundsRounding(rect);
	}

	public static Rect? ApplyRenderBoundsRounding(Rect? rect)
	{
		if (rect.HasValue)
		{
			Rect value = rect.Value;
			return new Rect(new Point(Math.Floor(value.X), Math.Floor(value.Y)), new Point(Math.Ceiling(value.Right), Math.Ceiling(value.Bottom)));
		}
		return null;
	}

	public override void DependencyQueuedInvalidate(IServerRenderResource sender)
	{
		_boundsValid = false;
		base.DependencyQueuedInvalidate(sender);
	}

	public void Render(IDrawingContextImpl context)
	{
		RenderDataNodeRenderContext context2 = new RenderDataNodeRenderContext(context);
		try
		{
			foreach (IRenderDataItem item in _items)
			{
				item.Invoke(ref context2);
			}
		}
		finally
		{
			context2.Dispose();
		}
	}

	private void Reset()
	{
		_bounds = null;
		_boundsValid = false;
		foreach (IServerRenderResource referencedResource in _referencedResources)
		{
			referencedResource.RemoveObserver(this);
		}
		_referencedResources.Dispose();
		foreach (IRenderDataItem item in _items)
		{
			if (item is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}
		_items.Dispose();
	}

	public override void Dispose()
	{
		Reset();
		base.Dispose();
	}
}
