using System;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop;

internal interface IAvnPlatformThreadingInterface : IUnknown, IDisposable
{
	int CurrentThreadIsLoopThread { get; }

	void SetEvents(IAvnPlatformThreadingInterfaceEvents cb);

	IAvnLoopCancellation CreateLoopCancellation();

	void RunLoop(IAvnLoopCancellation cancel);

	void Signal();

	void UpdateTimer(int ms);

	void RequestBackgroundProcessing();
}
