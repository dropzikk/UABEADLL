using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace SixLabors.ImageSharp.Memory.Internals;

internal sealed class Gen2GcCallback : CriticalFinalizerObject
{
	private readonly Func<bool>? callback0;

	private readonly Func<object, bool>? callback1;

	private GCHandle weakTargetObj;

	private Gen2GcCallback(Func<bool> callback)
	{
		callback0 = callback;
	}

	private Gen2GcCallback(Func<object, bool> callback, object targetObj)
	{
		callback1 = callback;
		weakTargetObj = GCHandle.Alloc(targetObj, GCHandleType.Weak);
	}

	~Gen2GcCallback()
	{
		if (weakTargetObj.IsAllocated)
		{
			object target = weakTargetObj.Target;
			if (target == null)
			{
				weakTargetObj.Free();
				return;
			}
			try
			{
				if (!callback1(target))
				{
					weakTargetObj.Free();
					return;
				}
			}
			catch
			{
			}
		}
		else
		{
			try
			{
				if (!callback0())
				{
					return;
				}
			}
			catch
			{
			}
		}
		GC.ReRegisterForFinalize(this);
	}

	public static void Register(Func<bool> callback)
	{
		new Gen2GcCallback(callback);
	}

	public static void Register(Func<object, bool> callback, object targetObj)
	{
		new Gen2GcCallback(callback, targetObj);
	}
}
