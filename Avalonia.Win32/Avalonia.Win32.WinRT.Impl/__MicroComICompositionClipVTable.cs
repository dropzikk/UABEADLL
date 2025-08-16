using System.Runtime.CompilerServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComICompositionClipVTable : __MicroComIInspectableVTable
{
	protected __MicroComICompositionClipVTable()
	{
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(ICompositionClip), new __MicroComICompositionClipVTable().CreateVTable());
	}
}
