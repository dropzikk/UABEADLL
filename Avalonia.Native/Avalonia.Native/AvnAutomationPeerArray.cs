using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Automation.Peers;
using Avalonia.Native.Interop;
using MicroCom.Runtime;

namespace Avalonia.Native;

internal class AvnAutomationPeerArray : NativeCallbackBase, IAvnAutomationPeerArray, IUnknown, IDisposable
{
	private readonly AvnAutomationPeer[] _items;

	public uint Count => (uint)_items.Length;

	public AvnAutomationPeerArray(IReadOnlyList<AutomationPeer> items)
	{
		_items = items.Select((AutomationPeer x) => AvnAutomationPeer.Wrap(x)).ToArray();
	}

	public IAvnAutomationPeer Get(uint index)
	{
		return _items[index];
	}
}
