using System;
using System.Runtime.InteropServices;

namespace TextMateSharp.Internal.Oniguruma;

internal static class OnigInterop
{
	internal interface IOnigInterop
	{
		unsafe IntPtr onigwrap_create(char* pattern, int len, int ignoreCase, int multiline);

		IntPtr onigwrap_region_new();

		void onigwrap_region_free(IntPtr region);

		void onigwrap_free(IntPtr regex);

		unsafe int onigwrap_search(IntPtr regex, char* text, int offset, int length, IntPtr region);

		int onigwrap_num_regs(IntPtr region);

		int onigwrap_pos(IntPtr region, int nth);

		int onigwrap_len(IntPtr region, int nth);
	}

	internal class InteropWin64 : IOnigInterop
	{
		private const string ONIGWRAP = "onigwrap-x64";

		private const CharSet charSet = CharSet.Unicode;

		private const CallingConvention convention = CallingConvention.Cdecl;

		unsafe IntPtr IOnigInterop.onigwrap_create(char* pattern, int len, int ignoreCase, int multiline)
		{
			return onigwrap_create(pattern, len, ignoreCase, multiline);
		}

		void IOnigInterop.onigwrap_free(IntPtr regex)
		{
			onigwrap_free(regex);
		}

		int IOnigInterop.onigwrap_len(IntPtr region, int nth)
		{
			return onigwrap_len(region, nth);
		}

		int IOnigInterop.onigwrap_num_regs(IntPtr region)
		{
			return onigwrap_num_regs(region);
		}

		int IOnigInterop.onigwrap_pos(IntPtr region, int nth)
		{
			return onigwrap_pos(region, nth);
		}

		void IOnigInterop.onigwrap_region_free(IntPtr region)
		{
			onigwrap_region_free(region);
		}

		IntPtr IOnigInterop.onigwrap_region_new()
		{
			return onigwrap_region_new();
		}

		unsafe int IOnigInterop.onigwrap_search(IntPtr regex, char* text, int offset, int length, IntPtr region)
		{
			return onigwrap_search(regex, text, offset, length, region);
		}

