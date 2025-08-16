using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Collections;
using Avalonia.Collections.Pooled;
using Avalonia.Media;
using Avalonia.Rendering.Composition.Drawing;
using Avalonia.VisualTree;

namespace Avalonia.Rendering.Composition;

internal class CompositingRenderer : IRendererWithCompositor, IRenderer, IDisposable, IHitTester
{
	private readonly IRenderRoot _root;

	private readonly Compositor _compositor;

	private readonly RenderDataDrawingContext _recorder;

	private readonly HashSet<Visual> _dirty = new HashSet<Visual>();

	private readonly HashSet<Visual> _recalculateChildren = new HashSet<Visual>();

	private readonly Action _update;

	private bool _queuedUpdate;

	private bool _updating;

	private bool _isDisposed;

	internal CompositionTarget CompositionTarget { get; }

	public RendererDiagnostics Diagnostics { get; }

	public Compositor Compositor => _compositor;

	public bool IsDisposed => _isDisposed;

	public event EventHandler<SceneInvalidatedEventArgs>? SceneInvalidated;

	public CompositingRenderer(IRenderRoot root, Compositor compositor, Func<IEnumerable<object>> surfaces)
	{
		_root = root;
		_compositor = compositor;
		_recorder = new RenderDataDrawingContext(compositor);
		CompositionTarget = compositor.CreateCompositionTarget(surfaces);
		CompositionTarget.Root = ((Visual)root).AttachToCompositor(compositor);
		_update = Update;
		Diagnostics = new RendererDiagnostics();
		Diagnostics.PropertyChanged += OnDiagnosticsPropertyChanged;
	}

