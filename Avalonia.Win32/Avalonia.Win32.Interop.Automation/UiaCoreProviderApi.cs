using System;
using System.Runtime.InteropServices;

namespace Avalonia.Win32.Interop.Automation;

internal static class UiaCoreProviderApi
{
	public const int UIA_E_ELEMENTNOTENABLED = -2147220992;

	[DllImport("UIAutomationCore.dll", CharSet = CharSet.Unicode)]
	public static extern bool UiaClientsAreListening();

	[DllImport("UIAutomationCore.dll", CharSet = CharSet.Unicode)]
	public static extern IntPtr UiaReturnRawElementProvider(IntPtr hwnd, IntPtr wParam, IntPtr lParam, IRawElementProviderSimple? el);

	[DllImport("UIAutomationCore.dll", CharSet = CharSet.Unicode)]
	public static extern int UiaHostProviderFromHwnd(IntPtr hwnd, [MarshalAs(UnmanagedType.Interface)] out IRawElementProviderSimple provider);

	[DllImport("UIAutomationCore.dll", CharSet = CharSet.Unicode)]
	public static extern int UiaRaiseAutomationEvent(IRawElementProviderSimple? provider, int id);

	[DllImport("UIAutomationCore.dll", CharSet = CharSet.Unicode)]
	public static extern int UiaRaiseAutomationPropertyChangedEvent(IRawElementProviderSimple? provider, int id, object? oldValue, object? newValue);

	[DllImport("UIAutomationCore.dll", CharSet = CharSet.Unicode)]
	public static extern int UiaRaiseStructureChangedEvent(IRawElementProviderSimple? provider, StructureChangeType structureChangeType, int[]? runtimeId, int runtimeIdLen);

	[DllImport("UIAutomationCore.dll", CharSet = CharSet.Unicode)]
	public static extern int UiaDisconnectProvider(IRawElementProviderSimple? provider);
}
