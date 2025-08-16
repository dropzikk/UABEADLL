using System;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop;

internal interface IAvnStringArray : IUnknown, IDisposable
{
	uint Count { get; }

	string[] ToStringArray();

	IAvnString Get(uint index);
}
