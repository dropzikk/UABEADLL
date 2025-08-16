using System;
using MicroCom.Runtime;

namespace Avalonia.Win32.Win32Com;

internal interface IShellItemArray : IUnknown, IDisposable
{
	int Count { get; }

	unsafe void* BindToHandler(void* pbc, Guid* bhid, Guid* riid);

	unsafe void* GetPropertyStore(ushort flags, Guid* riid);

	unsafe void* GetPropertyDescriptionList(void* keyType, Guid* riid);

	ushort GetAttributes(int AttribFlags, ushort sfgaoMask);

	IShellItem GetItemAt(int dwIndex);

	unsafe void* EnumItems();
}
