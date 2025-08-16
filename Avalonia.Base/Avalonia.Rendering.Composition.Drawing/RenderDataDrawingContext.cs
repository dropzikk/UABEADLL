using System;
using System.Collections.Generic;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using Avalonia.Platform;
using Avalonia.Rendering.Composition.Drawing.Nodes;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Threading;
using Avalonia.Utilities;

namespace Avalonia.Rendering.Composition.Drawing;

internal class RenderDataDrawingContext : DrawingContext
{
	private struct ParentStackItem
	{
		public RenderDataPushNode? Node;

		public List<IRenderDataItem> Items;
	}

	private readonly Compositor? _compositor;

	private CompositionRenderData? _renderData;

	private HashSet<object>? _resourcesHashSet;

	private static readonly ThreadSafeObjectPool<HashSet<object>> s_hashSetPool = new ThreadSafeObjectPool<HashSet<object>>();

	private List<IRenderDataItem>? _currentItemList;

	private static readonly ThreadSafeObjectPool<List<IRenderDataItem>> s_listPool = new ThreadSafeObjectPool<List<IRenderDataItem>>();

	private Stack<ParentStackItem>? _parentNodeStack;

	private static readonly ThreadSafeObjectPool<Stack<ParentStackItem>> s_parentStackPool = new ThreadSafeObjectPool<Stack<ParentStackItem>>();

	private CompositionRenderData RenderData => _renderData ?? (_renderData = new CompositionRenderData(_compositor));

	public RenderDataDrawingContext(Compositor? compositor)
	{
		_compositor = compositor;
	}

	private void Add(IRenderDataItem item)
	{
		if (_currentItemList == null)
		{
			_currentItemList = s_listPool.Get();
		}
		_currentItemList.Add(item);
	}

	private void Push(RenderDataPushNode? node = null)
	{
		if (node == null)
		{
			(_parentNodeStack ?? (_parentNodeStack = s_parentStackPool.Get())).Push(default(ParentStackItem));
			return;
		}
		Add(node);
		(_parentNodeStack ?? (_parentNodeStack = s_parentStackPool.Get())).Push(new ParentStackItem
		{
			Node = node,
			Items = _currentItemList
		});
		_currentItemList = null;
	}

	private void Pop<T>() where T : IRenderDataItem
	{
		ParentStackItem parentStackItem = _parentNodeStack.Pop();
		if (parentStackItem.Node == null)
		{
			return;
		}
		if (!(parentStackItem.Node is T))
		{
			throw new InvalidOperationException("Invalid Pop operation");
		}
		bool flag = true;
		if (_currentItemList != null)
		{
			flag = _currentItemList.Count == 0;
			foreach (IRenderDataItem currentItem in _currentItemList)
			{
				parentStackItem.Node.Children.Add(currentItem);
			}
			_currentItemList.Clear();
			s_listPool.ReturnAndSetNull(ref _currentItemList);
		}
		_currentItemList = parentStackItem.Items;
		if (flag)
		{
			_currentItemList.RemoveAt(_currentItemList.Count - 1);
		}
	}

	private void AddResource(object? resource)
	{
		if (_compositor != null && resource != null && !(resource is IImmutableBrush) && !(resource is ImmutablePen) && !(resource is ImmutableTransform))
		{
			if (!(resource is ICompositionRenderResource compositionRenderResource))
			{
				throw new InvalidOperationException(resource.GetType().FullName + " can not be used with this DrawingContext");
			}
			if (_resourcesHashSet == null)
			{
				_resourcesHashSet = s_hashSetPool.Get();
			}
			if (_resourcesHashSet.Add(compositionRenderResource))
			{
				compositionRenderResource.AddRefOnCompositor(_compositor);
				RenderData.AddResource(compositionRenderResource);
			}
		}
	}

	protected override void DrawLineCore(IPen? pen, Point p1, Point p2)
	{
		if (pen != null)
		{
			AddResource(pen);
			Add(new RenderDataLineNode
			{
				ClientPen = pen,
				ServerPen = pen.GetServer(_compositor),
				P1 = p1,
				P2 = p2
			});
		}
	}

	protected override void DrawGeometryCore(IBrush? brush, IPen? pen, IGeometryImpl geometry)
	{
		if (brush != null || pen != null)
		{
			AddResource(brush);
			AddResource(pen);
			Add(new RenderDataGeometryNode
			{
				ServerBrush = brush.GetServer(_compositor),
				ServerPen = pen.GetServer(_compositor),
				ClientPen = pen,
				Geometry = geometry
			});
		}
	}

	protected override void DrawRectangleCore(IBrush? brush, IPen? pen, RoundedRect rrect, BoxShadows boxShadows = default(BoxShadows))
	{
		if (!rrect.IsEmpty() && (brush != null || pen != null || !(boxShadows == default(BoxShadows))))
		{
			AddResource(brush);
			AddResource(pen);
			Add(new RenderDataRectangleNode
			{
				ServerBrush = brush.GetServer(_compositor),
				ServerPen = pen.GetServer(_compositor),
				ClientPen = pen,
				Rect = rrect,
				BoxShadows = boxShadows
			});
		}
	}

