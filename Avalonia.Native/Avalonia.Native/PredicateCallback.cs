using System;
using Avalonia.Native.Interop;
using MicroCom.Runtime;

namespace Avalonia.Native;

internal class PredicateCallback : NativeCallbackBase, IAvnPredicateCallback, IUnknown, IDisposable
{
	private Func<bool> _predicate;

	public PredicateCallback(Func<bool> predicate)
	{
		_predicate = predicate;
	}

	int IAvnPredicateCallback.Evaluate()
	{
		return _predicate().AsComBool();
	}
}
