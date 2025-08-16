using System;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop;

internal interface IAvnTextInputMethodClient : IUnknown, IDisposable
{
	void SetPreeditText(string preeditText);

	void SelectInSurroundingText(int start, int length);
}