	protected override void DrawEllipseCore(IBrush? brush, IPen? pen, Rect rect)
	{
		if (!rect.IsEmpty() && (brush != null || pen != null))
		{
			AddResource(brush);
			AddResource(pen);
			Add(new RenderDataEllipseNode
			{
				ServerBrush = brush.GetServer(_compositor),
				ServerPen = pen.GetServer(_compositor),
				ClientPen = pen,
				Rect = rect
			});
		}
	}

	public override void Custom(ICustomDrawOperation custom)
	{
		Add(new RenderDataCustomNode
		{
			Operation = custom
		});
	}

	public override void DrawGlyphRun(IBrush? foreground, GlyphRun? glyphRun)
	{
		if (foreground != null && glyphRun != null)
		{
			AddResource(foreground);
			Add(new RenderDataGlyphRunNode
			{
				ServerBrush = foreground.GetServer(_compositor),
				GlyphRun = glyphRun.PlatformImpl.Clone()
			});
		}
	}

	protected override void PushClipCore(RoundedRect rect)
	{
		Push(new RenderDataClipNode
		{
			Rect = rect
		});
	}

	protected override void PushClipCore(Rect rect)
	{
		Push(new RenderDataClipNode
		{
			Rect = rect
		});
	}

	protected override void PushGeometryClipCore(Geometry? clip)
	{
		if (clip == null)
		{
			Push();
			return;
		}
		Push(new RenderDataGeometryClipNode
		{
			Geometry = clip?.PlatformImpl
		});
	}

	protected override void PushOpacityCore(double opacity)
	{
		if (opacity == 1.0)
		{
			Push();
			return;
		}
		Push(new RenderDataOpacityNode
		{
			Opacity = opacity
		});
	}

	protected override void PushOpacityMaskCore(IBrush? mask, Rect bounds)
	{
		if (mask == null)
		{
			Push();
			return;
		}
		AddResource(mask);
		Push(new RenderDataOpacityMaskNode
		{
			ServerBrush = mask.GetServer(_compositor),
			BoundsRect = bounds
		});
	}

	protected override void PushTransformCore(Matrix matrix)
	{
		if (matrix.IsIdentity)
		{
			Push();
			return;
		}
		Push(new RenderDataPushMatrixNode
		{
			Matrix = matrix
		});
	}

	protected override void PopClipCore()
	{
		Pop<RenderDataClipNode>();
	}

	protected override void PopGeometryClipCore()
	{
		Pop<RenderDataGeometryClipNode>();
	}

	protected override void PopOpacityCore()
	{
		Pop<RenderDataOpacityNode>();
	}

	protected override void PopOpacityMaskCore()
	{
		Pop<RenderDataOpacityMaskNode>();
	}

	protected override void PopTransformCore()
	{
		Pop<RenderDataPushMatrixNode>();
	}

	internal override void DrawBitmap(IRef<IBitmapImpl>? source, double opacity, Rect sourceRect, Rect destRect)
	{
		if (source != null && !sourceRect.IsEmpty() && !destRect.IsEmpty())
		{
			Add(new RenderDataBitmapNode
			{
				Bitmap = source.Clone(),
				Opacity = opacity,
				SourceRect = sourceRect,
				DestRect = destRect
			});
		}
	}

	private void FlushStack()
	{
		if (_parentNodeStack != null)
		{
			while (_parentNodeStack.Count > 0)
			{
				Pop<IRenderDataItem>();
			}
		}
	}

	public CompositionRenderData? GetRenderResults()
	{
		FlushStack();
		List<IRenderDataItem> currentItemList = _currentItemList;
		if (currentItemList != null && currentItemList.Count > 0)
		{
			foreach (IRenderDataItem currentItem in _currentItemList)
			{
				RenderData.Add(currentItem);
			}
			_currentItemList.Clear();
		}
		CompositionRenderData renderData = _renderData;
		_renderData = null;
		_resourcesHashSet?.Clear();
		if (renderData != null)
		{
			_compositor.RegisterForSerialization(renderData);
		}
		return renderData;
	}

	public ImmediateRenderDataSceneBrushContent? GetImmediateSceneBrushContent(ITileBrush brush, Rect? rect, bool useScalableRasterization)
	{
		FlushStack();
		if (_currentItemList == null || _currentItemList.Count == 0)
		{
			return null;
		}
		List<IRenderDataItem> currentItemList = _currentItemList;
		_currentItemList = null;
		return new ImmediateRenderDataSceneBrushContent(brush, currentItemList, rect, useScalableRasterization, s_listPool);
	}

	public void Reset()
	{
		if (_renderData != null)
		{
			_renderData.Dispose();
			_renderData = null;
		}
		_currentItemList?.Clear();
		_parentNodeStack?.Clear();
		_resourcesHashSet?.Clear();
	}

	protected override void DisposeCore()
	{
		Reset();
		if (_resourcesHashSet != null)
		{
			s_hashSetPool.ReturnAndSetNull(ref _resourcesHashSet);
		}
	}
}
