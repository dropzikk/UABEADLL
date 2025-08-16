using System;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop;

internal interface IAvnApplicationCommands : IUnknown, IDisposable
{
	void HideApp();

	void ShowAll();

	void HideOthers();
}
