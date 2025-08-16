using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using Avalonia.Platform;
using Avalonia.Rendering.Composition.Expressions;
using Avalonia.Rendering.Composition.Transport;
using Avalonia.Utilities;

namespace Avalonia.Rendering.Composition.Server;

internal class ServerCompositionTarget : ServerObject, IDisposable
{
	private readonly ServerCompositor _compositor;

	private readonly Func<IEnumerable<object>> _surfaces;

	private readonly DiagnosticTextRenderer? _diagnosticTextRenderer;

	private static long s_nextId = 1L;

	private IRenderTarget? _renderTarget;

	private FpsCounter? _fpsCounter;

	private FrameTimeGraph? _renderTimeGraph;

	private FrameTimeGraph? _layoutTimeGraph;

	private Rect _dirtyRect;

	private Random _random = new Random();

	private Size _layerSize;

	private IDrawingContextLayerImpl? _layer;

	private bool _redrawRequested;

	private bool _disposed;

	private HashSet<ServerCompositionVisual> _attachedVisuals = new HashSet<ServerCompositionVisual>();

	private Queue<ServerCompositionVisual> _adornerUpdateQueue = new Queue<ServerCompositionVisual>();

	private ServerCompositionVisual? _root;

	internal static CompositionProperty s_IdOfRootProperty = CompositionProperty.Register();

	private bool _isEnabled;

	internal static CompositionProperty s_IdOfIsEnabledProperty = CompositionProperty.Register();

	private RendererDebugOverlays _debugOverlays;

	internal static CompositionProperty s_IdOfDebugOverlaysProperty = CompositionProperty.Register();

	private LayoutPassTiming _lastLayoutPassTiming;

	internal static CompositionProperty s_IdOfLastLayoutPassTimingProperty = CompositionProperty.Register();

	private double _scaling;

	internal static CompositionProperty s_IdOfScalingProperty = CompositionProperty.Register();

	private Size _size;

	internal static CompositionProperty s_IdOfSizeProperty = CompositionProperty.Register();

	public long Id { get; }

	public ulong Revision { get; private set; }

	public ICompositionTargetDebugEvents? DebugEvents { get; set; }

	public ReadbackIndices Readback { get; } = new ReadbackIndices();

	public int RenderedVisuals { get; set; }

	private FpsCounter? FpsCounter => _fpsCounter ?? (_fpsCounter = ((_diagnosticTextRenderer != null) ? new FpsCounter(_diagnosticTextRenderer) : null));

	private FrameTimeGraph? LayoutTimeGraph => _layoutTimeGraph ?? (_layoutTimeGraph = CreateTimeGraph("Layout"));

	private FrameTimeGraph? RenderTimeGraph => _renderTimeGraph ?? (_renderTimeGraph = CreateTimeGraph("Render"));

	public ServerCompositionVisual? Root
	{
		get
		{
			return GetValue(s_IdOfRootProperty, ref _root);
		}
		set
		{
			bool flag = false;
			if (_root != value)
			{
				flag = true;
			}
			SetValue(s_IdOfRootProperty, ref _root, value);
		}
	}

	public bool IsEnabled
	{
		get
		{
			return GetValue(s_IdOfIsEnabledProperty, ref _isEnabled);
		}
		set
		{
			bool flag = false;
			if (_isEnabled != value)
			{
				flag = true;
			}
			SetValue(s_IdOfIsEnabledProperty, ref _isEnabled, value);
			if (flag)
			{
				OnIsEnabledChanged();
			}
		}
	}

	public RendererDebugOverlays DebugOverlays
	{
		get
		{
			return GetValue(s_IdOfDebugOverlaysProperty, ref _debugOverlays);
		}
		set
		{
			bool flag = false;
			if (_debugOverlays != value)
			{
				flag = true;
			}
			SetValue(s_IdOfDebugOverlaysProperty, ref _debugOverlays, value);
			if (flag)
			{
				OnDebugOverlaysChanged();
			}
		}
	}

