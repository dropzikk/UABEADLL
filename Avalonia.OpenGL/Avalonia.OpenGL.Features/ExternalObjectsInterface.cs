using System;
using Avalonia.SourceGenerator;

namespace Avalonia.OpenGL.Features;

internal class ExternalObjectsInterface
{
	private unsafe delegate* unmanaged[Stdcall]<uint, ulong, int, int, void> _addr_ImportMemoryFdEXT;

	private unsafe delegate* unmanaged[Stdcall]<uint, int, int, void> _addr_ImportSemaphoreFdEXT;

	private unsafe delegate* unmanaged[Stdcall]<int, out uint, void> _addr_CreateMemoryObjectsEXT;

	private unsafe delegate* unmanaged[Stdcall]<int, ref uint, void> _addr_DeleteMemoryObjectsEXT;

	private unsafe delegate* unmanaged[Stdcall]<int, int, int, int, int, uint, ulong, void> _addr_TexStorageMem2DEXT;

	private unsafe delegate* unmanaged[Stdcall]<int, out uint, void> _addr_GenSemaphoresEXT;

	private unsafe delegate* unmanaged[Stdcall]<int, ref uint, void> _addr_DeleteSemaphoresEXT;

	private unsafe delegate* unmanaged[Stdcall]<uint, uint, uint*, uint, int*, int*, void> _addr_WaitSemaphoreEXT;

	private unsafe delegate* unmanaged[Stdcall]<uint, uint, uint*, uint, int*, int*, void> _addr_SignalSemaphoreEXT;

	private unsafe delegate* unmanaged[Stdcall]<int, uint, byte*, void> _addr_GetUnsignedBytei_vEXT;

	private unsafe delegate* unmanaged[Stdcall]<int, byte*, void> _addr_GetUnsignedBytevEXT;

	public unsafe bool IsImportMemoryFdEXTAvailable => _addr_ImportMemoryFdEXT != (delegate* unmanaged[Stdcall]<uint, ulong, int, int, void>)null;

	public unsafe bool IsImportSemaphoreFdEXTAvailable => _addr_ImportSemaphoreFdEXT != (delegate* unmanaged[Stdcall]<uint, int, int, void>)null;

	public unsafe bool IsGetUnsignedBytei_vEXTAvailable => _addr_GetUnsignedBytei_vEXT != (delegate* unmanaged[Stdcall]<int, uint, byte*, void>)null;

	public unsafe bool IsGetUnsignedBytevEXTAvailable => _addr_GetUnsignedBytevEXT != (delegate* unmanaged[Stdcall]<int, byte*, void>)null;

	public ExternalObjectsInterface(Func<string, IntPtr> getProcAddress)
	{
		Initialize(getProcAddress);
	}

	[GetProcAddress("glImportMemoryFdEXT", true)]
	public unsafe void ImportMemoryFdEXT(uint memory, ulong size, int handleType, int fd)
	{
		if (_addr_ImportMemoryFdEXT == (delegate* unmanaged[Stdcall]<uint, ulong, int, int, void>)null)
		{
			throw new EntryPointNotFoundException("ImportMemoryFdEXT");
		}
		_addr_ImportMemoryFdEXT(memory, size, handleType, fd);
	}

	[GetProcAddress("glImportSemaphoreFdEXT", true)]
	public unsafe void ImportSemaphoreFdEXT(uint semaphore, int handleType, int fd)
	{
		if (_addr_ImportSemaphoreFdEXT == (delegate* unmanaged[Stdcall]<uint, int, int, void>)null)
		{
			throw new EntryPointNotFoundException("ImportSemaphoreFdEXT");
		}
		_addr_ImportSemaphoreFdEXT(semaphore, handleType, fd);
	}

	[GetProcAddress("glCreateMemoryObjectsEXT")]
	public unsafe void CreateMemoryObjectsEXT(int n, out uint memoryObjects)
	{
		_addr_CreateMemoryObjectsEXT(n, out memoryObjects);
	}

	[GetProcAddress("glDeleteMemoryObjectsEXT")]
	public unsafe void DeleteMemoryObjectsEXT(int n, ref uint objects)
	{
		_addr_DeleteMemoryObjectsEXT(n, ref objects);
	}

	[GetProcAddress("glTexStorageMem2DEXT")]
	public unsafe void TexStorageMem2DEXT(int target, int levels, int internalFormat, int width, int height, uint memory, ulong offset)
	{
		_addr_TexStorageMem2DEXT(target, levels, internalFormat, width, height, memory, offset);
	}

