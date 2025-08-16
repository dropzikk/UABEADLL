using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComICompositionScopedBatchProxy : __MicroComIInspectableProxy, ICompositionScopedBatch, IInspectable, IUnknown, IDisposable
{
	public unsafe int IsActive
	{
		get
		{
			int result = 0;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetIsActive failed", num);
			}
			return result;
		}
	}

	public unsafe int IsEnded
	{
		get
		{
			int result = 0;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 1])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetIsEnded failed", num);
			}
			return result;
		}
	}

	protected override int VTableSize => base.VTableSize + 7;

	public unsafe void End()
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize + 2])(base.PPV);
		if (num != 0)
		{
			throw new COMException("End failed", num);
		}
	}

	public unsafe void Resume()
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize + 3])(base.PPV);
		if (num != 0)
		{
			throw new COMException("Resume failed", num);
		}
	}

	public unsafe void Suspend()
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize + 4])(base.PPV);
		if (num != 0)
		{
			throw new COMException("Suspend failed", num);
		}
	}

	public unsafe int AddCompleted(void* handler)
	{
		int result = 0;
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*base.PPV)[base.VTableSize + 5])(base.PPV, handler, &result);
		if (num != 0)
		{
			throw new COMException("AddCompleted failed", num);
		}
		return result;
	}

	public unsafe void RemoveCompleted(int token)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, int, int>)(*base.PPV)[base.VTableSize + 6])(base.PPV, token);
		if (num != 0)
		{
			throw new COMException("RemoveCompleted failed", num);
		}
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(ICompositionScopedBatch), new Guid("0D00DAD0-FB07-46FD-8C72-6280D1A3D1DD"), (IntPtr p, bool owns) => new __MicroComICompositionScopedBatchProxy(p, owns));
	}

	protected __MicroComICompositionScopedBatchProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
