using System;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop;

internal interface IAvnGCHandleDeallocatorCallback : IUnknown, IDisposable
{
	void FreeGCHandle(IntPtr handle);
}
