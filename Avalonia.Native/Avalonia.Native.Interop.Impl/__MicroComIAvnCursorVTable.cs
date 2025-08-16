using System.Runtime.CompilerServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnCursorVTable : MicroComVtblBase
{
	protected __MicroComIAvnCursorVTable()
	{
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IAvnCursor), new __MicroComIAvnCursorVTable().CreateVTable());
	}
}
