using System;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT;

internal interface IDispatcherQueueController : IInspectable, IUnknown, IDisposable
{
	IDispatcherQueue DispatcherQueue { get; }

	IAsyncAction ShutdownQueueAsync();
}
