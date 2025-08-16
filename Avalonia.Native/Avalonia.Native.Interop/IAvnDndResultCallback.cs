using System;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop;

internal interface IAvnDndResultCallback : IUnknown, IDisposable
{
	void OnDragAndDropComplete(AvnDragDropEffects effecct);
}
