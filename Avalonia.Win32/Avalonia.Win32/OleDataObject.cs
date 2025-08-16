using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Avalonia.Input;
using Avalonia.Platform.Storage;
using Avalonia.Platform.Storage.FileIO;
using Avalonia.Utilities;
using Avalonia.Win32.Interop;
using Avalonia.Win32.Win32Com;
using MicroCom.Runtime;

namespace Avalonia.Win32;

internal class OleDataObject : Avalonia.Input.IDataObject, IDisposable
{
	private readonly Avalonia.Win32.Win32Com.IDataObject _wrapped;

	public OleDataObject(Avalonia.Win32.Win32Com.IDataObject wrapped)
	{
		_wrapped = wrapped.CloneReference();
	}

	public bool Contains(string dataFormat)
	{
		return GetDataFormatsCore().Any((string df) => StringComparer.OrdinalIgnoreCase.Equals(df, dataFormat));
	}

	public IEnumerable<string> GetDataFormats()
	{
		return GetDataFormatsCore().Distinct();
	}

	public object? Get(string dataFormat)
	{
		return GetDataFromOleHGLOBAL(dataFormat, DVASPECT.DVASPECT_CONTENT);
	}

	[UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "We still use BinaryFormatter for WinForms dragndrop compatability")]
	private unsafe object? GetDataFromOleHGLOBAL(string format, DVASPECT aspect)
	{
		Avalonia.Win32.Interop.FORMATETC fORMATETC = default(Avalonia.Win32.Interop.FORMATETC);
		fORMATETC.cfFormat = ClipboardFormats.GetFormat(format);
		fORMATETC.dwAspect = aspect;
		fORMATETC.lindex = -1;
		fORMATETC.tymed = TYMED.TYMED_HGLOBAL;
		if (_wrapped.QueryGetData(&fORMATETC) == 0)
		{
			Avalonia.Win32.Interop.STGMEDIUM medium = default(Avalonia.Win32.Interop.STGMEDIUM);
			_wrapped.GetData(&fORMATETC, &medium);
			try
			{
				if (medium.unionmember != IntPtr.Zero && medium.tymed == TYMED.TYMED_HGLOBAL)
				{
					if (format == DataFormats.Text)
					{
						return ReadStringFromHGlobal(medium.unionmember);
					}
					if (format == DataFormats.FileNames)
					{
						return ReadFileNamesFromHGlobal(medium.unionmember);
					}
					if (format == DataFormats.Files)
					{
						return from f in ReadFileNamesFromHGlobal(medium.unionmember)
							select StorageProviderHelpers.TryCreateBclStorageItem(f) into f
							where f != null
							select f;
					}
					byte[] array = ReadBytesFromHGlobal(medium.unionmember);
					if (IsSerializedObject(array))
					{
						using (MemoryStream memoryStream = new MemoryStream(array))
						{
							memoryStream.Position = DataObject.SerializedObjectGUID.Length;
							return new BinaryFormatter().Deserialize(memoryStream);
						}
					}
					return array;
				}
			}
			finally
			{
				UnmanagedMethods.ReleaseStgMedium(ref medium);
			}
		}
		return null;
	}

	private static bool IsSerializedObject(ReadOnlySpan<byte> data)
	{
		return data.StartsWith(DataObject.SerializedObjectGUID);
	}

	private static IEnumerable<string> ReadFileNamesFromHGlobal(IntPtr hGlobal)
	{
		List<string> list = new List<string>();
		int num = UnmanagedMethods.DragQueryFile(hGlobal, -1, null, 0);
		if (num > 0)
		{
			for (int i = 0; i < num; i++)
			{
				int num2 = UnmanagedMethods.DragQueryFile(hGlobal, i, null, 0);
				StringBuilder stringBuilder = StringBuilderCache.Acquire(num2 + 1);
				if (UnmanagedMethods.DragQueryFile(hGlobal, i, stringBuilder, stringBuilder.Capacity) == num2)
				{
					list.Add(StringBuilderCache.GetStringAndRelease(stringBuilder));
				}
			}
		}
		return list;
	}

	private static string? ReadStringFromHGlobal(IntPtr hGlobal)
	{
		IntPtr ptr = UnmanagedMethods.GlobalLock(hGlobal);
		try
		{
			return Marshal.PtrToStringAuto(ptr);
		}
		finally
		{
			UnmanagedMethods.GlobalUnlock(hGlobal);
		}
	}

	private static byte[] ReadBytesFromHGlobal(IntPtr hGlobal)
	{
		IntPtr source = UnmanagedMethods.GlobalLock(hGlobal);
		try
		{
			int num = (int)UnmanagedMethods.GlobalSize(hGlobal).ToInt64();
			byte[] array = new byte[num];
			Marshal.Copy(source, array, 0, num);
			return array;
		}
		finally
		{
			UnmanagedMethods.GlobalUnlock(hGlobal);
		}
	}

	private unsafe IEnumerable<string> GetDataFormatsCore()
	{
		List<string> list = new List<string>();
		Avalonia.Win32.Win32Com.IEnumFORMATETC enumFORMATETC = _wrapped.EnumFormatEtc(1);
		if (enumFORMATETC != null)
		{
			enumFORMATETC.Reset();
			Avalonia.Win32.Interop.FORMATETC[] array = ArrayPool<Avalonia.Win32.Interop.FORMATETC>.Shared.Rent(1);
			try
			{
				uint num = 0u;
				do
				{
					fixed (Avalonia.Win32.Interop.FORMATETC* rgelt = array)
					{
						if (enumFORMATETC.Next(1u, rgelt, &num) != 0)
						{
							break;
						}
					}
					if (num != 0)
					{
						if (array[0].ptd != IntPtr.Zero)
						{
							Marshal.FreeCoTaskMem(array[0].ptd);
						}
						list.Add(ClipboardFormats.GetFormat(array[0].cfFormat));
					}
				}
				while (num != 0);
			}
			finally
			{
				ArrayPool<Avalonia.Win32.Interop.FORMATETC>.Shared.Return(array);
			}
		}
		return list;
	}

	public void Dispose()
	{
		_wrapped.Dispose();
	}
}
