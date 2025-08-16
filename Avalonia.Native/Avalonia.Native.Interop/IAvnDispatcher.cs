using System;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop;

internal interface IAvnDispatcher : IUnknown, IDisposable
{
	void Post(IAvnActionCallback cb);
}
