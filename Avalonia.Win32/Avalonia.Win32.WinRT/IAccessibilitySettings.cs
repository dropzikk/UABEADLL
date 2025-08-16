using System;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT;

internal interface IAccessibilitySettings : IInspectable, IUnknown, IDisposable
{
	int HighContrast { get; }

	IntPtr HighContrastScheme { get; }
}
