using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComIGraphicsEffectD2D1InteropProxy : MicroComProxyBase, IGraphicsEffectD2D1Interop, IUnknown, IDisposable
{
	public unsafe Guid EffectId
	{
		get
		{
			Guid result = default(Guid);
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetEffectId failed", num);
			}
			return result;
		}
	}

	public unsafe uint PropertyCount
	{
		get
		{
			uint result = 0u;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 2])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetPropertyCount failed", num);
			}
			return result;
		}
	}

	public unsafe uint SourceCount
	{
		get
		{
			uint result = 0u;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 5])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetSourceCount failed", num);
			}
			return result;
		}
	}

	protected override int VTableSize => base.VTableSize + 6;

	public unsafe void GetNamedPropertyMapping(IntPtr name, uint* index, GRAPHICS_EFFECT_PROPERTY_MAPPING* mapping)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, IntPtr, void*, void*, int>)(*base.PPV)[base.VTableSize + 1])(base.PPV, name, index, mapping);
		if (num != 0)
		{
			throw new COMException("GetNamedPropertyMapping failed", num);
		}
	}

	public unsafe IPropertyValue GetProperty(uint index)
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, uint, void*, int>)(*base.PPV)[base.VTableSize + 3])(base.PPV, index, &pObject);
		if (num != 0)
		{
			throw new COMException("GetProperty failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IPropertyValue>(pObject, ownsHandle: true);
	}

	public unsafe IGraphicsEffectSource GetSource(uint index)
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, uint, void*, int>)(*base.PPV)[base.VTableSize + 4])(base.PPV, index, &pObject);
		if (num != 0)
		{
			throw new COMException("GetSource failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IGraphicsEffectSource>(pObject, ownsHandle: true);
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IGraphicsEffectD2D1Interop), new Guid("2FC57384-A068-44D7-A331-30982FCF7177"), (IntPtr p, bool owns) => new __MicroComIGraphicsEffectD2D1InteropProxy(p, owns));
	}

	protected __MicroComIGraphicsEffectD2D1InteropProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
