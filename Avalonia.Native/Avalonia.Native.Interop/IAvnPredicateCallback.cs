using System;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop;

internal interface IAvnPredicateCallback : IUnknown, IDisposable
{
	int Evaluate();
}
