using System;
using MicroCom.Runtime;

namespace Avalonia.Win32.Win32Com;

internal interface IShellItem : IUnknown, IDisposable
{
	IShellItem Parent { get; }

	unsafe void* BindToHandler(void* pbc, Guid* bhid, Guid* riid);

	unsafe char* GetDisplayName(uint sigdnName);

	uint GetAttributes(uint sfgaoMask);

	int Compare(IShellItem psi, uint hint);
}
