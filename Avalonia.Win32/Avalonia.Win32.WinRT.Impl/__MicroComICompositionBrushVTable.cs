using System.Runtime.CompilerServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComICompositionBrushVTable : __MicroComIInspectableVTable
{
	protected __MicroComICompositionBrushVTable()
	{
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(ICompositionBrush), new __MicroComICompositionBrushVTable().CreateVTable());
	}
}
