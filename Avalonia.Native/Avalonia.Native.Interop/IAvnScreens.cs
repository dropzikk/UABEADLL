using System;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop;

internal interface IAvnScreens : IUnknown, IDisposable
{
	int ScreenCount { get; }

	AvnScreen GetScreen(int index);
}
