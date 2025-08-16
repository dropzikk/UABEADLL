using System;
using MicroCom.Runtime;

namespace Avalonia.Win32.Win32Com;

internal interface IDropSource : IUnknown, IDisposable
{
	int QueryContinueDrag(int fEscapePressed, int grfKeyState);

	int GiveFeedback(DropEffect dwEffect);
}
