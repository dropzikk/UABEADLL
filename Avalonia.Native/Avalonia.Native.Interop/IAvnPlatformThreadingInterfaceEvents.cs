using System;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop;

internal interface IAvnPlatformThreadingInterfaceEvents : IUnknown, IDisposable
{
	void Signaled();

	void Timer();

	void ReadyForBackgroundProcessing();
}
