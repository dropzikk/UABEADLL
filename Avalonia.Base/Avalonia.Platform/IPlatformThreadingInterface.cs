using System;
using Avalonia.Metadata;
using Avalonia.Threading;

namespace Avalonia.Platform;

[PrivateApi]
public interface IPlatformThreadingInterface
{
	bool CurrentThreadIsLoopThread { get; }

	event Action<DispatcherPriority?>? Signaled;

	IDisposable StartTimer(DispatcherPriority priority, TimeSpan interval, Action tick);

	void Signal(DispatcherPriority priority);
}
