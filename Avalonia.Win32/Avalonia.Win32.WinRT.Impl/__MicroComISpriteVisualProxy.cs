using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComISpriteVisualProxy : __MicroComIInspectableProxy, ISpriteVisual, IInspectable, IUnknown, IDisposable
{
	public unsafe ICompositionBrush Brush
	{
		get
		{
			void* pObject = null;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, &pObject);
			if (num != 0)
			{
				throw new COMException("GetBrush failed", num);
			}
			return MicroComRuntime.CreateProxyOrNullFor<ICompositionBrush>(pObject, ownsHandle: true);
		}
	}

	protected override int VTableSize => base.VTableSize + 2;

	public unsafe void SetBrush(ICompositionBrush value)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 1])(base.PPV, MicroComRuntime.GetNativePointer(value));
		if (num != 0)
		{
			throw new COMException("SetBrush failed", num);
		}
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(ISpriteVisual), new Guid("08E05581-1AD1-4F97-9757-402D76E4233B"), (IntPtr p, bool owns) => new __MicroComISpriteVisualProxy(p, owns));
	}

	protected __MicroComISpriteVisualProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
