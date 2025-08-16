using System.Runtime.CompilerServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.DirectX.Impl;

internal class __MicroComID3D11Texture2DVTable : MicroComVtblBase
{
	protected __MicroComID3D11Texture2DVTable()
	{
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(ID3D11Texture2D), new __MicroComID3D11Texture2DVTable().CreateVTable());
	}
}
