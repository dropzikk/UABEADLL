using System;
using System.Collections.Generic;

namespace Avalonia.Controls;

public class DataGridRowClipboardEventArgs : EventArgs
{
	private List<DataGridClipboardCellContent> _clipboardRowContent;

	private bool _isColumnHeadersRow;

	private object _item;

	public List<DataGridClipboardCellContent> ClipboardRowContent
	{
		get
		{
			if (_clipboardRowContent == null)
			{
				_clipboardRowContent = new List<DataGridClipboardCellContent>();
			}
			return _clipboardRowContent;
		}
	}

	public bool IsColumnHeadersRow => _isColumnHeadersRow;

	public object Item => _item;

	internal DataGridRowClipboardEventArgs(object item, bool isColumnHeadersRow)
	{
		_isColumnHeadersRow = isColumnHeadersRow;
		_item = item;
	}
}