		[DllImport("onigwrap-x64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private unsafe static extern IntPtr onigwrap_create(char* pattern, int len, int ignoreCase, int multiline);

		[DllImport("onigwrap-x64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private static extern IntPtr onigwrap_region_new();

		[DllImport("onigwrap-x64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private static extern void onigwrap_region_free(IntPtr region);

		[DllImport("onigwrap-x64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private static extern void onigwrap_free(IntPtr regex);

		[DllImport("onigwrap-x64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private unsafe static extern int onigwrap_search(IntPtr regex, char* text, int offset, int length, IntPtr region);

		[DllImport("onigwrap-x64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private static extern int onigwrap_num_regs(IntPtr region);

		[DllImport("onigwrap-x64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private static extern int onigwrap_pos(IntPtr region, int nth);

		[DllImport("onigwrap-x64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private static extern int onigwrap_len(IntPtr region, int nth);
	}

	internal class InteropWin32 : IOnigInterop
	{
		private const string ONIGWRAP = "onigwrap-x86";

		private const CharSet charSet = CharSet.Unicode;

		private const CallingConvention convention = CallingConvention.Cdecl;

		unsafe IntPtr IOnigInterop.onigwrap_create(char* pattern, int len, int ignoreCase, int multiline)
		{
			return onigwrap_create(pattern, len, ignoreCase, multiline);
		}

		void IOnigInterop.onigwrap_free(IntPtr regex)
		{
			onigwrap_free(regex);
		}

		int IOnigInterop.onigwrap_len(IntPtr region, int nth)
		{
			return onigwrap_len(region, nth);
		}

		int IOnigInterop.onigwrap_num_regs(IntPtr region)
		{
			return onigwrap_num_regs(region);
		}

		int IOnigInterop.onigwrap_pos(IntPtr region, int nth)
		{
			return onigwrap_pos(region, nth);
		}

		void IOnigInterop.onigwrap_region_free(IntPtr region)
		{
			onigwrap_region_free(region);
		}

		IntPtr IOnigInterop.onigwrap_region_new()
		{
			return onigwrap_region_new();
		}

		unsafe int IOnigInterop.onigwrap_search(IntPtr regex, char* text, int offset, int length, IntPtr region)
		{
			return onigwrap_search(regex, text, offset, length, region);
		}

		[DllImport("onigwrap-x86", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private unsafe static extern IntPtr onigwrap_create(char* pattern, int len, int ignoreCase, int multiline);

		[DllImport("onigwrap-x86", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private static extern IntPtr onigwrap_region_new();

		[DllImport("onigwrap-x86", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private static extern void onigwrap_region_free(IntPtr region);

		[DllImport("onigwrap-x86", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private static extern void onigwrap_free(IntPtr regex);

		[DllImport("onigwrap-x86", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private unsafe static extern int onigwrap_search(IntPtr regex, char* text, int offset, int length, IntPtr region);

		[DllImport("onigwrap-x86", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private static extern int onigwrap_num_regs(IntPtr region);

		[DllImport("onigwrap-x86", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private static extern int onigwrap_pos(IntPtr region, int nth);

		[DllImport("onigwrap-x86", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private static extern int onigwrap_len(IntPtr region, int nth);
	}

	internal class InteropUnix : IOnigInterop
	{
		private const string ONIGWRAP = "onigwrap";

		private const CharSet charSet = CharSet.Unicode;

		private const CallingConvention convention = CallingConvention.Cdecl;

		unsafe IntPtr IOnigInterop.onigwrap_create(char* pattern, int len, int ignoreCase, int multiline)
		{
			return onigwrap_create(pattern, len, ignoreCase, multiline);
		}

		void IOnigInterop.onigwrap_free(IntPtr regex)
		{
			onigwrap_free(regex);
		}

		int IOnigInterop.onigwrap_len(IntPtr region, int nth)
		{
			return onigwrap_len(region, nth);
		}

		int IOnigInterop.onigwrap_num_regs(IntPtr region)
		{
			return onigwrap_num_regs(region);
		}

		int IOnigInterop.onigwrap_pos(IntPtr region, int nth)
		{
			return onigwrap_pos(region, nth);
		}

		void IOnigInterop.onigwrap_region_free(IntPtr region)
		{
			onigwrap_region_free(region);
		}

		IntPtr IOnigInterop.onigwrap_region_new()
		{
			return onigwrap_region_new();
		}

		unsafe int IOnigInterop.onigwrap_search(IntPtr regex, char* text, int offset, int length, IntPtr region)
		{
			return onigwrap_search(regex, text, offset, length, region);
		}

		[DllImport("onigwrap", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private unsafe static extern IntPtr onigwrap_create(char* pattern, int len, int ignoreCase, int multiline);

		[DllImport("onigwrap", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private static extern IntPtr onigwrap_region_new();

		[DllImport("onigwrap", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private static extern void onigwrap_region_free(IntPtr region);

		[DllImport("onigwrap", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private static extern void onigwrap_free(IntPtr regex);

		[DllImport("onigwrap", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private unsafe static extern int onigwrap_search(IntPtr regex, char* text, int offset, int length, IntPtr region);

		[DllImport("onigwrap", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private static extern int onigwrap_num_regs(IntPtr region);

		[DllImport("onigwrap", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private static extern int onigwrap_pos(IntPtr region, int nth);

		[DllImport("onigwrap", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private static extern int onigwrap_len(IntPtr region, int nth);
	}

	internal static IOnigInterop Instance { get; private set; }

	static OnigInterop()
	{
		Instance = CreateInterop();
	}

	private static IOnigInterop CreateInterop()
	{
		if (!IsWindowsPlatform())
		{
			return new InteropUnix();
		}
		if (Environment.Is64BitProcess)
		{
			return new InteropWin64();
		}
		return new InteropWin32();
	}

	private static bool IsWindowsPlatform()
	{
		PlatformID platform = Environment.OSVersion.Platform;
		if ((uint)platform <= 2u)
		{
			return true;
		}
		return false;
	}
}
