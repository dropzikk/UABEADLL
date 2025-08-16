using System;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop;

internal interface IAvnPlatformBehaviorInhibition : IUnknown, IDisposable
{
	void SetInhibitAppSleep(int inhibitAppSleep, string reason);
}
