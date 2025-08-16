using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Animation.Easings;
using Avalonia.Media;
using Avalonia.Metadata;
using Avalonia.Platform;
using Avalonia.Rendering.Composition.Animations;
using Avalonia.Rendering.Composition.Server;
using Avalonia.Rendering.Composition.Transport;
using Avalonia.Threading;
using Avalonia.Utilities;

namespace Avalonia.Rendering.Composition;

public class Compositor
{
	private ServerCompositor _server;

	private Batch? _nextCommit;

	private BatchStreamObjectPool<object?> _batchObjectPool;

	private BatchStreamMemoryPool _batchMemoryPool;

	private Queue<ICompositorSerializable> _objectSerializationQueue = new Queue<ICompositorSerializable>();

	private HashSet<ICompositorSerializable> _objectSerializationHashSet = new HashSet<ICompositorSerializable>();

	private Queue<Action> _invokeBeforeCommitWrite = new Queue<Action>();

	private Queue<Action> _invokeBeforeCommitRead = new Queue<Action>();

	private HashSet<IDisposable> _disposeOnNextBatch = new HashSet<IDisposable>();

	private Batch? _pendingBatch;

	private readonly object _pendingBatchLock = new object();

	private List<Action> _pendingServerCompositorJobs = new List<Action>();

	private DiagnosticTextRenderer? _diagnosticTextRenderer;

	private Action _triggerCommitRequested;

	internal IRenderLoop Loop { get; }

	internal bool UseUiThreadForSynchronousCommits { get; }

	internal ServerCompositor Server => _server;

	internal IEasing DefaultEasing { get; }

	private DiagnosticTextRenderer? DiagnosticTextRenderer
	{
		get
		{
			if (_diagnosticTextRenderer == null)
			{
				if (AvaloniaLocator.Current.GetService<IFontManagerImpl>() == null)
				{
					return null;
				}
				_diagnosticTextRenderer = new DiagnosticTextRenderer(Typeface.Default.GlyphTypeface, 12.0);
			}
			return _diagnosticTextRenderer;
		}
	}

	internal event Action? AfterCommit;

	[PrivateApi]
	public Compositor(IPlatformGraphics? gpu, bool useUiThreadForSynchronousCommits = false)
		: this(RenderLoop.LocatorAutoInstance, gpu, useUiThreadForSynchronousCommits)
	{
	}

	internal Compositor(IRenderLoop loop, IPlatformGraphics? gpu, bool useUiThreadForSynchronousCommits = false)
		: this(loop, gpu, useUiThreadForSynchronousCommits, MediaContext.Instance, reclaimBuffersImmediately: false)
	{
	}

	internal Compositor(IRenderLoop loop, IPlatformGraphics? gpu, bool useUiThreadForSynchronousCommits, ICompositorScheduler scheduler, bool reclaimBuffersImmediately)
	{
		Compositor compositor = this;
		Loop = loop;
		UseUiThreadForSynchronousCommits = useUiThreadForSynchronousCommits;
		_batchMemoryPool = new BatchStreamMemoryPool(reclaimBuffersImmediately);
		_batchObjectPool = new BatchStreamObjectPool<object>(reclaimBuffersImmediately);
		_server = new ServerCompositor(loop, gpu, _batchObjectPool, _batchMemoryPool);
		_triggerCommitRequested = delegate
		{
			scheduler.CommitRequested(compositor);
		};
		DefaultEasing = new CubicBezierEasing(new Point(0.25, 0.10000000149011612), new Point(0.25, 1.0));
	}

	public Task RequestCommitAsync()
	{
		Dispatcher.UIThread.VerifyAccess();
		if (_nextCommit == null)
		{
			_nextCommit = new Batch();
			Batch pendingBatch = _pendingBatch;
			if (pendingBatch != null)
			{
				pendingBatch.Processed.ContinueWith(delegate
				{
					Dispatcher.UIThread.Post(_triggerCommitRequested, DispatcherPriority.Send);
				});
			}
			else
			{
				_triggerCommitRequested();
			}
		}
		return _nextCommit.Processed;
	}

	internal Batch Commit()
	{
		try
		{
			return CommitCore();
		}
		finally
		{
			if (_invokeBeforeCommitWrite.Count > 0)
			{
				RequestCommitAsync();
			}
			this.AfterCommit?.Invoke();
		}
	}