	public LayoutPassTiming LastLayoutPassTiming
	{
		get
		{
			return GetValue(s_IdOfLastLayoutPassTimingProperty, ref _lastLayoutPassTiming);
		}
		set
		{
			bool flag = false;
			if (_lastLayoutPassTiming != value)
			{
				flag = true;
			}
			SetValue(s_IdOfLastLayoutPassTimingProperty, ref _lastLayoutPassTiming, value);
			if (flag)
			{
				OnLastLayoutPassTimingChanged();
			}
		}
	}

	public double Scaling
	{
		get
		{
			return GetValue(s_IdOfScalingProperty, ref _scaling);
		}
		set
		{
			bool flag = false;
			if (_scaling != value)
			{
				flag = true;
			}
			SetValue(s_IdOfScalingProperty, ref _scaling, value);
		}
	}

	public Size Size
	{
		get
		{
			return GetValue(s_IdOfSizeProperty, ref _size);
		}
		set
		{
			bool flag = false;
			if (_size != value)
			{
				flag = true;
			}
			SetValue(s_IdOfSizeProperty, ref _size, value);
		}
	}

	public ServerCompositionTarget(ServerCompositor compositor, Func<IEnumerable<object>> surfaces, DiagnosticTextRenderer? diagnosticTextRenderer)
		: base(compositor)
	{
		_compositor = compositor;
		_surfaces = surfaces;
		_diagnosticTextRenderer = diagnosticTextRenderer;
		Id = Interlocked.Increment(ref s_nextId);
	}

	private FrameTimeGraph? CreateTimeGraph(string title)
	{
		if (_diagnosticTextRenderer == null)
		{
			return null;
		}
		return new FrameTimeGraph(360, new Size(360.0, 64.0), 16.666666666666668, title, _diagnosticTextRenderer);
	}

	public void Render()
	{
		if (_disposed)
		{
			base.Compositor.RemoveCompositionTarget(this);
		}
		else
		{
			if (Root == null)
			{
				return;
			}
			IRenderTarget? renderTarget = _renderTarget;
			if (renderTarget != null && renderTarget.IsCorrupted)
			{
				_layer?.Dispose();
				_layer = null;
				_renderTarget.Dispose();
				_renderTarget = null;
				_redrawRequested = true;
			}
			try
			{
				if (_renderTarget == null)
				{
					_renderTarget = _compositor.CreateRenderTarget(_surfaces());
				}
			}
			catch (RenderTargetNotReadyException)
			{
				return;
			}
			catch (RenderTargetCorruptedException)
			{
				return;
			}
			if (_dirtyRect.Width == 0.0 && _dirtyRect.Height == 0.0 && !_redrawRequested)
			{
				return;
			}
			Revision++;
			bool flag = (DebugOverlays & RendererDebugOverlays.RenderTimeGraph) != 0;
			long startingTimestamp = (flag ? Stopwatch.GetTimestamp() : 0);
			Root.Update(this);
			while (_adornerUpdateQueue.Count > 0)
			{
				_adornerUpdateQueue.Dequeue().Update(this);
			}
			Readback.CompleteWrite(Revision);
			_redrawRequested = false;
			using IDrawingContextImpl drawingContextImpl = _renderTarget.CreateDrawingContext();
			Size size = Size;
			Size size2 = size * Scaling;
			if (size2 != _layerSize || _layer == null || _layer.IsCorrupted)
			{
				_layer?.Dispose();
				_layer = null;
				_layer = drawingContextImpl.CreateLayer(size);
				_layerSize = size2;
				_dirtyRect = new Rect(0.0, 0.0, size.Width, size.Height);
			}
			if (_dirtyRect.Width != 0.0 || _dirtyRect.Height != 0.0)
			{
				using IDrawingContextImpl drawingContextImpl2 = _layer.CreateDrawingContext();
				drawingContextImpl2.PushClip(_dirtyRect);
				drawingContextImpl2.Clear(Colors.Transparent);
				Root.Render(new CompositorDrawingContextProxy(drawingContextImpl2), _dirtyRect);
				drawingContextImpl2.PopClip();
			}
			drawingContextImpl.Clear(Colors.Transparent);
			drawingContextImpl.Transform = Matrix.Identity;
			if (_layer.CanBlit)
			{
				_layer.Blit(drawingContextImpl);
			}
			else
			{
				drawingContextImpl.DrawBitmap(_layer, 1.0, new Rect(_layerSize), new Rect(size));
			}
			if (DebugOverlays != 0)
			{
				if (flag)
				{
					TimeSpan elapsedTime = StopwatchHelper.GetElapsedTime(startingTimestamp);
					RenderTimeGraph?.AddFrameValue(elapsedTime.TotalMilliseconds);
				}
				DrawOverlays(drawingContextImpl);
			}
			RenderedVisuals = 0;
			_dirtyRect = default(Rect);
		}
	}

