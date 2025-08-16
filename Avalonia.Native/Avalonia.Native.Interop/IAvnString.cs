using System;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop;

internal interface IAvnString : IUnknown, IDisposable
{
	string String { get; }

	byte[] Bytes { get; }

	unsafe void* Pointer();

	int Length();
}
