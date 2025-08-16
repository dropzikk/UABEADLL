using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnWindowBaseProxy : MicroComProxyBase, IAvnWindowBase, IUnknown, IDisposable
{
	public unsafe AvnSize ClientSize
	{
		get
		{
			AvnSize result = default(AvnSize);
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 4])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetClientSize failed", num);
			}
			return result;
		}
	}

	public unsafe double Scaling
	{
		get
		{
			double result = 0.0;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 6])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetScaling failed", num);
			}
			return result;
		}
	}

	public unsafe AvnPoint Position
	{
		get
		{
			AvnPoint result = default(AvnPoint);
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 12])(base.PPV, &result);
			if (num != 0)
			{
				throw new COMException("GetPosition failed", num);
			}
			return result;
		}
	}

	public unsafe IAvnTextInputMethod InputMethod
	{
		get
		{
			void* pObject = null;
			int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 30])(base.PPV, &pObject);
			if (num != 0)
			{
				throw new COMException("GetInputMethod failed", num);
			}
			return MicroComRuntime.CreateProxyOrNullFor<IAvnTextInputMethod>(pObject, ownsHandle: true);
		}
	}

	protected override int VTableSize => base.VTableSize + 31;

	public unsafe void Show(int activate, int isDialog)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, int, int, int>)(*base.PPV)[base.VTableSize])(base.PPV, activate, isDialog);
		if (num != 0)
		{
			throw new COMException("Show failed", num);
		}
	}

	public unsafe void Hide()
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize + 1])(base.PPV);
		if (num != 0)
		{
			throw new COMException("Hide failed", num);
		}
	}

	public unsafe void Close()
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize + 2])(base.PPV);
		if (num != 0)
		{
			throw new COMException("Close failed", num);
		}
	}

	public unsafe void Activate()
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize + 3])(base.PPV);
		if (num != 0)
		{
			throw new COMException("Activate failed", num);
		}
	}

	public unsafe void GetFrameSize(AvnSize* result)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 5])(base.PPV, result);
		if (num != 0)
		{
			throw new COMException("GetFrameSize failed", num);
		}
	}

	public unsafe void SetMinMaxSize(AvnSize minSize, AvnSize maxSize)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, AvnSize, AvnSize, int>)(*base.PPV)[base.VTableSize + 7])(base.PPV, minSize, maxSize);
		if (num != 0)
		{
			throw new COMException("SetMinMaxSize failed", num);
		}
	}

	public unsafe void Resize(double width, double height, AvnPlatformResizeReason reason)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, double, double, AvnPlatformResizeReason, int>)(*base.PPV)[base.VTableSize + 8])(base.PPV, width, height, reason);
		if (num != 0)
		{
			throw new COMException("Resize failed", num);
		}
	}

	public unsafe void Invalidate(AvnRect rect)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, AvnRect, int>)(*base.PPV)[base.VTableSize + 9])(base.PPV, rect);
		if (num != 0)
		{
			throw new COMException("Invalidate failed", num);
		}
	}

	public unsafe void BeginMoveDrag()
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize + 10])(base.PPV);
		if (num != 0)
		{
			throw new COMException("BeginMoveDrag failed", num);
		}
	}

	public unsafe void BeginResizeDrag(AvnWindowEdge edge)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, AvnWindowEdge, int>)(*base.PPV)[base.VTableSize + 11])(base.PPV, edge);
		if (num != 0)
		{
			throw new COMException("BeginResizeDrag failed", num);
		}
	}

	public unsafe void SetPosition(AvnPoint point)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, AvnPoint, int>)(*base.PPV)[base.VTableSize + 13])(base.PPV, point);
		if (num != 0)
		{
			throw new COMException("SetPosition failed", num);
		}
	}

	public unsafe AvnPoint PointToClient(AvnPoint point)
	{
		AvnPoint result = default(AvnPoint);
		int num = ((delegate* unmanaged[Stdcall]<void*, AvnPoint, void*, int>)(*base.PPV)[base.VTableSize + 14])(base.PPV, point, &result);
		if (num != 0)
		{
			throw new COMException("PointToClient failed", num);
		}
		return result;
	}

	public unsafe AvnPoint PointToScreen(AvnPoint point)
	{
		AvnPoint result = default(AvnPoint);
		int num = ((delegate* unmanaged[Stdcall]<void*, AvnPoint, void*, int>)(*base.PPV)[base.VTableSize + 15])(base.PPV, point, &result);
		if (num != 0)
		{
			throw new COMException("PointToScreen failed", num);
		}
		return result;
	}

	public unsafe void SetTopMost(int value)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, int, int>)(*base.PPV)[base.VTableSize + 16])(base.PPV, value);
		if (num != 0)
		{
			throw new COMException("SetTopMost failed", num);
		}
	}

	public unsafe void SetCursor(IAvnCursor cursor)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 17])(base.PPV, MicroComRuntime.GetNativePointer(cursor));
		if (num != 0)
		{
			throw new COMException("SetCursor failed", num);
		}
	}

	public unsafe IAvnGlSurfaceRenderTarget CreateGlRenderTarget(IAvnGlContext context)
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*base.PPV)[base.VTableSize + 18])(base.PPV, MicroComRuntime.GetNativePointer(context), &pObject);
		if (num != 0)
		{
			throw new COMException("CreateGlRenderTarget failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IAvnGlSurfaceRenderTarget>(pObject, ownsHandle: true);
	}

	public unsafe IAvnSoftwareRenderTarget CreateSoftwareRenderTarget()
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 19])(base.PPV, &pObject);
		if (num != 0)
		{
			throw new COMException("CreateSoftwareRenderTarget failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IAvnSoftwareRenderTarget>(pObject, ownsHandle: true);
	}

	public unsafe IAvnMetalRenderTarget CreateMetalRenderTarget(IAvnMetalDevice device)
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*base.PPV)[base.VTableSize + 20])(base.PPV, MicroComRuntime.GetNativePointer(device), &pObject);
		if (num != 0)
		{
			throw new COMException("CreateMetalRenderTarget failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IAvnMetalRenderTarget>(pObject, ownsHandle: true);
	}

	public unsafe void SetMainMenu(IAvnMenu menu)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 21])(base.PPV, MicroComRuntime.GetNativePointer(menu));
		if (num != 0)
		{
			throw new COMException("SetMainMenu failed", num);
		}
	}

	public unsafe IntPtr ObtainNSWindowHandle()
	{
		IntPtr result = default(IntPtr);
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 22])(base.PPV, &result);
		if (num != 0)
		{
			throw new COMException("ObtainNSWindowHandle failed", num);
		}
		return result;
	}

	public unsafe IntPtr ObtainNSWindowHandleRetained()
	{
		IntPtr result = default(IntPtr);
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 23])(base.PPV, &result);
		if (num != 0)
		{
			throw new COMException("ObtainNSWindowHandleRetained failed", num);
		}
		return result;
	}

	public unsafe IntPtr ObtainNSViewHandle()
	{
		IntPtr result = default(IntPtr);
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 24])(base.PPV, &result);
		if (num != 0)
		{
			throw new COMException("ObtainNSViewHandle failed", num);
		}
		return result;
	}

	public unsafe IntPtr ObtainNSViewHandleRetained()
	{
		IntPtr result = default(IntPtr);
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 25])(base.PPV, &result);
		if (num != 0)
		{
			throw new COMException("ObtainNSViewHandleRetained failed", num);
		}
		return result;
	}

	public unsafe IAvnNativeControlHost CreateNativeControlHost()
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 26])(base.PPV, &pObject);
		if (num != 0)
		{
			throw new COMException("CreateNativeControlHost failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IAvnNativeControlHost>(pObject, ownsHandle: true);
	}

	public unsafe void BeginDragAndDropOperation(AvnDragDropEffects effects, AvnPoint point, IAvnClipboard clipboard, IAvnDndResultCallback cb, IntPtr sourceHandle)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, AvnDragDropEffects, AvnPoint, void*, void*, IntPtr, int>)(*base.PPV)[base.VTableSize + 27])(base.PPV, effects, point, MicroComRuntime.GetNativePointer(clipboard), MicroComRuntime.GetNativePointer(cb), sourceHandle);
		if (num != 0)
		{
			throw new COMException("BeginDragAndDropOperation failed", num);
		}
	}

	public unsafe void SetTransparencyMode(AvnWindowTransparencyMode mode)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, AvnWindowTransparencyMode, int>)(*base.PPV)[base.VTableSize + 28])(base.PPV, mode);
		if (num != 0)
		{
			throw new COMException("SetTransparencyMode failed", num);
		}
	}

	public unsafe void SetFrameThemeVariant(AvnPlatformThemeVariant mode)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, AvnPlatformThemeVariant, int>)(*base.PPV)[base.VTableSize + 29])(base.PPV, mode);
		if (num != 0)
		{
			throw new COMException("SetFrameThemeVariant failed", num);
		}
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IAvnWindowBase), new Guid("e5aca675-02b7-4129-aa79-d6e417210bda"), (IntPtr p, bool owns) => new __MicroComIAvnWindowBaseProxy(p, owns));
	}

	protected __MicroComIAvnWindowBaseProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
