using System;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop;

internal interface IAvnLoopCancellation : IUnknown, IDisposable
{
	void Cancel();
}
