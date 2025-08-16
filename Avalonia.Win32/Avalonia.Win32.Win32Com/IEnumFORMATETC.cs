using System;
using Avalonia.Win32.Interop;
using MicroCom.Runtime;

namespace Avalonia.Win32.Win32Com;

internal interface IEnumFORMATETC : IUnknown, IDisposable
{
	unsafe uint Next(uint celt, FORMATETC* rgelt, uint* pceltFetched);

	uint Skip(uint celt);

	void Reset();

	IEnumFORMATETC Clone();
}
