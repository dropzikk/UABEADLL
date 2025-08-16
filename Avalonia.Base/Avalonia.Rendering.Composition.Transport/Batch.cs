using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Avalonia.Rendering.Composition.Transport;

internal class Batch
{
	private static long _nextSequenceId = 1L;

	private static ConcurrentBag<BatchStreamData> _pool = new ConcurrentBag<BatchStreamData>();

	private readonly TaskCompletionSource<int> _acceptedTcs = new TaskCompletionSource<int>();

	private readonly TaskCompletionSource<int> _renderedTcs = new TaskCompletionSource<int>();

	public long SequenceId { get; }

	public BatchStreamData Changes { get; private set; }

	public TimeSpan CommittedAt { get; set; }

	public Task Processed => _acceptedTcs.Task;

	public Task Rendered => _renderedTcs.Task;

	public Batch()
	{
		SequenceId = Interlocked.Increment(ref _nextSequenceId);
		if (!_pool.TryTake(out BatchStreamData result))
		{
			result = new BatchStreamData();
		}
		Changes = result;
	}

	public void NotifyProcessed()
	{
		_pool.Add(Changes);
		Changes = null;
		_acceptedTcs.TrySetResult(0);
	}

	public void NotifyRendered()
	{
		_renderedTcs.TrySetResult(0);
	}
}
