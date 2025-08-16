using System;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop;

internal interface IAvnActionCallback : IUnknown, IDisposable
{
	void Run();
}