	private Batch CommitCore()
	{
		Dispatcher.UIThread.VerifyAccess();
		using (NonPumpingLockHelper.Use())
		{
			Batch result = _nextCommit ?? (_nextCommit = new Batch());
			Queue<Action> invokeBeforeCommitWrite = _invokeBeforeCommitWrite;
			Queue<Action> invokeBeforeCommitRead = _invokeBeforeCommitRead;
			_invokeBeforeCommitRead = invokeBeforeCommitWrite;
			_invokeBeforeCommitWrite = invokeBeforeCommitRead;
			while (_invokeBeforeCommitRead.Count > 0)
			{
				_invokeBeforeCommitRead.Dequeue()();
			}
			using (BatchStreamWriter batchStreamWriter = new BatchStreamWriter(_nextCommit.Changes, _batchMemoryPool, _batchObjectPool))
			{
				ICompositorSerializable result2;
				while (_objectSerializationQueue.TryDequeue(out result2))
				{
					SimpleServerObject simpleServerObject = result2.TryGetServer(this);
					if (simpleServerObject != null)
					{
						batchStreamWriter.WriteObject(simpleServerObject);
						result2.SerializeChanges(this, batchStreamWriter);
					}
				}
				_objectSerializationHashSet.Clear();
				if (_disposeOnNextBatch.Count != 0)
				{
					batchStreamWriter.WriteObject(ServerCompositor.RenderThreadDisposeStartMarker);
					batchStreamWriter.Write(_disposeOnNextBatch.Count);
					foreach (IDisposable item in _disposeOnNextBatch)
					{
						batchStreamWriter.WriteObject(item);
					}
					_disposeOnNextBatch.Clear();
				}
				if (_pendingServerCompositorJobs.Count > 0)
				{
					batchStreamWriter.WriteObject(ServerCompositor.RenderThreadJobsStartMarker);
					foreach (Action pendingServerCompositorJob in _pendingServerCompositorJobs)
					{
						batchStreamWriter.WriteObject(pendingServerCompositorJob);
					}
					batchStreamWriter.WriteObject(ServerCompositor.RenderThreadJobsEndMarker);
				}
				_pendingServerCompositorJobs.Clear();
			}
			_nextCommit.CommittedAt = Server.Clock.Elapsed;
			_server.EnqueueBatch(_nextCommit);
			lock (_pendingBatchLock)
			{
				_pendingBatch = _nextCommit;
				_pendingBatch.Processed.ContinueWith(delegate(Task t)
				{
					lock (_pendingBatchLock)
					{
						if (_pendingBatch.Processed == t)
						{
							_pendingBatch = null;
						}
					}
				}, TaskContinuationOptions.ExecuteSynchronously);
				_nextCommit = null;
				return result;
			}
		}
	}

	internal void RegisterForSerialization(ICompositorSerializable compositionObject)
	{
		Dispatcher.UIThread.VerifyAccess();
		if (_objectSerializationHashSet.Add(compositionObject))
		{
			_objectSerializationQueue.Enqueue(compositionObject);
		}
		RequestCommitAsync();
	}

	internal void DisposeOnNextBatch(SimpleServerObject obj)
	{
		if (obj is IDisposable item && _disposeOnNextBatch.Add(item))
		{
			RequestCommitAsync();
		}
	}

	public void RequestCompositionUpdate(Action action)
	{
		Dispatcher.UIThread.VerifyAccess();
		_invokeBeforeCommitWrite.Enqueue(action);
		RequestCommitAsync();
	}

	internal void PostServerJob(Action job)
	{
		Dispatcher.UIThread.VerifyAccess();
		_pendingServerCompositorJobs.Add(job);
		RequestCommitAsync();
	}

	internal Task InvokeServerJobAsync(Action job)
	{
		return InvokeServerJobAsync(delegate
		{
			job();
			return (object?)null;
		});
	}

	internal Task<T> InvokeServerJobAsync<T>(Func<T> job)
	{
		TaskCompletionSource<T> tcs = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
		PostServerJob(delegate
		{
			try
			{
				tcs.SetResult(job());
			}
			catch (Exception exception)
			{
				tcs.TrySetException(exception);
			}
		});
		return tcs.Task;
	}

