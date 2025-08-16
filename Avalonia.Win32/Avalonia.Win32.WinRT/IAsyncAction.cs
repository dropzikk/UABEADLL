using System;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT;

internal interface IAsyncAction : IInspectable, IUnknown, IDisposable
{
	IAsyncActionCompletedHandler Completed { get; }

	void SetCompleted(IAsyncActionCompletedHandler handler);

	void GetResults();
}
