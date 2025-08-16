using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT;

internal class WinRTInspectable : IInspectable, IUnknown, IDisposable, IMicroComShadowContainer
{
	public IntPtr RuntimeClassName => NativeWinRTMethods.WindowsCreateString(GetType().FullName);

	public TrustLevel TrustLevel => TrustLevel.BaseTrust;

	public MicroComShadow? Shadow { get; set; }

	public virtual void Dispose()
	{
	}

	public unsafe void GetIids(ulong* iidCount, Guid** iids)
	{
		Guid[] array = GetType().GetInterfaces().Where(typeof(IUnknown).IsAssignableFrom).Select(MicroComRuntime.GetGuidFor)
			.ToArray();
		Guid* ptr = (Guid*)(void*)Marshal.AllocCoTaskMem(Unsafe.SizeOf<Guid>() * array.Length);
		for (int i = 0; i < array.Length; i++)
		{
			ptr[i] = array[i];
		}
		*iids = ptr;
		*iidCount = (ulong)array.Length;
	}

	public virtual void OnReferencedFromNative()
	{
	}

	public virtual void OnUnreferencedFromNative()
	{
	}
}
