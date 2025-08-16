using System;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop;

internal interface IAvnClipboard : IUnknown, IDisposable
{
	IAvnString GetText(string type);

	void SetText(string type, string utf8Text);

	IAvnStringArray ObtainFormats();

	IAvnStringArray GetStrings(string type);

	unsafe void SetBytes(string type, void* utf8Text, int len);

	IAvnString GetBytes(string type);

	void Clear();
}
