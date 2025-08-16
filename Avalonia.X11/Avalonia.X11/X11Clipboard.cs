using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Input;
using Avalonia.Input.Platform;

namespace Avalonia.X11;

internal class X11Clipboard : IClipboard
{
	private readonly X11Info _x11;

	private IDataObject _storedDataObject;

	private IntPtr _handle;

	private TaskCompletionSource<bool> _storeAtomTcs;

	private TaskCompletionSource<IntPtr[]> _requestedFormatsTcs;

	private TaskCompletionSource<object> _requestedDataTcs;

	private readonly IntPtr[] _textAtoms;

	private readonly IntPtr _avaloniaSaveTargetsAtom;

	private bool HasOwner => XLib.XGetSelectionOwner(_x11.Display, _x11.Atoms.CLIPBOARD) != IntPtr.Zero;

	public X11Clipboard(AvaloniaX11Platform platform)
	{
		_x11 = platform.Info;
		_handle = XLib.CreateEventWindow(platform, OnEvent);
		_avaloniaSaveTargetsAtom = XLib.XInternAtom(_x11.Display, "AVALONIA_SAVE_TARGETS_PROPERTY_ATOM", only_if_exists: false);
		_textAtoms = new IntPtr[4]
		{
			_x11.Atoms.XA_STRING,
			_x11.Atoms.OEMTEXT,
			_x11.Atoms.UTF8_STRING,
			_x11.Atoms.UTF16_STRING
		}.Where((IntPtr a) => a != IntPtr.Zero).ToArray();
	}

	private bool IsStringAtom(IntPtr atom)
	{
		return _textAtoms.Contains(atom);
	}

	private Encoding GetStringEncoding(IntPtr atom)
	{
		if (!(atom == _x11.Atoms.XA_STRING) && !(atom == _x11.Atoms.OEMTEXT))
		{
			if (!(atom == _x11.Atoms.UTF8_STRING))
			{
				if (!(atom == _x11.Atoms.UTF16_STRING))
				{
					return null;
				}
				return Encoding.Unicode;
			}
			return Encoding.UTF8;
		}
		return Encoding.ASCII;
	}

