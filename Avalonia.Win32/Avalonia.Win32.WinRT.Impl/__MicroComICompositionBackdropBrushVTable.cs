using System.Runtime.CompilerServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComICompositionBackdropBrushVTable : __MicroComIInspectableVTable
{
	protected __MicroComICompositionBackdropBrushVTable()
	{
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(ICompositionBackdropBrush), new __MicroComICompositionBackdropBrushVTable().CreateVTable());
	}
}
