using System;
using Avalonia.Utilities;

namespace Avalonia.Rendering.Composition.Drawing.Nodes;

internal abstract class RenderDataPushNode : IRenderDataItem, IDisposable
{
	public PooledInlineList<IRenderDataItem> Children;

	public virtual Rect? Bounds
	{
		get
		{
			if (Children.Count == 0)
			{
				return null;
			}
			Rect? rect = null;
			foreach (IRenderDataItem child in Children)
			{
				rect = Rect.Union(rect, child.Bounds);
			}
			return rect;
		}
	}

	public abstract void Push(ref RenderDataNodeRenderContext context);

	public abstract void Pop(ref RenderDataNodeRenderContext context);

	public void Invoke(ref RenderDataNodeRenderContext context)
	{
		if (Children.Count == 0)
		{
			return;
		}
		Push(ref context);
		foreach (IRenderDataItem child in Children)
		{
			child.Invoke(ref context);
		}
		Pop(ref context);
	}

	public virtual bool HitTest(Point p)
	{
		if (Children.Count == 0)
		{
			return false;
		}
		foreach (IRenderDataItem child in Children)
		{
			if (child.HitTest(p))
			{
				return true;
			}
		}
		return false;
	}

	public void Dispose()
	{
		if (Children.Count <= 0)
		{
			return;
		}
		foreach (IRenderDataItem child in Children)
		{
			if (child is RenderDataPushNode renderDataPushNode)
			{
				renderDataPushNode.Dispose();
			}
		}
		Children.Dispose();
	}
}