	private unsafe void OnEvent(ref XEvent ev)
	{
		if (ev.type == XEventName.SelectionClear)
		{
			_storeAtomTcs?.TrySetResult(result: true);
			return;
		}
		if (ev.type == XEventName.SelectionRequest)
		{
			XSelectionRequestEvent selectionRequestEvent = ev.SelectionRequestEvent;
			XEvent xEvent = default(XEvent);
			xEvent.SelectionEvent.type = XEventName.SelectionNotify;
			xEvent.SelectionEvent.send_event = 1;
			xEvent.SelectionEvent.display = _x11.Display;
			xEvent.SelectionEvent.selection = selectionRequestEvent.selection;
			xEvent.SelectionEvent.target = selectionRequestEvent.target;
			xEvent.SelectionEvent.requestor = selectionRequestEvent.requestor;
			xEvent.SelectionEvent.time = selectionRequestEvent.time;
			xEvent.SelectionEvent.property = IntPtr.Zero;
			XEvent send_event = xEvent;
			if (selectionRequestEvent.selection == _x11.Atoms.CLIPBOARD)
			{
				send_event.SelectionEvent.property = WriteTargetToProperty(selectionRequestEvent.target, selectionRequestEvent.requestor, selectionRequestEvent.property);
			}
			XLib.XSendEvent(_x11.Display, selectionRequestEvent.requestor, propagate: false, new IntPtr(0), ref send_event);
		}
		if (ev.type != XEventName.SelectionNotify || !(ev.SelectionEvent.selection == _x11.Atoms.CLIPBOARD))
		{
			return;
		}
		XSelectionEvent selectionEvent = ev.SelectionEvent;
		if (selectionEvent.property == IntPtr.Zero)
		{
			_requestedFormatsTcs?.TrySetResult(null);
			_requestedDataTcs?.TrySetResult(null);
		}
		XLib.XGetWindowProperty(_x11.Display, _handle, selectionEvent.property, IntPtr.Zero, new IntPtr(int.MaxValue), delete: true, (IntPtr)0, out var actual_type, out var actual_format, out var nitems, out var _, out var prop);
		Encoding encoding = null;
		if (nitems == IntPtr.Zero)
		{
			_requestedFormatsTcs?.TrySetResult(null);
			_requestedDataTcs?.TrySetResult(null);
		}
		else if (selectionEvent.property == _x11.Atoms.TARGETS)
		{
			if (actual_format != 32)
			{
				_requestedFormatsTcs?.TrySetResult(null);
			}
			else
			{
				IntPtr[] array = new IntPtr[nitems.ToInt32()];
				Marshal.Copy(prop, array, 0, array.Length);
				_requestedFormatsTcs?.TrySetResult(array);
			}
		}
		else if ((encoding = GetStringEncoding(actual_type)) != null)
		{
			string @string = encoding.GetString((byte*)prop.ToPointer(), nitems.ToInt32());
			_requestedDataTcs?.TrySetResult(@string);
		}
		else if (actual_type == _x11.Atoms.INCR)
		{
			_requestedDataTcs.TrySetResult(null);
		}
		else
		{
			byte[] array2 = new byte[(int)nitems * (actual_format / 8)];
			Marshal.Copy(prop, array2, 0, array2.Length);
			_requestedDataTcs?.TrySetResult(array2);
		}
		XLib.XFree(prop);
		unsafe IntPtr WriteTargetToProperty(IntPtr target, IntPtr window, IntPtr property)
		{
			if (target == _x11.Atoms.TARGETS)
			{
				IntPtr[] array3 = ConvertDataObject(_storedDataObject);
				XLib.XChangeProperty(_x11.Display, window, property, _x11.Atoms.XA_ATOM, 32, PropertyMode.Replace, array3, array3.Length);
				return property;
			}
			if (target == _x11.Atoms.SAVE_TARGETS && _x11.Atoms.SAVE_TARGETS != IntPtr.Zero)
			{
				return property;
			}
			Encoding stringEncoding;
			if ((stringEncoding = GetStringEncoding(target)) != null)
			{
				IDataObject storedDataObject = _storedDataObject;
				if (storedDataObject != null && storedDataObject.Contains(DataFormats.Text))
				{
					string text = _storedDataObject.GetText();
					if (text == null)
					{
						return IntPtr.Zero;
					}
					byte[] bytes = stringEncoding.GetBytes(text);
					fixed (byte* ptr = bytes)
					{
						void* data = ptr;
						XLib.XChangeProperty(_x11.Display, window, property, target, 8, PropertyMode.Replace, data, bytes.Length);
					}
					return property;
				}
			}
			if (target == _x11.Atoms.MULTIPLE && _x11.Atoms.MULTIPLE != IntPtr.Zero)
			{
				XLib.XGetWindowProperty(_x11.Display, window, property, IntPtr.Zero, new IntPtr(int.MaxValue), delete: false, _x11.Atoms.ATOM_PAIR, out var _, out var actual_format2, out var nitems2, out var _, out var prop2);
				if (nitems2 == IntPtr.Zero)
				{
					return IntPtr.Zero;
				}
				if (actual_format2 == 32)
				{
					IntPtr* ptr2 = (IntPtr*)prop2.ToPointer();
					for (int i = 0; i < nitems2.ToInt32(); i += 2)
					{
						IntPtr target2 = ptr2[i];
						IntPtr property2 = ptr2[i + 1];
						IntPtr intPtr = WriteTargetToProperty(target2, window, property2);
						ptr2[i + 1] = intPtr;
					}
					XLib.XChangeProperty(_x11.Display, window, property, _x11.Atoms.ATOM_PAIR, 32, PropertyMode.Replace, prop2.ToPointer(), nitems2.ToInt32());
				}
				XLib.XFree(prop2);
				return property;
			}
			IDataObject storedDataObject2 = _storedDataObject;
			if (storedDataObject2 != null && storedDataObject2.Contains(_x11.Atoms.GetAtomName(target)))
			{
				object obj = _storedDataObject.Get(_x11.Atoms.GetAtomName(target));
				byte[] array4 = obj as byte[];
				if (array4 == null)
				{
					if (!(obj is string s))
					{
						return IntPtr.Zero;
					}
					array4 = Encoding.UTF8.GetBytes(s);
				}
				XLib.XChangeProperty(_x11.Display, window, property, target, 8, PropertyMode.Replace, array4, array4.Length);
				return property;
			}
			return IntPtr.Zero;
		}
	}

	private Task<IntPtr[]> SendFormatRequest()
	{
		if (_requestedFormatsTcs == null || _requestedFormatsTcs.Task.IsCompleted)
		{
			_requestedFormatsTcs = new TaskCompletionSource<IntPtr[]>();
		}
		XLib.XConvertSelection(_x11.Display, _x11.Atoms.CLIPBOARD, _x11.Atoms.TARGETS, _x11.Atoms.TARGETS, _handle, IntPtr.Zero);
		return _requestedFormatsTcs.Task;
	}

