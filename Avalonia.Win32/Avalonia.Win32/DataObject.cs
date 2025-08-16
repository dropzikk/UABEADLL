using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Serialization.Formatters.Binary;
using Avalonia.Input;
using Avalonia.MicroCom;
using Avalonia.Platform.Storage;
using Avalonia.Win32.Interop;
using Avalonia.Win32.Win32Com;
using MicroCom.Runtime;

namespace Avalonia.Win32;

internal sealed class DataObject : CallbackBase, Avalonia.Input.IDataObject, Avalonia.Win32.Win32Com.IDataObject, IUnknown, IDisposable
{
	private class FormatEnumerator : CallbackBase, Avalonia.Win32.Win32Com.IEnumFORMATETC, IUnknown, IDisposable
	{
		private readonly Avalonia.Win32.Interop.FORMATETC[] _formats;

		private uint _current;

		private FormatEnumerator(Avalonia.Win32.Interop.FORMATETC[] formats, uint current)
		{
			_formats = formats;
			_current = current;
		}

		public FormatEnumerator(Avalonia.Input.IDataObject dataobj)
		{
			_formats = dataobj.GetDataFormats().Select(ConvertToFormatEtc).ToArray();
			_current = 0u;
		}

		private Avalonia.Win32.Interop.FORMATETC ConvertToFormatEtc(string aFormatName)
		{
			Avalonia.Win32.Interop.FORMATETC result = default(Avalonia.Win32.Interop.FORMATETC);
			result.cfFormat = ClipboardFormats.GetFormat(aFormatName);
			result.dwAspect = DVASPECT.DVASPECT_CONTENT;
			result.ptd = IntPtr.Zero;
			result.lindex = -1;
			result.tymed = TYMED.TYMED_HGLOBAL;
			return result;
		}

		public unsafe uint Next(uint celt, Avalonia.Win32.Interop.FORMATETC* rgelt, uint* results)
		{
			if (rgelt == null)
			{
				return 2147942487u;
			}
			uint num;
			for (num = 0u; num < celt; num++)
			{
				if (_current >= _formats.Length)
				{
					break;
				}
				rgelt[num] = _formats[_current];
				_current++;
			}
			if (num != celt)
			{
				return 1u;
			}
			if (celt != 1 || results != null)
			{
				*results = num;
			}
			return 0u;
		}

		public uint Skip(uint celt)
		{
			_current += Math.Min(celt, int.MaxValue - _current);
			if (_current >= _formats.Length)
			{
				return 1u;
			}
			return 0u;
		}

		public void Reset()
		{
			_current = 0u;
		}

		public Avalonia.Win32.Win32Com.IEnumFORMATETC Clone()
		{
			return new FormatEnumerator(_formats, _current);
		}
	}

	internal static readonly byte[] SerializedObjectGUID = new Guid("FD9EA796-3B13-4370-A679-56106BB288FB").ToByteArray();

	private const uint DV_E_TYMED = 2147745897u;

	private const uint DV_E_DVASPECT = 2147745899u;

	private const uint DV_E_FORMATETC = 2147745892u;

	private const uint OLE_E_ADVISENOTSUPPORTED = 2147745795u;

	private const uint STG_E_MEDIUMFULL = 2147680368u;

	private const int GMEM_ZEROINIT = 64;

	private const int GMEM_MOVEABLE = 2;

	private Avalonia.Input.IDataObject _wrapped;

	public DataObject(Avalonia.Input.IDataObject wrapped)
	{
		if (wrapped != null)
		{
			if (wrapped is DataObject || wrapped is OleDataObject)
			{
				throw new ArgumentException($"Cannot wrap a {wrapped.GetType()}");
			}
			_wrapped = wrapped;
			return;
		}
		throw new ArgumentNullException("wrapped");
	}

	bool Avalonia.Input.IDataObject.Contains(string dataFormat)
	{
		return _wrapped.Contains(dataFormat);
	}

	IEnumerable<string> Avalonia.Input.IDataObject.GetDataFormats()
	{
		return _wrapped.GetDataFormats();
	}

	object? Avalonia.Input.IDataObject.Get(string dataFormat)
	{
		return _wrapped.Get(dataFormat);
	}

	unsafe int Avalonia.Win32.Win32Com.IDataObject.DAdvise(Avalonia.Win32.Interop.FORMATETC* pFormatetc, int advf, void* adviseSink)
	{
		if (_wrapped is Avalonia.Win32.Win32Com.IDataObject dataObject)
		{
			return dataObject.DAdvise(pFormatetc, advf, adviseSink);
		}
		return 0;
	}

	void Avalonia.Win32.Win32Com.IDataObject.DUnadvise(int connection)
	{
		if (_wrapped is Avalonia.Win32.Win32Com.IDataObject dataObject)
		{
			dataObject.DUnadvise(connection);
		}
		throw new COMException("OLE_E_ADVISENOTSUPPORTED", -2147221501);
	}

