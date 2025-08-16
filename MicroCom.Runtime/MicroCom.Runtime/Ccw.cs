using System;
using System.Runtime.InteropServices;

namespace MicroCom.Runtime;

internal struct Ccw
{
	public IntPtr VTable;

	public IntPtr GcShadowHandle;

	public volatile int RefCount;

	public MicroComShadow GetShadow()
	{
		return (MicroComShadow)GCHandle.FromIntPtr(GcShadowHandle).Target;
	}
}
