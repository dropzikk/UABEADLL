using System.Runtime.CompilerServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComIGraphicsEffectSourceVTable : __MicroComIInspectableVTable
{
	protected __MicroComIGraphicsEffectSourceVTable()
	{
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IGraphicsEffectSource), new __MicroComIGraphicsEffectSourceVTable().CreateVTable());
	}
}
