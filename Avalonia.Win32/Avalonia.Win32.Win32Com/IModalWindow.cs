using System;
using MicroCom.Runtime;

namespace Avalonia.Win32.Win32Com;

internal interface IModalWindow : IUnknown, IDisposable
{
	int Show(IntPtr hwndOwner);
}