	private void DrawOverlays(IDrawingContextImpl targetContext)
	{
		if ((DebugOverlays & RendererDebugOverlays.DirtyRects) != 0)
		{
			targetContext.DrawRectangle(new ImmutableSolidColorBrush(new Color(30, (byte)_random.Next(255), (byte)_random.Next(255), (byte)_random.Next(255))), null, _dirtyRect);
		}
		if ((DebugOverlays & RendererDebugOverlays.Fps) != 0)
		{
			string text = ByteSizeHelper.ToString((ulong)((base.Compositor.BatchMemoryPool.CurrentUsage + base.Compositor.BatchMemoryPool.CurrentPool) * base.Compositor.BatchMemoryPool.BufferSize), separate: false);
			string text2 = ByteSizeHelper.ToString((ulong)((base.Compositor.BatchObjectPool.CurrentUsage + base.Compositor.BatchObjectPool.CurrentPool) * base.Compositor.BatchObjectPool.ArraySize * IntPtr.Size), separate: false);
			FpsCounter?.RenderFps(targetContext, FormattableString.Invariant($"M:{text2} / N:{text} R:{RenderedVisuals:0000}"));
		}
		double top = 0.0;
		if ((DebugOverlays & RendererDebugOverlays.LayoutTimeGraph) != 0)
		{
			DrawTimeGraph(LayoutTimeGraph);
		}
		if ((DebugOverlays & RendererDebugOverlays.RenderTimeGraph) != 0)
		{
			DrawTimeGraph(RenderTimeGraph);
		}
		void DrawTimeGraph(FrameTimeGraph? graph)
		{
			if (graph != null)
			{
				top += 8.0;
				targetContext.Transform = Matrix.CreateTranslation(Size.Width - graph.Size.Width - 8.0, top);
				graph.Render(targetContext);
				top += graph.Size.Height;
			}
		}
	}

	public Rect SnapToDevicePixels(Rect rect)
	{
		return SnapToDevicePixels(rect, Scaling);
	}

	private static Rect SnapToDevicePixels(Rect rect, double scale)
	{
		return new Rect(new Point(Math.Floor(rect.X * scale) / scale, Math.Floor(rect.Y * scale) / scale), new Point(Math.Ceiling(rect.Right * scale) / scale, Math.Ceiling(rect.Bottom * scale) / scale));
	}

	public void AddDirtyRect(Rect rect)
	{
		if (rect.Width != 0.0 || rect.Height != 0.0)
		{
			Rect rect2 = SnapToDevicePixels(rect, Scaling);
			DebugEvents?.RectInvalidated(rect);
			_dirtyRect = _dirtyRect.Union(rect2);
			_redrawRequested = true;
		}
	}

	public void Invalidate()
	{
		_redrawRequested = true;
	}

	public void Dispose()
	{
		if (_disposed)
		{
			return;
		}
		_disposed = true;
		using (_compositor.RenderInterface.EnsureCurrent())
		{
			if (_layer != null)
			{
				_layer.Dispose();
				_layer = null;
			}
			_renderTarget?.Dispose();
			_renderTarget = null;
		}
		_compositor.RemoveCompositionTarget(this);
	}

