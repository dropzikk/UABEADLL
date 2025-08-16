using System;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop;

internal interface IAvnGlDisplay : IUnknown, IDisposable
{
	IAvnGlContext CreateContext(IAvnGlContext share);

	void LegacyClearCurrentContext();

	IAvnGlContext WrapContext(IntPtr native);

	IntPtr GetProcAddress(string proc);
}
