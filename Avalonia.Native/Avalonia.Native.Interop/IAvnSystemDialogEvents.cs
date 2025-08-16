using System;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop;

internal interface IAvnSystemDialogEvents : IUnknown, IDisposable
{
	unsafe void OnCompleted(int numResults, void* ptrFirstResult);
}
