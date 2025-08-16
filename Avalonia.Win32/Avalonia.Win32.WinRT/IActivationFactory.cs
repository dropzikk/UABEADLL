using System;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT;

internal interface IActivationFactory : IInspectable, IUnknown, IDisposable
{
	IntPtr ActivateInstance();
}
