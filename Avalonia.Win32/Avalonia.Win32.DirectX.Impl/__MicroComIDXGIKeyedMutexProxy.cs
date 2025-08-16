using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.DirectX.Impl;

internal class __MicroComIDXGIKeyedMutexProxy : __MicroComIDXGIDeviceSubObjectProxy, IDXGIKeyedMutex, IDXGIDeviceSubObject, IDXGIObject, IUnknown, IDisposable
{
	protected override int VTableSize => base.VTableSize + 2;

	public unsafe void AcquireSync(ulong Key, uint dwMilliseconds)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, ulong, uint, int>)(*base.PPV)[base.VTableSize])(base.PPV, Key, dwMilliseconds);
		if (num != 0)
		{
			throw new COMException("AcquireSync failed", num);
		}
	}

	public unsafe void ReleaseSync(ulong Key)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, ulong, int>)(*base.PPV)[base.VTableSize + 1])(base.PPV, Key);
		if (num != 0)
		{
			throw new COMException("ReleaseSync failed", num);
		}
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IDXGIKeyedMutex), new Guid(" 9d8e1289-d7b3-465f-8126-250e349af85d"), (IntPtr p, bool owns) => new __MicroComIDXGIKeyedMutexProxy(p, owns));
	}

	protected __MicroComIDXGIKeyedMutexProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