	public ValueTask<object?> TryGetRenderInterfaceFeature(Type featureType)
	{
		return new ValueTask<object>(InvokeServerJobAsync(delegate
		{
			using (Server.RenderInterface.EnsureCurrent())
			{
				return Server.RenderInterface.Value.TryGetFeature(featureType);
			}
		}));
	}

	public ValueTask<ICompositionGpuInterop?> TryGetCompositionGpuInterop()
	{
		return new ValueTask<ICompositionGpuInterop>(InvokeServerJobAsync(delegate
		{
			using (Server.RenderInterface.EnsureCurrent())
			{
				IExternalObjectsRenderInterfaceContextFeature externalObjectsRenderInterfaceContextFeature = Server.RenderInterface.Value.TryGetFeature<IExternalObjectsRenderInterfaceContextFeature>();
				if (externalObjectsRenderInterfaceContextFeature == null)
				{
					return (ICompositionGpuInterop?)null;
				}
				return new CompositionInterop(this, externalObjectsRenderInterfaceContextFeature);
			}
		}));
	}

	internal bool UnitTestIsRegisteredForSerialization(ICompositorSerializable serializable)
	{
		return _objectSerializationHashSet.Contains(serializable);
	}

	internal CompositionTarget CreateCompositionTarget(Func<IEnumerable<object>> surfaces)
	{
		return new CompositionTarget(this, new ServerCompositionTarget(_server, surfaces, DiagnosticTextRenderer));
	}

	public CompositionContainerVisual CreateContainerVisual()
	{
		return new CompositionContainerVisual(this, new ServerCompositionContainerVisual(_server));
	}

	public ExpressionAnimation CreateExpressionAnimation()
	{
		return new ExpressionAnimation(this);
	}

	public ExpressionAnimation CreateExpressionAnimation(string expression)
	{
		return new ExpressionAnimation(this)
		{
			Expression = expression
		};
	}

	public ImplicitAnimationCollection CreateImplicitAnimationCollection()
	{
		return new ImplicitAnimationCollection(this);
	}

	public CompositionAnimationGroup CreateAnimationGroup()
	{
		return new CompositionAnimationGroup(this);
	}

	public CompositionSolidColorVisual CreateSolidColorVisual()
	{
		return new CompositionSolidColorVisual(this, new ServerCompositionSolidColorVisual(Server));
	}

	public CompositionCustomVisual CreateCustomVisual(CompositionCustomVisualHandler handler)
	{
		return new CompositionCustomVisual(this, handler);
	}

	public CompositionSurfaceVisual CreateSurfaceVisual()
	{
		return new CompositionSurfaceVisual(this, new ServerCompositionSurfaceVisual(_server));
	}

	public CompositionDrawingSurface CreateDrawingSurface()
	{
		return new CompositionDrawingSurface(this);
	}

	public ScalarKeyFrameAnimation CreateScalarKeyFrameAnimation()
	{
		return new ScalarKeyFrameAnimation(this);
	}

	public DoubleKeyFrameAnimation CreateDoubleKeyFrameAnimation()
	{
		return new DoubleKeyFrameAnimation(this);
	}

	public BooleanKeyFrameAnimation CreateBooleanKeyFrameAnimation()
	{
		return new BooleanKeyFrameAnimation(this);
	}

	public ColorKeyFrameAnimation CreateColorKeyFrameAnimation()
	{
		return new ColorKeyFrameAnimation(this);
	}

	public VectorKeyFrameAnimation CreateVectorKeyFrameAnimation()
	{
		return new VectorKeyFrameAnimation(this);
	}

	public Vector2KeyFrameAnimation CreateVector2KeyFrameAnimation()
	{
		return new Vector2KeyFrameAnimation(this);
	}

	public Vector3KeyFrameAnimation CreateVector3KeyFrameAnimation()
	{
		return new Vector3KeyFrameAnimation(this);
	}

	public Vector3DKeyFrameAnimation CreateVector3DKeyFrameAnimation()
	{
		return new Vector3DKeyFrameAnimation(this);
	}

	public Vector4KeyFrameAnimation CreateVector4KeyFrameAnimation()
	{
		return new Vector4KeyFrameAnimation(this);
	}

	public QuaternionKeyFrameAnimation CreateQuaternionKeyFrameAnimation()
	{
		return new QuaternionKeyFrameAnimation(this);
	}
}
