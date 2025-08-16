using Avalonia.Controls.Platform;

namespace Avalonia;

internal class ManualRawEventGrouperDispatchQueueDispatcherInputProvider : ManagedDispatcherImpl.IManagedDispatcherInputProvider
{
	private readonly ManualRawEventGrouperDispatchQueue _queue;

	public bool HasInput => _queue.HasJobs;

	public ManualRawEventGrouperDispatchQueueDispatcherInputProvider(ManualRawEventGrouperDispatchQueue queue)
	{
		_queue = queue;
	}

	public void DispatchNextInputEvent()
	{
		_queue.DispatchNext();
	}
}
