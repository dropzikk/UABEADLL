using System.Runtime.CompilerServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnPopupVTable : __MicroComIAvnWindowBaseVTable
{
	protected __MicroComIAvnPopupVTable()
	{
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IAvnPopup), new __MicroComIAvnPopupVTable().CreateVTable());
	}
}
