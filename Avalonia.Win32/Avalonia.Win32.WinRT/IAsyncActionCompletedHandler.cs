using System;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT;

internal interface IAsyncActionCompletedHandler : IUnknown, IDisposable
{
	void Invoke(IAsyncAction asyncInfo, AsyncStatus asyncStatus);
}
