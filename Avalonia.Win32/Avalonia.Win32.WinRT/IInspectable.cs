using System;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT;

internal interface IInspectable : IUnknown, IDisposable
{
	IntPtr RuntimeClassName { get; }

	TrustLevel TrustLevel { get; }

	unsafe void GetIids(ulong* iidCount, Guid** iids);
}