	unsafe void* Avalonia.Win32.Win32Com.IDataObject.EnumDAdvise()
	{
		if (_wrapped is Avalonia.Win32.Win32Com.IDataObject dataObject)
		{
			return dataObject.EnumDAdvise();
		}
		return null;
	}

	Avalonia.Win32.Win32Com.IEnumFORMATETC Avalonia.Win32.Win32Com.IDataObject.EnumFormatEtc(int direction)
	{
		if (_wrapped is Avalonia.Win32.Win32Com.IDataObject dataObject)
		{
			return dataObject.EnumFormatEtc(direction);
		}
		if (direction == 1)
		{
			return new FormatEnumerator(_wrapped);
		}
		throw new COMException("E_NOTIMPL", -2147467263);
	}

	unsafe Avalonia.Win32.Interop.FORMATETC Avalonia.Win32.Win32Com.IDataObject.GetCanonicalFormatEtc(Avalonia.Win32.Interop.FORMATETC* formatIn)
	{
		if (_wrapped is Avalonia.Win32.Win32Com.IDataObject dataObject)
		{
			return dataObject.GetCanonicalFormatEtc(formatIn);
		}
		throw new COMException("E_NOTIMPL", -2147467263);
	}

	unsafe uint Avalonia.Win32.Win32Com.IDataObject.GetData(Avalonia.Win32.Interop.FORMATETC* format, Avalonia.Win32.Interop.STGMEDIUM* medium)
	{
		if (_wrapped is Avalonia.Win32.Win32Com.IDataObject dataObject)
		{
			return dataObject.GetData(format, medium);
		}
		if (!format->tymed.HasAllFlags(TYMED.TYMED_HGLOBAL))
		{
			return 2147745897u;
		}
		if (format->dwAspect != DVASPECT.DVASPECT_CONTENT)
		{
			return 2147745899u;
		}
		string format2 = ClipboardFormats.GetFormat(format->cfFormat);
		if (string.IsNullOrEmpty(format2) || !_wrapped.Contains(format2))
		{
			return 2147745892u;
		}
		*medium = default(Avalonia.Win32.Interop.STGMEDIUM);
		medium->tymed = TYMED.TYMED_HGLOBAL;
		return WriteDataToHGlobal(format2, ref medium->unionmember);
	}

	unsafe uint Avalonia.Win32.Win32Com.IDataObject.GetDataHere(Avalonia.Win32.Interop.FORMATETC* format, Avalonia.Win32.Interop.STGMEDIUM* medium)
	{
		if (_wrapped is Avalonia.Win32.Win32Com.IDataObject dataObject)
		{
			return dataObject.GetDataHere(format, medium);
		}
		if (medium->tymed != TYMED.TYMED_HGLOBAL || !format->tymed.HasAllFlags(TYMED.TYMED_HGLOBAL))
		{
			return 2147745897u;
		}
		if (format->dwAspect != DVASPECT.DVASPECT_CONTENT)
		{
			return 2147745899u;
		}
		string format2 = ClipboardFormats.GetFormat(format->cfFormat);
		if (string.IsNullOrEmpty(format2) || !_wrapped.Contains(format2))
		{
			return 2147745892u;
		}
		if (medium->unionmember == IntPtr.Zero)
		{
			return 2147680368u;
		}
		return WriteDataToHGlobal(format2, ref medium->unionmember);
	}

	unsafe uint Avalonia.Win32.Win32Com.IDataObject.QueryGetData(Avalonia.Win32.Interop.FORMATETC* format)
	{
		if (_wrapped is Avalonia.Win32.Win32Com.IDataObject dataObject)
		{
			return dataObject.QueryGetData(format);
		}
		if (format->dwAspect != DVASPECT.DVASPECT_CONTENT)
		{
			return 2147745899u;
		}
		if (!format->tymed.HasAllFlags(TYMED.TYMED_HGLOBAL))
		{
			return 2147745897u;
		}
		string format2 = ClipboardFormats.GetFormat(format->cfFormat);
		if (string.IsNullOrEmpty(format2) || !_wrapped.Contains(format2))
		{
			return 2147745892u;
		}
		return 0u;
	}

	unsafe uint Avalonia.Win32.Win32Com.IDataObject.SetData(Avalonia.Win32.Interop.FORMATETC* pformatetc, Avalonia.Win32.Interop.STGMEDIUM* pmedium, int fRelease)
	{
		if (_wrapped is Avalonia.Win32.Win32Com.IDataObject dataObject)
		{
			return dataObject.SetData(pformatetc, pmedium, fRelease);
		}
		return 2147500033u;
	}

