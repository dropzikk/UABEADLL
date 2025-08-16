using System;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop;

internal interface IAvnMenuEvents : IUnknown, IDisposable
{
	void NeedsUpdate();

	void Opening();

	void Closed();
}
