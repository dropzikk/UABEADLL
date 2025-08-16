using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Reactive;
using Avalonia.Threading;
using Avalonia.Win32.Interop;
using Avalonia.Win32.Win32Com;
using MicroCom.Runtime;

namespace Avalonia.Win32;

internal class ClipboardImpl : IClipboard
{
	private const int OleRetryCount = 10;

	private const int OleRetryDelay = 100;

	private static async Task<IDisposable> OpenClipboard()
	{
		int i = 10;
		while (!UnmanagedMethods.OpenClipboard(IntPtr.Zero))
		{
			int num = i - 1;
			i = num;
			if (num == 0)
			{
				throw new TimeoutException("Timeout opening clipboard.");
			}
			await Task.Delay(100);
		}
		return Disposable.Create(delegate
		{
			UnmanagedMethods.CloseClipboard();
		});
	}

	public async Task<string?> GetTextAsync()
	{
		using (await OpenClipboard())
		{
			IntPtr clipboardData = UnmanagedMethods.GetClipboardData(UnmanagedMethods.ClipboardFormat.CF_UNICODETEXT);
			if (clipboardData == IntPtr.Zero)
			{
				return null;
			}
			IntPtr intPtr = UnmanagedMethods.GlobalLock(clipboardData);
			if (intPtr == IntPtr.Zero)
			{
				return null;
			}
			string? result = Marshal.PtrToStringUni(intPtr);
			UnmanagedMethods.GlobalUnlock(clipboardData);
			return result;
		}
	}

	public async Task SetTextAsync(string? text)
	{
		using (await OpenClipboard())
		{
			UnmanagedMethods.EmptyClipboard();
			if (text != null)
			{
				IntPtr hMem = Marshal.StringToHGlobalUni(text);
				UnmanagedMethods.SetClipboardData(UnmanagedMethods.ClipboardFormat.CF_UNICODETEXT, hMem);
			}
		}
	}

	public async Task ClearAsync()
	{
		using (await OpenClipboard())
		{
			UnmanagedMethods.EmptyClipboard();
		}
	}

	public async Task SetDataObjectAsync(Avalonia.Input.IDataObject data)
	{
		Dispatcher.UIThread.VerifyAccess();
		using DataObject wrapper = new DataObject(data);
		int i = 10;
		while (true)
		{
			int num = UnmanagedMethods.OleSetClipboard(((Avalonia.Win32.Win32Com.IDataObject)wrapper).GetNativeIntPtr(owned: false));
			if (num != 0)
			{
				int num2 = i - 1;
				i = num2;
				if (num2 == 0)
				{
					Marshal.ThrowExceptionForHR(num);
				}
				await Task.Delay(100);
				continue;
			}
			break;
		}
	}

	public async Task<string[]> GetFormatsAsync()
	{
		Dispatcher.UIThread.VerifyAccess();
		int i = 10;
		IntPtr dataObject;
		while (true)
		{
			int num = UnmanagedMethods.OleGetClipboard(out dataObject);
			if (num == 0)
			{
				break;
			}
			int num2 = i - 1;
			i = num2;
			if (num2 == 0)
			{
				Marshal.ThrowExceptionForHR(num);
			}
			await Task.Delay(100);
		}
		using Avalonia.Win32.Win32Com.IDataObject wrapped = MicroComRuntime.CreateProxyFor<Avalonia.Win32.Win32Com.IDataObject>(dataObject, ownsHandle: true);
		using OleDataObject oleDataObject = new OleDataObject(wrapped);
		return oleDataObject.GetDataFormats().ToArray();
	}

	public async Task<object?> GetDataAsync(string format)
	{
		Dispatcher.UIThread.VerifyAccess();
		int i = 10;
		IntPtr dataObject;
		while (true)
		{
			int num = UnmanagedMethods.OleGetClipboard(out dataObject);
			if (num == 0)
			{
				break;
			}
			int num2 = i - 1;
			i = num2;
			if (num2 == 0)
			{
				Marshal.ThrowExceptionForHR(num);
			}
			await Task.Delay(100);
		}
		using Avalonia.Win32.Win32Com.IDataObject wrapped = MicroComRuntime.CreateProxyFor<Avalonia.Win32.Win32Com.IDataObject>(dataObject, ownsHandle: true);
		using OleDataObject oleDataObject = new OleDataObject(wrapped);
		return oleDataObject.Get(format);
	}
}
