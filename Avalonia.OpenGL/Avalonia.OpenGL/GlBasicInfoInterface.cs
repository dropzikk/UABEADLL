using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Avalonia.SourceGenerator;

namespace Avalonia.OpenGL;

public class GlBasicInfoInterface
{
	private unsafe delegate* unmanaged[Stdcall]<int, out int, void> _addr_GetIntegerv;

	private unsafe delegate* unmanaged[Stdcall]<int, IntPtr> _addr_GetStringNative;

	private unsafe delegate* unmanaged[Stdcall]<int, int, IntPtr> _addr_GetStringiNative;

	public GlBasicInfoInterface(Func<string, IntPtr> getProcAddress)
	{
		Initialize(getProcAddress);
	}

	[GetProcAddress("glGetIntegerv")]
	public unsafe void GetIntegerv(int name, out int rv)
	{
		_addr_GetIntegerv(name, out rv);
	}

	[GetProcAddress("glGetString")]
	public unsafe IntPtr GetStringNative(int v)
	{
		return _addr_GetStringNative(v);
	}

	[GetProcAddress("glGetStringi")]
	public unsafe IntPtr GetStringiNative(int v, int v1)
	{
		return _addr_GetStringiNative(v, v1);
	}

	public string? GetString(int v)
	{
		IntPtr stringNative = GetStringNative(v);
		if (stringNative != IntPtr.Zero)
		{
			return Marshal.PtrToStringAnsi(stringNative);
		}
		return null;
	}

	public string? GetString(int v, int index)
	{
		IntPtr stringiNative = GetStringiNative(v, index);
		if (stringiNative != IntPtr.Zero)
		{
			return Marshal.PtrToStringAnsi(stringiNative);
		}
		return null;
	}

	public List<string> GetExtensions()
	{
		string @string = GetString(7939);
		if (@string != null)
		{
			return @string.Split(' ').ToList();
		}
		GetIntegerv(33309, out var rv);
		List<string> list = new List<string>(rv);
		for (int i = 0; i < rv; i++)
		{
			string string2 = GetString(7939, i);
			if (string2 != null)
			{
				list.Add(string2);
			}
		}
		return list;
	}

	private unsafe void Initialize(Func<string, IntPtr> getProcAddress)
	{
		IntPtr zero = IntPtr.Zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glGetIntegerv");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_GetIntegerv");
		}
		_addr_GetIntegerv = (delegate* unmanaged[Stdcall]<int, out int, void>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glGetString");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_GetStringNative");
		}
		_addr_GetStringNative = (delegate* unmanaged[Stdcall]<int, IntPtr>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glGetStringi");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_GetStringiNative");
		}
		_addr_GetStringiNative = (delegate* unmanaged[Stdcall]<int, int, IntPtr>)(void*)zero;
	}
}
