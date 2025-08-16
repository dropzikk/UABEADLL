using System;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop;

internal interface IAvnGlContext : IUnknown, IDisposable
{
	int SampleCount { get; }

	int StencilSize { get; }

	IntPtr NativeHandle { get; }

	IUnknown MakeCurrent();

	void LegacyMakeCurrent();
}
