using System.Runtime.CompilerServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComICompositionSurfaceVTable : __MicroComIInspectableVTable
{
	protected __MicroComICompositionSurfaceVTable()
	{
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(ICompositionSurface), new __MicroComICompositionSurfaceVTable().CreateVTable());
	}
}