	public void AddVisual(ServerCompositionVisual visual)
	{
		if (_attachedVisuals.Add(visual) && IsEnabled)
		{
			visual.Activate();
		}
	}

	public void RemoveVisual(ServerCompositionVisual visual)
	{
		if (_attachedVisuals.Remove(visual) && IsEnabled)
		{
			visual.Deactivate();
		}
		if (visual.IsVisibleInFrame)
		{
			AddDirtyRect(visual.TransformedOwnContentBounds);
		}
	}

	public void EnqueueAdornerUpdate(ServerCompositionVisual visual)
	{
		_adornerUpdateQueue.Enqueue(visual);
	}

	private void DeserializeChangesExtra(BatchStreamReader c)
	{
		_redrawRequested = true;
	}

	private void OnIsEnabledChanged()
	{
		if (IsEnabled)
		{
			_compositor.AddCompositionTarget(this);
			{
				foreach (ServerCompositionVisual attachedVisual in _attachedVisuals)
				{
					attachedVisual.Activate();
				}
				return;
			}
		}
		_compositor.RemoveCompositionTarget(this);
		foreach (ServerCompositionVisual attachedVisual2 in _attachedVisuals)
		{
			attachedVisual2.Deactivate();
		}
	}

	private void OnDebugOverlaysChanged()
	{
		if ((DebugOverlays & RendererDebugOverlays.Fps) == 0)
		{
			_fpsCounter?.Reset();
		}
		if ((DebugOverlays & RendererDebugOverlays.LayoutTimeGraph) == 0)
		{
			_layoutTimeGraph?.Reset();
		}
		if ((DebugOverlays & RendererDebugOverlays.RenderTimeGraph) == 0)
		{
			_renderTimeGraph?.Reset();
		}
	}

	private void OnLastLayoutPassTimingChanged()
	{
		if ((DebugOverlays & RendererDebugOverlays.LayoutTimeGraph) != 0)
		{
			LayoutTimeGraph?.AddFrameValue(LastLayoutPassTiming.Elapsed.TotalMilliseconds);
		}
	}

	protected override void DeserializeChangesCore(BatchStreamReader reader, TimeSpan committedAt)
	{
		base.DeserializeChangesCore(reader, committedAt);
		DeserializeChangesExtra(reader);
		CompositionTargetChangedFields num = reader.Read<CompositionTargetChangedFields>();
		if ((num & CompositionTargetChangedFields.Root) == CompositionTargetChangedFields.Root)
		{
			Root = reader.ReadObject<ServerCompositionVisual>();
		}
		if ((num & CompositionTargetChangedFields.IsEnabled) == CompositionTargetChangedFields.IsEnabled)
		{
			IsEnabled = reader.Read<bool>();
		}
		if ((num & CompositionTargetChangedFields.DebugOverlays) == CompositionTargetChangedFields.DebugOverlays)
		{
			DebugOverlays = reader.Read<RendererDebugOverlays>();
		}
		if ((num & CompositionTargetChangedFields.LastLayoutPassTiming) == CompositionTargetChangedFields.LastLayoutPassTiming)
		{
			LastLayoutPassTiming = reader.Read<LayoutPassTiming>();
		}
		if ((num & CompositionTargetChangedFields.Scaling) == CompositionTargetChangedFields.Scaling)
		{
			Scaling = reader.Read<double>();
		}
		if ((num & CompositionTargetChangedFields.Size) == CompositionTargetChangedFields.Size)
		{
			Size = reader.Read<Size>();
		}
	}

	public override ExpressionVariant GetPropertyForAnimation(string name)
	{
		if (name == "IsEnabled")
		{
			return IsEnabled;
		}
		return base.GetPropertyForAnimation(name);
	}

	public override CompositionProperty? GetCompositionProperty(string name)
	{
		if (name == "IsEnabled")
		{
			return s_IdOfIsEnabledProperty;
		}
		return base.GetCompositionProperty(name);
	}
}