	private uint WriteDataToHGlobal(string dataFormat, ref IntPtr hGlobal)
	{
		object obj = _wrapped.Get(dataFormat);
		if (dataFormat == DataFormats.Text || obj is string)
		{
			return WriteStringToHGlobal(ref hGlobal, Convert.ToString(obj) ?? string.Empty);
		}
		if (dataFormat == DataFormats.FileNames && obj is IEnumerable<string> files)
		{
			return WriteFileListToHGlobal(ref hGlobal, files);
		}
		if (dataFormat == DataFormats.Files && obj is IEnumerable<IStorageItem> source)
		{
			return WriteFileListToHGlobal(ref hGlobal, from f in source
				select f.TryGetLocalPath() into f
				where f != null
				select f);
		}
		if (obj is Stream stream)
		{
			int num = (int)(stream.Length - stream.Position);
			byte[] array = ArrayPool<byte>.Shared.Rent(num);
			try
			{
				stream.Read(array, 0, num);
				return WriteBytesToHGlobal(ref hGlobal, array.AsSpan(0, num));
			}
			finally
			{
				ArrayPool<byte>.Shared.Return(array);
			}
		}
		if (obj is IEnumerable<byte> enumerable)
		{
			byte[] array2 = (enumerable as byte[]) ?? enumerable.ToArray();
			return WriteBytesToHGlobal(ref hGlobal, array2);
		}
		return WriteBytesToHGlobal(ref hGlobal, SerializeObject(obj));
	}

	[UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "We still use BinaryFormatter for WinForms dragndrop compatability")]
	private static byte[] SerializeObject(object data)
	{
		using MemoryStream memoryStream = new MemoryStream();
		memoryStream.Write(SerializedObjectGUID, 0, SerializedObjectGUID.Length);
		new BinaryFormatter().Serialize(memoryStream, data);
		return memoryStream.ToArray();
	}

	private unsafe static uint WriteBytesToHGlobal(ref IntPtr hGlobal, ReadOnlySpan<byte> data)
	{
		int length = data.Length;
		if (hGlobal == IntPtr.Zero)
		{
			hGlobal = UnmanagedMethods.GlobalAlloc(66, length);
		}
		long num = UnmanagedMethods.GlobalSize(hGlobal).ToInt64();
		if (length > num)
		{
			return 2147680368u;
		}
		IntPtr intPtr = UnmanagedMethods.GlobalLock(hGlobal);
		try
		{
			data.CopyTo(new Span<byte>((void*)intPtr, data.Length));
			return 0u;
		}
		finally
		{
			UnmanagedMethods.GlobalUnlock(hGlobal);
		}
	}

	private static uint WriteFileListToHGlobal(ref IntPtr hGlobal, IEnumerable<string> files)
	{
		if (!files.Any())
		{
			return 0u;
		}
		char[] array = (string.Join("\0", files) + "\0\0").ToCharArray();
		_DROPFILES structure = default(_DROPFILES);
		structure.pFiles = Marshal.SizeOf<_DROPFILES>();
		structure.fWide = true;
		int num = array.Length * 2 + Marshal.SizeOf<_DROPFILES>();
		if (hGlobal == IntPtr.Zero)
		{
			hGlobal = UnmanagedMethods.GlobalAlloc(66, num);
		}
		long num2 = UnmanagedMethods.GlobalSize(hGlobal).ToInt64();
		if (num > num2)
		{
			return 2147680368u;
		}
		IntPtr intPtr = UnmanagedMethods.GlobalLock(hGlobal);
		try
		{
			Marshal.StructureToPtr(structure, intPtr, fDeleteOld: false);
			Marshal.Copy(array, 0, intPtr + Marshal.SizeOf<_DROPFILES>(), array.Length);
			return 0u;
		}
		finally
		{
			UnmanagedMethods.GlobalUnlock(hGlobal);
		}
	}

	private static uint WriteStringToHGlobal(ref IntPtr hGlobal, string data)
	{
		int num = (data.Length + 1) * 2;
		if (hGlobal == IntPtr.Zero)
		{
			hGlobal = UnmanagedMethods.GlobalAlloc(66, num);
		}
		long num2 = UnmanagedMethods.GlobalSize(hGlobal).ToInt64();
		if (num > num2)
		{
			return 2147680368u;
		}
		IntPtr destination = UnmanagedMethods.GlobalLock(hGlobal);
		try
		{
			char[] array = (data + "\0").ToCharArray();
			Marshal.Copy(array, 0, destination, array.Length);
			return 0u;
		}
		finally
		{
			UnmanagedMethods.GlobalUnlock(hGlobal);
		}
	}

	protected override void Destroyed()
	{
		ReleaseWrapped();
	}

	public void ReleaseWrapped()
	{
		_wrapped = null;
	}
}
