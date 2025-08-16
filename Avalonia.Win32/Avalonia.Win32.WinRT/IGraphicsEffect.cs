using System;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT;

internal interface IGraphicsEffect : IInspectable, IUnknown, IDisposable
{
	IntPtr Name { get; }

	void SetName(IntPtr name);
}
