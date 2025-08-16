using System;
using System.Runtime.InteropServices;

namespace Avalonia.Win32.Interop;

internal class TaskBarList
{
	private static IntPtr s_taskBarList;

	private static UnmanagedMethods.HrInit? s_hrInitDelegate;

	private static UnmanagedMethods.MarkFullscreenWindow? s_markFullscreenWindowDelegate;

	public unsafe static void MarkFullscreen(IntPtr hwnd, bool fullscreen)
	{
		if (s_taskBarList == IntPtr.Zero)
		{
			Guid clsid = UnmanagedMethods.ShellIds.TaskBarList;
			Guid iid = UnmanagedMethods.ShellIds.ITaskBarList2;
			UnmanagedMethods.CoCreateInstance(ref clsid, IntPtr.Zero, 1, ref iid, out s_taskBarList);
			if (s_taskBarList != IntPtr.Zero)
			{
				UnmanagedMethods.ITaskBarList2VTable** ptr = (UnmanagedMethods.ITaskBarList2VTable**)s_taskBarList.ToPointer();
				if (s_hrInitDelegate == null)
				{
					s_hrInitDelegate = Marshal.GetDelegateForFunctionPointer<UnmanagedMethods.HrInit>((*ptr)->HrInit);
				}
				if (s_hrInitDelegate(s_taskBarList) != 0)
				{
					s_taskBarList = IntPtr.Zero;
				}
			}
		}
		if (s_taskBarList != IntPtr.Zero)
		{
			UnmanagedMethods.ITaskBarList2VTable** ptr2 = (UnmanagedMethods.ITaskBarList2VTable**)s_taskBarList.ToPointer();
			if (s_markFullscreenWindowDelegate == null)
			{
				s_markFullscreenWindowDelegate = Marshal.GetDelegateForFunctionPointer<UnmanagedMethods.MarkFullscreenWindow>((*ptr2)->MarkFullscreenWindow);
			}
			s_markFullscreenWindowDelegate(s_taskBarList, hwnd, fullscreen);
		}
	}
}
