using System;
using System.Runtime.CompilerServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnPlatformSettingsProxy : MicroComProxyBase, IAvnPlatformSettings, IUnknown, IDisposable
{
	public unsafe AvnPlatformThemeVariant PlatformTheme => ((delegate* unmanaged[Stdcall]<void*, AvnPlatformThemeVariant>)(*base.PPV)[base.VTableSize])(base.PPV);

	public unsafe uint AccentColor => ((delegate* unmanaged[Stdcall]<void*, uint>)(*base.PPV)[base.VTableSize + 1])(base.PPV);

	protected override int VTableSize => base.VTableSize + 3;

	public unsafe void RegisterColorsChange(IAvnActionCallback callback)
	{
		((delegate* unmanaged[Stdcall]<void*, void*, void>)(*base.PPV)[base.VTableSize + 2])(base.PPV, MicroComRuntime.GetNativePointer(callback));
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IAvnPlatformSettings), new Guid("d1f009cc-9d2d-493b-845d-90d2c104baae"), (IntPtr p, bool owns) => new __MicroComIAvnPlatformSettingsProxy(p, owns));
	}

	protected __MicroComIAvnPlatformSettingsProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
