using System;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop;

internal interface IAvnMetalDisplay : IUnknown, IDisposable
{
	IAvnMetalDevice CreateDevice();
}
