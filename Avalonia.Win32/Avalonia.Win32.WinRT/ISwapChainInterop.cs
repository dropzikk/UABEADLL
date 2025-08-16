using System;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT;

internal interface ISwapChainInterop : IUnknown, IDisposable
{
	void SetSwapChain(IUnknown swapChain);
}
