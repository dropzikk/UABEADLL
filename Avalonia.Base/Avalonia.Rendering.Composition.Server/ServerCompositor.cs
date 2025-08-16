using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Avalonia.Logging;
using Avalonia.Platform;
using Avalonia.Rendering.Composition.Transport;

namespace Avalonia.Rendering.Composition.Server;

internal class ServerCompositor : IRenderLoopTask
{
	private readonly IRenderLoop _renderLoop;

	private readonly Queue<Batch> _batches = new Queue<Batch>();

	private readonly Queue<Action> _receivedJobQueue = new Queue<Action>();

	private List<ServerCompositionTarget> _activeTargets = new List<ServerCompositionTarget>();

	private HashSet<IServerClockItem> _clockItems = new HashSet<IServerClockItem>();

	private List<IServerClockItem> _clockItemsToUpdate = new List<IServerClockItem>();

	internal BatchStreamObjectPool<object?> BatchObjectPool;

	internal BatchStreamMemoryPool BatchMemoryPool;

	private object _lock = new object();

	private Thread? _safeThread;

	internal static readonly object RenderThreadDisposeStartMarker = new object();

	internal static readonly object RenderThreadJobsStartMarker = new object();

	internal static readonly object RenderThreadJobsEndMarker = new object();

	private List<Batch> _reusableToNotifyProcessedList = new List<Batch>();

	private List<Batch> _reusableToNotifyRenderedList = new List<Batch>();

	private Queue<IServerRenderResource> _renderResourcesInvalidationQueue = new Queue<IServerRenderResource>();

	private HashSet<IServerRenderResource> _renderResourcesInvalidationSet = new HashSet<IServerRenderResource>();

	public long LastBatchId { get; private set; }

	public Stopwatch Clock { get; } = Stopwatch.StartNew();

	public TimeSpan ServerNow { get; private set; }

	public PlatformRenderInterfaceContextManager RenderInterface { get; }

	public ServerCompositor(IRenderLoop renderLoop, IPlatformGraphics? platformGraphics, BatchStreamObjectPool<object?> batchObjectPool, BatchStreamMemoryPool batchMemoryPool)
	{
		_renderLoop = renderLoop;
		RenderInterface = new PlatformRenderInterfaceContextManager(platformGraphics);
		BatchObjectPool = batchObjectPool;
		BatchMemoryPool = batchMemoryPool;
		_renderLoop.Add(this);
	}

	public void EnqueueBatch(Batch batch)
	{
		lock (_batches)
		{
			_batches.Enqueue(batch);
		}
	}

	internal void UpdateServerTime()
	{
		ServerNow = Clock.Elapsed;
	}

	private void ApplyPendingBatches()
	{
		while (true)
		{
			Batch batch;
			lock (_batches)
			{
				if (_batches.Count == 0)
				{
					break;
				}
				batch = _batches.Dequeue();
			}
			using (BatchStreamReader batchStreamReader = new BatchStreamReader(batch.Changes, BatchMemoryPool, BatchObjectPool))
			{
				while (!batchStreamReader.IsObjectEof)
				{
					object obj = batchStreamReader.ReadObject();
					if (obj == RenderThreadJobsStartMarker)
					{
						ReadServerJobs(batchStreamReader);
					}
					else if (obj == RenderThreadDisposeStartMarker)
					{
						ReadDisposeJobs(batchStreamReader);
					}
					else
					{
						((SimpleServerObject)obj).DeserializeChanges(batchStreamReader, batch);
					}
				}
			}
			_reusableToNotifyProcessedList.Add(batch);
			LastBatchId = batch.SequenceId;
		}
	}

	private void ReadServerJobs(BatchStreamReader reader)
	{
		object obj;
		while ((obj = reader.ReadObject()) != RenderThreadJobsEndMarker)
		{
			_receivedJobQueue.Enqueue((Action)obj);
		}
	}

	private void ReadDisposeJobs(BatchStreamReader reader)
	{
		for (int num = reader.Read<int>(); num > 0; num--)
		{
			(reader.ReadObject() as IDisposable)?.Dispose();
		}
	}

	private void ExecuteServerJobs()
	{
		while (_receivedJobQueue.Count > 0)
		{
			try
			{
				_receivedJobQueue.Dequeue()();
			}
			catch
			{
			}
		}
	}

	private void NotifyBatchesProcessed()
	{
		foreach (Batch reusableToNotifyProcessed in _reusableToNotifyProcessedList)
		{
			reusableToNotifyProcessed.NotifyProcessed();
		}
		foreach (Batch reusableToNotifyProcessed2 in _reusableToNotifyProcessedList)
		{
			_reusableToNotifyRenderedList.Add(reusableToNotifyProcessed2);
		}
		_reusableToNotifyProcessedList.Clear();
	}

	private void NotifyBatchesRendered()
	{
		foreach (Batch reusableToNotifyRendered in _reusableToNotifyRenderedList)
		{
			reusableToNotifyRendered.NotifyRendered();
		}
		_reusableToNotifyRenderedList.Clear();
	}

	public void Render()
	{
		lock (_lock)
		{
			try
			{
				_safeThread = Thread.CurrentThread;
				RenderCore();
			}
			finally
			{
				NotifyBatchesRendered();
				_safeThread = null;
			}
		}
	}

	private void RenderCore()
	{
		UpdateServerTime();
		ApplyPendingBatches();
		NotifyBatchesProcessed();
		foreach (IServerClockItem clockItem in _clockItems)
		{
			_clockItemsToUpdate.Add(clockItem);
		}
		foreach (IServerClockItem item in _clockItemsToUpdate)
		{
			item.OnTick();
		}
		_clockItemsToUpdate.Clear();
		ApplyEnqueuedRenderResourceChanges();
		try
		{
			RenderInterface.EnsureValidBackendContext();
			ExecuteServerJobs();
			foreach (ServerCompositionTarget activeTarget in _activeTargets)
			{
				activeTarget.Render();
			}
		}
		catch (Exception propertyValue)
		{
			Logger.TryGet(LogEventLevel.Error, "Visual")?.Log(this, "Exception when rendering: {Error}", propertyValue);
		}
	}

	public void AddCompositionTarget(ServerCompositionTarget target)
	{
		_activeTargets.Add(target);
	}

	public void RemoveCompositionTarget(ServerCompositionTarget target)
	{
		_activeTargets.Remove(target);
	}

	public void AddToClock(IServerClockItem item)
	{
		_clockItems.Add(item);
	}

	public void RemoveFromClock(IServerClockItem item)
	{
		_clockItems.Remove(item);
	}

	public IRenderTarget CreateRenderTarget(IEnumerable<object> surfaces)
	{
		using (RenderInterface.EnsureCurrent())
		{
			return RenderInterface.CreateRenderTarget(surfaces);
		}
	}

	public bool CheckAccess()
	{
		return _safeThread == Thread.CurrentThread;
	}

	public void VerifyAccess()
	{
		if (!CheckAccess())
		{
			throw new InvalidOperationException("This object can be only accessed under compositor lock");
		}
	}

	public void ApplyEnqueuedRenderResourceChanges()
	{
		IServerRenderResource result;
		while (_renderResourcesInvalidationQueue.TryDequeue(out result))
		{
			result.QueuedInvalidate();
		}
		_renderResourcesInvalidationSet.Clear();
	}

	public void EnqueueRenderResourceForInvalidation(IServerRenderResource resource)
	{
		if (_renderResourcesInvalidationSet.Add(resource))
		{
			_renderResourcesInvalidationQueue.Enqueue(resource);
		}
	}
}
