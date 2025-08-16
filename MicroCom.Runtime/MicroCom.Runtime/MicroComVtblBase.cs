using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MicroCom.Runtime;

public class MicroComVtblBase
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int AddRefDelegate(Ccw* ccw);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int QueryInterfaceDelegate(Ccw* ccw, Guid* guid, void** ppv);

	private List<IntPtr> _methods = new List<IntPtr>();

	public static IntPtr Vtable { get; } = new MicroComVtblBase().CreateVTable();

	public unsafe MicroComVtblBase()
	{
		AddMethod((delegate*<Ccw*, Guid*, void**, int>)(&QueryInterface));
		AddMethod((delegate*<Ccw*, int>)(&AddRef));
		AddMethod((delegate*<Ccw*, int>)(&Release));
	}

	protected void AddMethod(Delegate d)
	{
		GCHandle.Alloc(d);
		_methods.Add(Marshal.GetFunctionPointerForDelegate(d));
	}

	protected unsafe void AddMethod(void* m)
	{
		_methods.Add(new IntPtr(m));
	}

	protected unsafe IntPtr CreateVTable()
	{
		IntPtr* ptr = (IntPtr*)(void*)Marshal.AllocHGlobal((IntPtr.Size + 1) * _methods.Count);
		for (int i = 0; i < _methods.Count; i++)
		{
			ptr[i] = _methods[i];
		}
		return new IntPtr(ptr);
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int QueryInterface(Ccw* ccw, Guid* guid, void** ppv)
	{
		return ccw->GetShadow().QueryInterface(ccw, guid, ppv);
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int AddRef(Ccw* ccw)
	{
		return ccw->GetShadow().AddRef(ccw);
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int Release(Ccw* ccw)
	{
		return ccw->GetShadow().Release(ccw);
	}
}
