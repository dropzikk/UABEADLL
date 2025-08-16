using System;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop;

internal interface IAvnMenu : IUnknown, IDisposable
{
	void RaiseNeedsUpdate();

	void RaiseOpening();

	void RaiseClosed();

	void Deinitialise();

	void InsertItem(int index, IAvnMenuItem item);

	void RemoveItem(IAvnMenuItem item);

	void SetTitle(string utf8String);

	void Clear();
}
