using System;
using System.Runtime.InteropServices;
using System.Threading;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT;

internal static class NativeWinRTMethods
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private delegate int GetActivationFactoryDelegate(IntPtr classId, out IntPtr ppv);

	internal enum DISPATCHERQUEUE_THREAD_APARTMENTTYPE
	{
		DQTAT_COM_NONE,
		DQTAT_COM_ASTA,
		DQTAT_COM_STA
	}

	internal enum DISPATCHERQUEUE_THREAD_TYPE
	{
		DQTYPE_THREAD_DEDICATED = 1,
		DQTYPE_THREAD_CURRENT
	}

	internal struct DispatcherQueueOptions
	{
		public int dwSize;

		[MarshalAs(UnmanagedType.I4)]
		public DISPATCHERQUEUE_THREAD_TYPE threadType;

		[MarshalAs(UnmanagedType.I4)]
		public DISPATCHERQUEUE_THREAD_APARTMENTTYPE apartmentType;
	}

	internal enum RO_INIT_TYPE
	{
		RO_INIT_SINGLETHREADED,
		RO_INIT_MULTITHREADED
	}

	private static bool s_initialized;

	[DllImport("api-ms-win-core-winrt-string-l1-1-0.dll", CallingConvention = CallingConvention.StdCall, PreserveSig = false)]
	internal static extern IntPtr WindowsCreateString([MarshalAs(UnmanagedType.LPWStr)] string sourceString, int length);

	internal static IntPtr WindowsCreateString(string sourceString)
	{
		return WindowsCreateString(sourceString, sourceString.Length);
	}

	[DllImport("api-ms-win-core-winrt-string-l1-1-0.dll", CallingConvention = CallingConvention.StdCall)]
	internal unsafe static extern char* WindowsGetStringRawBuffer(IntPtr hstring, uint* length);

	[DllImport("api-ms-win-core-winrt-string-l1-1-0.dll", CallingConvention = CallingConvention.StdCall, PreserveSig = false)]
	internal static extern void WindowsDeleteString(IntPtr hString);

	[DllImport("Windows.UI.Composition", CallingConvention = CallingConvention.StdCall, EntryPoint = "DllGetActivationFactory", PreserveSig = false)]
	private static extern IntPtr GetWindowsUICompositionActivationFactory(IntPtr activatableClassId);

	internal static IActivationFactory GetWindowsUICompositionActivationFactory(string className)
	{
		return MicroComRuntime.CreateProxyFor<IActivationFactory>(GetWindowsUICompositionActivationFactory(WindowsCreateString(className)), ownsHandle: true);
	}

	internal static T CreateInstance<T>(string fullName) where T : IUnknown
	{
		IntPtr intPtr = WindowsCreateString(fullName);
		EnsureRoInitialized();
		using IUnknown unknown = MicroComRuntime.CreateProxyFor<IUnknown>(RoActivateInstance(intPtr), ownsHandle: true);
		WindowsDeleteString(intPtr);
		return unknown.QueryInterface<T>();
	}

	internal static TFactory CreateActivationFactory<TFactory>(string fullName) where TFactory : IUnknown
	{
		IntPtr intPtr = WindowsCreateString(fullName);
		EnsureRoInitialized();
		Guid iid = MicroComRuntime.GetGuidFor(typeof(TFactory));
		using IUnknown unknown = MicroComRuntime.CreateProxyFor<IUnknown>(RoGetActivationFactory(intPtr, ref iid), ownsHandle: true);
		WindowsDeleteString(intPtr);
		return unknown.QueryInterface<TFactory>();
	}

	[DllImport("coremessaging.dll", PreserveSig = false)]
	internal static extern IntPtr CreateDispatcherQueueController(DispatcherQueueOptions options);

	[DllImport("combase.dll", PreserveSig = false)]
	private static extern void RoInitialize(RO_INIT_TYPE initType);

	[DllImport("combase.dll", PreserveSig = false)]
	private static extern IntPtr RoActivateInstance(IntPtr activatableClassId);

	[DllImport("combase.dll", PreserveSig = false)]
	private static extern IntPtr RoGetActivationFactory(IntPtr activatableClassId, ref Guid iid);

	private static void EnsureRoInitialized()
	{
		if (!s_initialized)
		{
			RoInitialize((Thread.CurrentThread.GetApartmentState() != 0) ? RO_INIT_TYPE.RO_INIT_MULTITHREADED : RO_INIT_TYPE.RO_INIT_SINGLETHREADED);
			s_initialized = true;
		}
	}
}
