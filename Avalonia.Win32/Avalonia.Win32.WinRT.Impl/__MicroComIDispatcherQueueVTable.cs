using System.Runtime.CompilerServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComIDispatcherQueueVTable : __MicroComIInspectableVTable
{
	protected __MicroComIDispatcherQueueVTable()
	{
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IDispatcherQueue), new __MicroComIDispatcherQueueVTable().CreateVTable());
	}
}