	private void OnDiagnosticsPropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		string propertyName = e.PropertyName;
		if (!(propertyName == "DebugOverlays"))
		{
			if (propertyName == "LastLayoutPassTiming")
			{
				CompositionTarget.LastLayoutPassTiming = Diagnostics.LastLayoutPassTiming;
			}
		}
		else
		{
			CompositionTarget.DebugOverlays = Diagnostics.DebugOverlays;
		}
	}

	private void QueueUpdate()
	{
		if (!_queuedUpdate)
		{
			_queuedUpdate = true;
			_compositor.RequestCompositionUpdate(_update);
		}
	}

	public void AddDirty(Visual visual)
	{
		if (!_isDisposed)
		{
			if (_updating)
			{
				throw new InvalidOperationException("Visual was invalidated during the render pass");
			}
			_dirty.Add(visual);
			QueueUpdate();
		}
	}

	public IEnumerable<Visual> HitTest(Point p, Visual? root, Func<Visual, bool>? filter)
	{
		CompositionVisual root2 = null;
		if (root != null)
		{
			if (root.CompositionVisual == null)
			{
				yield break;
			}
			root2 = root.CompositionVisual;
		}
		Func<CompositionVisual, bool> filter2 = null;
		if (filter != null)
		{
			filter2 = (CompositionVisual v) => !(v is CompositionDrawListVisual compositionDrawListVisual) || filter(compositionDrawListVisual.Visual);
		}
		PooledList<CompositionVisual> pooledList = CompositionTarget.TryHitTest(p, root2, filter2);
		if (pooledList == null)
		{
			yield break;
		}
		foreach (CompositionVisual item in pooledList)
		{
			if (item is CompositionDrawListVisual compositionDrawListVisual2 && (filter == null || filter(compositionDrawListVisual2.Visual)))
			{
				yield return compositionDrawListVisual2.Visual;
			}
		}
	}

	public Visual? HitTestFirst(Point p, Visual root, Func<Visual, bool>? filter)
	{
		return HitTest(p, root, filter).FirstOrDefault();
	}

	public void RecalculateChildren(Visual visual)
	{
		if (!_isDisposed)
		{
			if (_updating)
			{
				throw new InvalidOperationException("Visual was invalidated during the render pass");
			}
			_recalculateChildren.Add(visual);
			QueueUpdate();
		}
	}

	private static void SyncChildren(Visual v)
	{
		if (v.CompositionVisual == null)
		{
			return;
		}
		CompositionVisualCollection children = v.CompositionVisual.Children;
		AvaloniaList<Visual> avaloniaList = (AvaloniaList<Visual>)v.GetVisualChildren();
		PooledList<(Visual, int)> pooledList = null;
		if (v.HasNonUniformZIndexChildren && avaloniaList.Count > 1)
		{
			pooledList = new PooledList<(Visual, int)>(avaloniaList.Count);
			for (int i = 0; i < avaloniaList.Count; i++)
			{
				pooledList.Add((avaloniaList[i], i));
			}
			pooledList.Sort(delegate((Visual visual, int index) lhs, (Visual visual, int index) rhs)
			{
				int num = lhs.visual.ZIndex.CompareTo(rhs.visual.ZIndex);
				return (num != 0) ? num : lhs.index.CompareTo(rhs.index);
			});
		}
		CompositionVisual compositionVisual = v.ChildCompositionVisual;
		if (compositionVisual != null && compositionVisual.Compositor != v.CompositionVisual.Compositor)
		{
			compositionVisual = null;
		}
		int num2 = avaloniaList.Count;
		if (compositionVisual != null)
		{
			num2++;
		}
		if (children.Count == num2)
		{
			bool flag = false;
			if (pooledList != null)
			{
				for (int j = 0; j < avaloniaList.Count; j++)
				{
					if (children[j] != pooledList[j].Item1.CompositionVisual)
					{
						flag = true;
						break;
					}
				}
			}
			else
			{
				for (int k = 0; k < avaloniaList.Count; k++)
				{
					if (children[k] != avaloniaList[k].CompositionVisual)
					{
						flag = true;
						break;
					}
				}
			}
			if (compositionVisual != null && children[children.Count - 1] != compositionVisual)
			{
				flag = true;
			}
			if (!flag)
			{
				pooledList?.Dispose();
				return;
			}
		}
		children.Clear();
		if (pooledList != null)
		{
			foreach (var item in pooledList)
			{
				CompositionDrawListVisual compositionVisual2 = item.Item1.CompositionVisual;
				if (compositionVisual2 != null)
				{
					children.Add(compositionVisual2);
				}
			}
			pooledList.Dispose();
		}
		else
		{
			foreach (Visual item2 in avaloniaList)
			{
				CompositionDrawListVisual compositionVisual3 = item2.CompositionVisual;
				if (compositionVisual3 != null)
				{
					children.Add(compositionVisual3);
				}
			}
		}
		if (compositionVisual != null)
		{
			children.Add(compositionVisual);
		}
	}

	private void UpdateCore()
	{
		_queuedUpdate = false;
		foreach (Visual item in _dirty)
		{
			CompositionDrawListVisual compositionVisual = item.CompositionVisual;
			if (compositionVisual != null)
			{
				compositionVisual.Offset = new Vector3D(item.Bounds.Left, item.Bounds.Top, 0.0);
				compositionVisual.Size = new Vector(item.Bounds.Width, item.Bounds.Height);
				compositionVisual.Visible = item.IsVisible;
				compositionVisual.Opacity = (float)item.Opacity;
				compositionVisual.ClipToBounds = item.ClipToBounds;
				compositionVisual.Clip = item.Clip?.PlatformImpl;
				if (!object.Equals(compositionVisual.OpacityMask, item.OpacityMask))
				{
					compositionVisual.OpacityMask = item.OpacityMask?.ToImmutable();
				}
				if (!compositionVisual.Effect.EffectEquals(item.Effect))
				{
					compositionVisual.Effect = item.Effect?.ToImmutable();
				}
				compositionVisual.RenderOptions = item.RenderOptions;
				Matrix transformMatrix = Matrix.Identity;
				if (item.HasMirrorTransform)
				{
					transformMatrix = new Matrix(-1.0, 0.0, 0.0, 1.0, item.Bounds.Width, 0.0);
				}
				if (item.RenderTransform != null)
				{
					Matrix matrix = Matrix.CreateTranslation(item.RenderTransformOrigin.ToPixels(new Size(item.Bounds.Width, item.Bounds.Height)));
					transformMatrix *= -matrix * item.RenderTransform.Value * matrix;
				}
				compositionVisual.TransformMatrix = transformMatrix;
				try
				{
					item.Render(_recorder);
					compositionVisual.DrawList = _recorder.GetRenderResults();
				}
				finally
				{
					_recorder.Reset();
				}
				SyncChildren(item);
			}
		}
		foreach (Visual recalculateChild in _recalculateChildren)
		{
			if (!_dirty.Contains(recalculateChild))
			{
				SyncChildren(recalculateChild);
			}
		}
		_dirty.Clear();
		_recalculateChildren.Clear();
		CompositionTarget.Size = _root.ClientSize;
		CompositionTarget.Scaling = _root.RenderScaling;
		TriggerSceneInvalidatedOnBatchCompletion(_compositor.RequestCommitAsync());
	}

	private async void TriggerSceneInvalidatedOnBatchCompletion(Task batchCompletion)
	{
		await batchCompletion;
		this.SceneInvalidated?.Invoke(this, new SceneInvalidatedEventArgs(_root, new Rect(_root.ClientSize)));
	}

	public void TriggerSceneInvalidatedForUnitTests(Rect rect)
	{
		this.SceneInvalidated?.Invoke(this, new SceneInvalidatedEventArgs(_root, rect));
	}

	private void Update()
	{
		if (_updating)
		{
			return;
		}
		_updating = true;
		try
		{
			UpdateCore();
		}
		finally
		{
			_updating = false;
		}
	}

	public void Resized(Size size)
	{
	}

	public void Paint(Rect rect)
	{
		if (!_isDisposed)
		{
			QueueUpdate();
			CompositionTarget.RequestRedraw();
			MediaContext.Instance.ImmediateRenderRequested(CompositionTarget);
		}
	}

	public void Start()
	{
		if (!_isDisposed)
		{
			CompositionTarget.IsEnabled = true;
		}
	}

	public void Stop()
	{
		CompositionTarget.IsEnabled = false;
	}

	public ValueTask<object?> TryGetRenderInterfaceFeature(Type featureType)
	{
		return Compositor.TryGetRenderInterfaceFeature(featureType);
	}

	public void Dispose()
	{
		if (!_isDisposed)
		{
			_isDisposed = true;
			_dirty.Clear();
			_recalculateChildren.Clear();
			this.SceneInvalidated = null;
			Stop();
			MediaContext.Instance.SyncDisposeCompositionTarget(CompositionTarget);
		}
	}
}
