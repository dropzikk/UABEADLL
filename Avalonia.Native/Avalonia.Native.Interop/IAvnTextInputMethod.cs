using System;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop;

internal interface IAvnTextInputMethod : IUnknown, IDisposable
{
	void SetClient(IAvnTextInputMethodClient client);

	void Reset();

	void SetCursorRect(AvnRect rect);

	void SetSurroundingText(string text, int anchorOffset, int cursorOffset);
}
