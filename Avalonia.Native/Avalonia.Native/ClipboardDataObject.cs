using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Input;
using Avalonia.Native.Interop;

namespace Avalonia.Native;

internal class ClipboardDataObject : IDataObject, IDisposable
{
	private ClipboardImpl _clipboard;

	private List<string> _formats;

	private List<string> Formats => _formats ?? (_formats = _clipboard.GetFormats().ToList());

	public ClipboardDataObject(IAvnClipboard clipboard)
	{
		_clipboard = new ClipboardImpl(clipboard);
	}

	public void Dispose()
	{
		_clipboard?.Dispose();
		_clipboard = null;
	}

	public IEnumerable<string> GetDataFormats()
	{
		return Formats;
	}

	public bool Contains(string dataFormat)
	{
		return Formats.Contains(dataFormat);
	}

	public object Get(string dataFormat)
	{
		if (dataFormat == DataFormats.Text)
		{
			return _clipboard.GetTextAsync().Result;
		}
		if (dataFormat == DataFormats.Files)
		{
			return _clipboard.GetFiles();
		}
		if (dataFormat == DataFormats.FileNames)
		{
			return _clipboard.GetFileNames();
		}
		return null;
	}
}
