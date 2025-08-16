using System;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop;

internal interface IAvnAutomationPeerArray : IUnknown, IDisposable
{
	uint Count { get; }

	IAvnAutomationPeer Get(uint index);
}
