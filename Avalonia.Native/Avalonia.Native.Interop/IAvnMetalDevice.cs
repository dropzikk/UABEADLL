using System;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop;

internal interface IAvnMetalDevice : IUnknown, IDisposable
{
	IntPtr Device { get; }

	IntPtr Queue { get; }
}
