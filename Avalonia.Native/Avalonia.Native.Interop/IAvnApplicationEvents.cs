using System;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop;

internal interface IAvnApplicationEvents : IUnknown, IDisposable
{
	void FilesOpened(IAvnStringArray urls);

	int TryShutdown();
}
