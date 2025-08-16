using System;
using System.Runtime.CompilerServices;
using System.Text;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnSystemDialogsProxy : MicroComProxyBase, IAvnSystemDialogs, IUnknown, IDisposable
{
	protected override int VTableSize => base.VTableSize + 3;

	public unsafe void SelectFolderDialog(IAvnWindow parentWindowHandle, IAvnSystemDialogEvents events, int allowMultiple, string title, string initialPath)
	{
		byte[] array = new byte[Encoding.UTF8.GetByteCount(title) + 1];
		Encoding.UTF8.GetBytes(title, 0, title.Length, array, 0);
		byte[] array2 = new byte[Encoding.UTF8.GetByteCount(initialPath) + 1];
		Encoding.UTF8.GetBytes(initialPath, 0, initialPath.Length, array2, 0);
		fixed (byte* ptr2 = array2)
		{
			fixed (byte* ptr = array)
			{
				((delegate* unmanaged[Stdcall]<void*, void*, void*, int, void*, void*, void>)(*base.PPV)[base.VTableSize])(base.PPV, MicroComRuntime.GetNativePointer(parentWindowHandle), MicroComRuntime.GetNativePointer(events), allowMultiple, ptr, ptr2);
			}
		}
	}

	public unsafe void OpenFileDialog(IAvnWindow parentWindowHandle, IAvnSystemDialogEvents events, int allowMultiple, string title, string initialDirectory, string initialFile, string filters)
	{
		byte[] array = new byte[Encoding.UTF8.GetByteCount(title) + 1];
		Encoding.UTF8.GetBytes(title, 0, title.Length, array, 0);
		byte[] array2 = new byte[Encoding.UTF8.GetByteCount(initialDirectory) + 1];
		Encoding.UTF8.GetBytes(initialDirectory, 0, initialDirectory.Length, array2, 0);
		byte[] array3 = new byte[Encoding.UTF8.GetByteCount(initialFile) + 1];
		Encoding.UTF8.GetBytes(initialFile, 0, initialFile.Length, array3, 0);
		byte[] array4 = new byte[Encoding.UTF8.GetByteCount(filters) + 1];
		Encoding.UTF8.GetBytes(filters, 0, filters.Length, array4, 0);
		fixed (byte* ptr4 = array4)
		{
			fixed (byte* ptr3 = array3)
			{
				fixed (byte* ptr2 = array2)
				{
					fixed (byte* ptr = array)
					{
						((delegate* unmanaged[Stdcall]<void*, void*, void*, int, void*, void*, void*, void*, void>)(*base.PPV)[base.VTableSize + 1])(base.PPV, MicroComRuntime.GetNativePointer(parentWindowHandle), MicroComRuntime.GetNativePointer(events), allowMultiple, ptr, ptr2, ptr3, ptr4);
					}
				}
			}
		}
	}

	public unsafe void SaveFileDialog(IAvnWindow parentWindowHandle, IAvnSystemDialogEvents events, string title, string initialDirectory, string initialFile, string filters)
	{
		byte[] array = new byte[Encoding.UTF8.GetByteCount(title) + 1];
		Encoding.UTF8.GetBytes(title, 0, title.Length, array, 0);
		byte[] array2 = new byte[Encoding.UTF8.GetByteCount(initialDirectory) + 1];
		Encoding.UTF8.GetBytes(initialDirectory, 0, initialDirectory.Length, array2, 0);
		byte[] array3 = new byte[Encoding.UTF8.GetByteCount(initialFile) + 1];
		Encoding.UTF8.GetBytes(initialFile, 0, initialFile.Length, array3, 0);
		byte[] array4 = new byte[Encoding.UTF8.GetByteCount(filters) + 1];
		Encoding.UTF8.GetBytes(filters, 0, filters.Length, array4, 0);
		fixed (byte* ptr4 = array4)
		{
			fixed (byte* ptr3 = array3)
			{
				fixed (byte* ptr2 = array2)
				{
					fixed (byte* ptr = array)
					{
						((delegate* unmanaged[Stdcall]<void*, void*, void*, void*, void*, void*, void*, void>)(*base.PPV)[base.VTableSize + 2])(base.PPV, MicroComRuntime.GetNativePointer(parentWindowHandle), MicroComRuntime.GetNativePointer(events), ptr, ptr2, ptr3, ptr4);
					}
				}
			}
		}
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IAvnSystemDialogs), new Guid("4d7a47db-a944-4061-abe7-62cb6aa0ffd5"), (IntPtr p, bool owns) => new __MicroComIAvnSystemDialogsProxy(p, owns));
	}

	protected __MicroComIAvnSystemDialogsProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