	[GetProcAddress("glGenSemaphoresEXT")]
	public unsafe void GenSemaphoresEXT(int n, out uint semaphores)
	{
		_addr_GenSemaphoresEXT(n, out semaphores);
	}

	[GetProcAddress("glDeleteSemaphoresEXT")]
	public unsafe void DeleteSemaphoresEXT(int n, ref uint semaphores)
	{
		_addr_DeleteSemaphoresEXT(n, ref semaphores);
	}

	[GetProcAddress("glWaitSemaphoreEXT")]
	public unsafe void WaitSemaphoreEXT(uint semaphore, uint numBufferBarriers, uint* buffers, uint numTextureBarriers, int* textures, int* srcLayouts)
	{
		_addr_WaitSemaphoreEXT(semaphore, numBufferBarriers, buffers, numTextureBarriers, textures, srcLayouts);
	}

	[GetProcAddress("glSignalSemaphoreEXT")]
	public unsafe void SignalSemaphoreEXT(uint semaphore, uint numBufferBarriers, uint* buffers, uint numTextureBarriers, int* textures, int* dstLayouts)
	{
		_addr_SignalSemaphoreEXT(semaphore, numBufferBarriers, buffers, numTextureBarriers, textures, dstLayouts);
	}

	[GetProcAddress("glGetUnsignedBytei_vEXT", true)]
	public unsafe void GetUnsignedBytei_vEXT(int target, uint index, byte* data)
	{
		if (_addr_GetUnsignedBytei_vEXT == (delegate* unmanaged[Stdcall]<int, uint, byte*, void>)null)
		{
			throw new EntryPointNotFoundException("GetUnsignedBytei_vEXT");
		}
		_addr_GetUnsignedBytei_vEXT(target, index, data);
	}

	[GetProcAddress("glGetUnsignedBytevEXT", true)]
	public unsafe void GetUnsignedBytevEXT(int target, byte* data)
	{
		if (_addr_GetUnsignedBytevEXT == (delegate* unmanaged[Stdcall]<int, byte*, void>)null)
		{
			throw new EntryPointNotFoundException("GetUnsignedBytevEXT");
		}
		_addr_GetUnsignedBytevEXT(target, data);
	}

	private unsafe void Initialize(Func<string, IntPtr> getProcAddress)
	{
		IntPtr zero = IntPtr.Zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glImportMemoryFdEXT");
		_addr_ImportMemoryFdEXT = (delegate* unmanaged[Stdcall]<uint, ulong, int, int, void>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glImportSemaphoreFdEXT");
		_addr_ImportSemaphoreFdEXT = (delegate* unmanaged[Stdcall]<uint, int, int, void>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glCreateMemoryObjectsEXT");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_CreateMemoryObjectsEXT");
		}
		_addr_CreateMemoryObjectsEXT = (delegate* unmanaged[Stdcall]<int, out uint, void>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glDeleteMemoryObjectsEXT");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_DeleteMemoryObjectsEXT");
		}
		_addr_DeleteMemoryObjectsEXT = (delegate* unmanaged[Stdcall]<int, ref uint, void>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glTexStorageMem2DEXT");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_TexStorageMem2DEXT");
		}
		_addr_TexStorageMem2DEXT = (delegate* unmanaged[Stdcall]<int, int, int, int, int, uint, ulong, void>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glGenSemaphoresEXT");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_GenSemaphoresEXT");
		}
		_addr_GenSemaphoresEXT = (delegate* unmanaged[Stdcall]<int, out uint, void>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glDeleteSemaphoresEXT");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_DeleteSemaphoresEXT");
		}
		_addr_DeleteSemaphoresEXT = (delegate* unmanaged[Stdcall]<int, ref uint, void>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glWaitSemaphoreEXT");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_WaitSemaphoreEXT");
		}
		_addr_WaitSemaphoreEXT = (delegate* unmanaged[Stdcall]<uint, uint, uint*, uint, int*, int*, void>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glSignalSemaphoreEXT");
		if (zero == IntPtr.Zero)
		{
			throw new EntryPointNotFoundException("_addr_SignalSemaphoreEXT");
		}
		_addr_SignalSemaphoreEXT = (delegate* unmanaged[Stdcall]<uint, uint, uint*, uint, int*, int*, void>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glGetUnsignedBytei_vEXT");
		_addr_GetUnsignedBytei_vEXT = (delegate* unmanaged[Stdcall]<int, uint, byte*, void>)(void*)zero;
		zero = IntPtr.Zero;
		zero = getProcAddress("glGetUnsignedBytevEXT");
		_addr_GetUnsignedBytevEXT = (delegate* unmanaged[Stdcall]<int, byte*, void>)(void*)zero;
	}
}
