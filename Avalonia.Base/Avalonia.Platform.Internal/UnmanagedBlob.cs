using System;
using System.Runtime.InteropServices;

namespace Avalonia.Platform.Internal;

internal class UnmanagedBlob
{
	private IntPtr _address;

	private readonly object _lock = new object();

	private bool? _useMmap;

	public IntPtr Address
	{
		get
		{
			if (!IsDisposed)
			{
				return _address;
			}
			throw new ObjectDisposedException("UnmanagedBlob");
		}
	}

	public int Size { get; private set; }

	public bool IsDisposed { get; private set; }

	private bool UseMmap
	{
		get
		{
			bool? useMmap = _useMmap;
			if (!useMmap.HasValue)
			{
				bool? flag = (_useMmap = RuntimeInformation.IsOSPlatform(OSPlatform.Linux));
				return flag.Value;
			}
			return useMmap == true;
		}
	}

	public UnmanagedBlob(int size)
	{
		try
		{
			if (size <= 0)
			{
				throw new ArgumentException("Positive number required", "size");
			}
			_address = Alloc(size);
			GC.AddMemoryPressure(size);
			Size = size;
		}
		catch
		{
			GC.SuppressFinalize(this);
			throw;
		}
	}

	private void DoDispose()
	{
		lock (_lock)
		{
			if (!IsDisposed)
			{
				Free(_address, Size);
				GC.RemoveMemoryPressure(Size);
				IsDisposed = true;
				_address = IntPtr.Zero;
				Size = 0;
			}
		}
	}

	public void Dispose()
	{
		DoDispose();
		GC.SuppressFinalize(this);
	}

	~UnmanagedBlob()
	{
		DoDispose();
	}

	[DllImport("libc", SetLastError = true)]
	private static extern IntPtr mmap(IntPtr addr, IntPtr length, int prot, int flags, int fd, IntPtr offset);

	[DllImport("libc", SetLastError = true)]
	private static extern int munmap(IntPtr addr, IntPtr length);

	[DllImport("libc", SetLastError = true)]
	private static extern long sysconf(int name);

	private IntPtr Alloc(int size)
	{
		if (!UseMmap)
		{
			return Marshal.AllocHGlobal(size);
		}
		IntPtr result = mmap(IntPtr.Zero, new IntPtr(size), 3, 34, -1, IntPtr.Zero);
		if (result.ToInt64() == -1 || result.ToInt64() == uint.MaxValue)
		{
			throw new Exception("Unable to allocate memory: " + Marshal.GetLastWin32Error());
		}
		return result;
	}

	private void Free(IntPtr ptr, int len)
	{
		if (!UseMmap)
		{
			Marshal.FreeHGlobal(ptr);
		}
		else if (munmap(ptr, new IntPtr(len)) == -1)
		{
			throw new Exception("Unable to free memory: " + Marshal.GetLastWin32Error());
		}
	}
}
