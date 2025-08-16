using System;
using Avalonia.Input.Raw;

namespace Avalonia;

internal interface IRawEventGrouperDispatchQueue
{
	void Add(RawInputEventArgs args, Action<RawInputEventArgs> handler);
}
