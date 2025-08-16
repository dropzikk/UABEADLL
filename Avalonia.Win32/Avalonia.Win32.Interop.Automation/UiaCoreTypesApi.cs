using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Avalonia.Win32.Interop.Automation;

internal static class UiaCoreTypesApi
{
	internal enum AutomationIdType
	{
		Property,
		Pattern,
		Event,
		ControlType,
		TextAttribute
	}

	internal const int UIA_E_ELEMENTNOTENABLED = -2147220992;

	internal const int UIA_E_ELEMENTNOTAVAILABLE = -2147220991;

	internal const int UIA_E_NOCLICKABLEPOINT = -2147220990;

	internal const int UIA_E_PROXYASSEMBLYNOTLOADED = -2147220989;

	private static bool? s_isNetComInteropAvailable;

	internal static bool IsNetComInteropAvailable
	{
		get
		{
			bool valueOrDefault = s_isNetComInteropAvailable == true;
			if (!s_isNetComInteropAvailable.HasValue)
			{
				valueOrDefault = GetIsNetComInteropAvailable();
				s_isNetComInteropAvailable = valueOrDefault;
				return valueOrDefault;
			}
			return valueOrDefault;
		}
	}

	internal static int UiaLookupId(AutomationIdType type, ref Guid guid)
	{
		return RawUiaLookupId(type, ref guid);
	}

	[RequiresUnreferencedCode("Requires .NET COM interop")]
	internal static object UiaGetReservedNotSupportedValue()
	{
		CheckError(RawUiaGetReservedNotSupportedValue(out object notSupportedValue));
		return notSupportedValue;
	}

	[RequiresUnreferencedCode("Requires .NET COM interop")]
	internal static object UiaGetReservedMixedAttributeValue()
	{
		CheckError(RawUiaGetReservedMixedAttributeValue(out object mixedAttributeValue));
		return mixedAttributeValue;
	}

	private static void CheckError(int hr)
	{
		if (hr < 0)
		{
			Marshal.ThrowExceptionForHR(hr, (IntPtr)(-1));
		}
	}

	private static bool GetIsNetComInteropAvailable()
	{
		if (!RuntimeFeature.IsDynamicCodeSupported)
		{
			return false;
		}
		if (AppContext.GetData("System.Runtime.InteropServices.BuiltInComInterop.IsSupported") is string value)
		{
			return bool.Parse(value);
		}
		return true;
	}

	[DllImport("UIAutomationCore.dll", CharSet = CharSet.Unicode, EntryPoint = "UiaLookupId")]
	private static extern int RawUiaLookupId(AutomationIdType type, ref Guid guid);

	[DllImport("UIAutomationCore.dll", CharSet = CharSet.Unicode, EntryPoint = "UiaGetReservedNotSupportedValue")]
	private static extern int RawUiaGetReservedNotSupportedValue([MarshalAs(UnmanagedType.IUnknown)] out object notSupportedValue);

	[DllImport("UIAutomationCore.dll", CharSet = CharSet.Unicode, EntryPoint = "UiaGetReservedMixedAttributeValue")]
	private static extern int RawUiaGetReservedMixedAttributeValue([MarshalAs(UnmanagedType.IUnknown)] out object mixedAttributeValue);
}
