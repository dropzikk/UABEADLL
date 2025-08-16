using System;
using Avalonia.Native.Interop;
using Avalonia.Threading;
using MicroCom.Runtime;

namespace Avalonia.Native;

internal class AvnDispatcher : NativeCallbackBase, IAvnDispatcher, IUnknown, IDisposable
{
	public void Post(IAvnActionCallback cb)
	{
		IAvnActionCallback callback = cb.CloneReference();
		Dispatcher.UIThread.Post(delegate
		{
			using (callback)
			{
				callback.Run();
			}
		}, DispatcherPriority.Send);
	}
}
