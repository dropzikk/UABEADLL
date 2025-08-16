using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Native.Interop;
using Avalonia.Platform.Storage;
using Avalonia.Platform.Storage.FileIO;

namespace Avalonia.Native;

internal class ClipboardImpl : IClipboard, IDisposable
{
	private IAvnClipboard _native;

	private const string NSPasteboardTypeString = "public.utf8-plain-text";

	private const string NSFilenamesPboardType = "NSFilenamesPboardType";

	public ClipboardImpl(IAvnClipboard native)
	{
		_native = native;
	}

	public Task ClearAsync()
	{
		_native.Clear();
		return Task.CompletedTask;
	}

	public Task<string> GetTextAsync()
	{
		using IAvnString avnString = _native.GetText("public.utf8-plain-text");
		return Task.FromResult(avnString.String);
	}

	public Task SetTextAsync(string text)
	{
		_native.Clear();
		if (text != null)
		{
			_native.SetText("public.utf8-plain-text", text);
		}
		return Task.CompletedTask;
	}

	public IEnumerable<string> GetFormats()
	{
		List<string> list = new List<string>();
		using IAvnStringArray avnStringArray = _native.ObtainFormats();
		uint count = avnStringArray.Count;
		for (uint num = 0u; num < count; num++)
		{
			using IAvnString avnString = avnStringArray.Get(num);
			if (avnString.String == "public.utf8-plain-text")
			{
				list.Add(DataFormats.Text);
			}
			if (avnString.String == "NSFilenamesPboardType")
			{
				list.Add(DataFormats.FileNames);
				list.Add(DataFormats.Files);
			}
		}
		return list;
	}

	public void Dispose()
	{
		_native?.Dispose();
		_native = null;
	}

	public IEnumerable<string> GetFileNames()
	{
		using IAvnStringArray avnStringArray = _native.GetStrings("NSFilenamesPboardType");
		return avnStringArray?.ToStringArray();
	}

	public IEnumerable<IStorageItem> GetFiles()
	{
		return (from f in GetFileNames()?.Select((string f) => StorageProviderHelpers.TryCreateBclStorageItem(f))
			where f != null
			select f);
	}

	public unsafe Task SetDataObjectAsync(IDataObject data)
	{
		_native.Clear();
		foreach (string dataFormat in data.GetDataFormats())
		{
			object obj = data.Get(dataFormat);
			if (obj is string utf8Text)
			{
				_native.SetText(dataFormat, utf8Text);
			}
			else
			{
				if (!(obj is byte[] array))
				{
					continue;
				}
				fixed (byte* utf8Text2 = array)
				{
					_native.SetBytes(dataFormat, utf8Text2, array.Length);
				}
			}
		}
		return Task.CompletedTask;
	}

	public Task<string[]> GetFormatsAsync()
	{
		using IAvnStringArray avnStringArray = _native.ObtainFormats();
		return Task.FromResult(avnStringArray.ToStringArray());
	}

	public async Task<object> GetDataAsync(string format)
	{
		if (format == DataFormats.Text)
		{
			return await GetTextAsync();
		}
		if (format == DataFormats.FileNames)
		{
			return GetFileNames();
		}
		if (format == DataFormats.Files)
		{
			return GetFiles();
		}
		using IAvnString avnString = _native.GetBytes(format);
		return avnString.Bytes;
	}
}