	private Task<object> SendDataRequest(IntPtr format)
	{
		if (_requestedDataTcs == null || _requestedDataTcs.Task.IsCompleted)
		{
			_requestedDataTcs = new TaskCompletionSource<object>();
		}
		XLib.XConvertSelection(_x11.Display, _x11.Atoms.CLIPBOARD, format, format, _handle, IntPtr.Zero);
		return _requestedDataTcs.Task;
	}

	public async Task<string> GetTextAsync()
	{
		if (!HasOwner)
		{
			return null;
		}
		IntPtr[] array = await SendFormatRequest();
		IntPtr format = _x11.Atoms.UTF8_STRING;
		if (array != null)
		{
			IntPtr[] array2 = new IntPtr[3]
			{
				_x11.Atoms.UTF16_STRING,
				_x11.Atoms.UTF8_STRING,
				_x11.Atoms.XA_STRING
			};
			foreach (IntPtr intPtr in array2)
			{
				if (array.Contains(intPtr))
				{
					format = intPtr;
					break;
				}
			}
		}
		return (string)(await SendDataRequest(format));
	}

	private IntPtr[] ConvertDataObject(IDataObject data)
	{
		HashSet<IntPtr> hashSet = new HashSet<IntPtr>
		{
			_x11.Atoms.TARGETS,
			_x11.Atoms.MULTIPLE
		};
		foreach (string dataFormat in data.GetDataFormats())
		{
			if (dataFormat == DataFormats.Text)
			{
				IntPtr[] textAtoms = _textAtoms;
				foreach (IntPtr item in textAtoms)
				{
					hashSet.Add(item);
				}
			}
			else
			{
				hashSet.Add(_x11.Atoms.GetAtom(dataFormat));
			}
		}
		return hashSet.ToArray();
	}

	private Task StoreAtomsInClipboardManager(IDataObject data)
	{
		if (_x11.Atoms.CLIPBOARD_MANAGER != IntPtr.Zero && _x11.Atoms.SAVE_TARGETS != IntPtr.Zero && XLib.XGetSelectionOwner(_x11.Display, _x11.Atoms.CLIPBOARD_MANAGER) != IntPtr.Zero)
		{
			if (_storeAtomTcs == null || _storeAtomTcs.Task.IsCompleted)
			{
				_storeAtomTcs = new TaskCompletionSource<bool>();
			}
			IntPtr[] array = ConvertDataObject(data);
			XLib.XChangeProperty(_x11.Display, _handle, _avaloniaSaveTargetsAtom, _x11.Atoms.XA_ATOM, 32, PropertyMode.Replace, array, array.Length);
			XLib.XConvertSelection(_x11.Display, _x11.Atoms.CLIPBOARD_MANAGER, _x11.Atoms.SAVE_TARGETS, _avaloniaSaveTargetsAtom, _handle, IntPtr.Zero);
			return _storeAtomTcs.Task;
		}
		return Task.CompletedTask;
	}

	public Task SetTextAsync(string text)
	{
		DataObject dataObject = new DataObject();
		dataObject.Set(DataFormats.Text, text);
		return SetDataObjectAsync(dataObject);
	}

	public Task ClearAsync()
	{
		return SetTextAsync(null);
	}

	public Task SetDataObjectAsync(IDataObject data)
	{
		_storedDataObject = data;
		XLib.XSetSelectionOwner(_x11.Display, _x11.Atoms.CLIPBOARD, _handle, IntPtr.Zero);
		return StoreAtomsInClipboardManager(data);
	}

	public async Task<string[]> GetFormatsAsync()
	{
		if (!HasOwner)
		{
			return null;
		}
		IntPtr[] array = await SendFormatRequest();
		if (array == null)
		{
			return null;
		}
		List<string> list = new List<string>();
		if (_textAtoms.Any(((IEnumerable<IntPtr>)array).Contains<IntPtr>))
		{
			list.Add(DataFormats.Text);
		}
		IntPtr[] array2 = array;
		foreach (IntPtr atom in array2)
		{
			list.Add(_x11.Atoms.GetAtomName(atom));
		}
		return list.ToArray();
	}

	public async Task<object> GetDataAsync(string format)
	{
		if (!HasOwner)
		{
			return null;
		}
		if (format == DataFormats.Text)
		{
			return await GetTextAsync();
		}
		IntPtr formatAtom = _x11.Atoms.GetAtom(format);
		if (!(await SendFormatRequest()).Contains(formatAtom))
		{
			return null;
		}
		return await SendDataRequest(formatAtom);
	}
}
