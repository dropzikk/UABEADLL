using System;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Text;

namespace Tmds.DBus.Protocol;

public static class Address
{
	private struct Passwd
	{
		public IntPtr Name;

		public IntPtr Password;

		public uint UserID;

		public uint GroupID;

		public IntPtr UserInfo;

		public IntPtr HomeDir;

		public IntPtr Shell;
	}

	private static bool _systemAddressResolved;

	private static string? _systemAddress;

	private static bool _sessionAddressResolved;

	private static string? _sessionAddress;

	public static string? System
	{
		get
		{
			if (_systemAddressResolved)
			{
				return _systemAddress;
			}
			_systemAddress = Environment.GetEnvironmentVariable("DBUS_SYSTEM_BUS_ADDRESS");
			if (string.IsNullOrEmpty(_systemAddress) && !PlatformDetection.IsWindows())
			{
				_systemAddress = "unix:path=/var/run/dbus/system_bus_socket";
			}
			_systemAddressResolved = true;
			return _systemAddress;
		}
	}

	public static string? Session
	{
		get
		{
			if (_sessionAddressResolved)
			{
				return _sessionAddress;
			}
			_sessionAddress = Environment.GetEnvironmentVariable("DBUS_SESSION_BUS_ADDRESS");
			if (string.IsNullOrEmpty(_sessionAddress))
			{
				if (PlatformDetection.IsWindows())
				{
					_sessionAddress = GetSessionBusAddressFromSharedMemory();
				}
				else
				{
					_sessionAddress = GetSessionBusAddressFromX11();
				}
			}
			_sessionAddressResolved = true;
			return _sessionAddress;
		}
	}

	private unsafe static string? GetSessionBusAddressFromX11()
	{
		if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DISPLAY")))
		{
			IntPtr intPtr = XOpenDisplay(null);
			if (intPtr == IntPtr.Zero)
			{
				return null;
			}
			byte* buf = stackalloc byte[1024];
			getpwuid_r(getuid(), out var pwd, buf, 1024, out var result);
			if (result != IntPtr.Zero)
			{
				string text = Marshal.PtrToStringAnsi(pwd.Name);
				string text2 = DBusEnvironment.MachineId.Replace("-", string.Empty);
				string atom_name = "_DBUS_SESSION_BUS_SELECTION_" + text + "_" + text2;
				IntPtr intPtr2 = XInternAtom(intPtr, atom_name, only_if_exists: false);
				if (intPtr2 == IntPtr.Zero)
				{
					return null;
				}
				IntPtr intPtr3 = XGetSelectionOwner(intPtr, intPtr2);
				if (intPtr3 == IntPtr.Zero)
				{
					return null;
				}
				IntPtr intPtr4 = XInternAtom(intPtr, "_DBUS_SESSION_BUS_ADDRESS", only_if_exists: false);
				if (intPtr4 == IntPtr.Zero)
				{
					return null;
				}
				IntPtr actual_type_return;
				IntPtr actual_format_return;
				IntPtr nitems_return;
				IntPtr bytes_after_return;
				IntPtr prop_return;
				string? result2 = ((XGetWindowProperty(intPtr, intPtr3, intPtr4, 0, 1024, delete: false, (IntPtr)31, out actual_type_return, out actual_format_return, out nitems_return, out bytes_after_return, out prop_return) == 0) ? Marshal.PtrToStringAnsi(prop_return) : null);
				if (prop_return != IntPtr.Zero)
				{
					XFree(prop_return);
				}
				XCloseDisplay(intPtr);
				return result2;
			}
			return null;
		}
		return null;
	}

	private static string? GetSessionBusAddressFromSharedMemory()
	{
		string text = ReadSharedMemoryString("DBusDaemonAddressInfo", 255L);
		if (string.IsNullOrEmpty(text))
		{
			text = ReadSharedMemoryString("DBusDaemonAddressInfoDebug", 255L);
		}
		return text;
	}

	private static string? ReadSharedMemoryString(string id, long maxlen = -1L)
	{
		if (!PlatformDetection.IsWindows())
		{
			return null;
		}
		MemoryMappedFile memoryMappedFile;
		try
		{
			memoryMappedFile = MemoryMappedFile.OpenExisting(id);
		}
		catch
		{
			memoryMappedFile = null;
		}
		if (memoryMappedFile == null)
		{
			return null;
		}
		MemoryMappedViewStream memoryMappedViewStream = memoryMappedFile.CreateViewStream();
		long num = memoryMappedViewStream.Length;
		if (maxlen >= 0 && num > maxlen)
		{
			num = maxlen;
		}
		if (num == 0L)
		{
			return string.Empty;
		}
		if (num > int.MaxValue)
		{
			num = 2147483647L;
		}
		byte[] array = new byte[num];
		int num2 = memoryMappedViewStream.Read(array, 0, (int)num);
		if (num2 <= 0)
		{
			return string.Empty;
		}
		for (num2 = 0; num2 < num && array[num2] != 0; num2++)
		{
		}
		return Encoding.UTF8.GetString(array, 0, num2);
	}

	[DllImport("libc")]
	private unsafe static extern int getpwuid_r(uint uid, out Passwd pwd, byte* buf, int bufLen, out IntPtr result);

	[DllImport("libc")]
	private static extern uint getuid();

	[DllImport("libX11")]
	private static extern IntPtr XOpenDisplay(string? name);

	[DllImport("libX11")]
	private static extern int XCloseDisplay(IntPtr display);

	[DllImport("libX11")]
	private static extern IntPtr XInternAtom(IntPtr display, string atom_name, bool only_if_exists);

	[DllImport("libX11")]
	private static extern int XGetWindowProperty(IntPtr display, IntPtr w, IntPtr property, int long_offset, int long_length, bool delete, IntPtr req_type, out IntPtr actual_type_return, out IntPtr actual_format_return, out IntPtr nitems_return, out IntPtr bytes_after_return, out IntPtr prop_return);

	[DllImport("libX11")]
	private static extern int XFree(IntPtr data);

	[DllImport("libX11")]
	private static extern IntPtr XGetSelectionOwner(IntPtr display, IntPtr Atom);
}
