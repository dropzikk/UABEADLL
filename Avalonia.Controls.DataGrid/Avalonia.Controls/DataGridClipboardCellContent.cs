namespace Avalonia.Controls;

public struct DataGridClipboardCellContent
{
	private DataGridColumn _column;

	private object _content;

	private object _item;

	public DataGridColumn Column => _column;

	public object Content => _content;

	public object Item => _item;

	public DataGridClipboardCellContent(object item, DataGridColumn column, object content)
	{
		_item = item;
		_column = column;
		_content = content;
	}

	public override bool Equals(object obj)
	{
		if (obj is DataGridClipboardCellContent dataGridClipboardCellContent)
		{
			if (_column == dataGridClipboardCellContent._column && _content == dataGridClipboardCellContent._content)
			{
				return _item == dataGridClipboardCellContent._item;
			}
			return false;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return _column.GetHashCode() ^ _content.GetHashCode() ^ _item.GetHashCode();
	}

	public static bool operator ==(DataGridClipboardCellContent clipboardCellContent1, DataGridClipboardCellContent clipboardCellContent2)
	{
		if (clipboardCellContent1._column == clipboardCellContent2._column && clipboardCellContent1._content == clipboardCellContent2._content)
		{
			return clipboardCellContent1._item == clipboardCellContent2._item;
		}
		return false;
	}

	public static bool operator !=(DataGridClipboardCellContent clipboardCellContent1, DataGridClipboardCellContent clipboardCellContent2)
	{
		if (clipboardCellContent1._column == clipboardCellContent2._column && clipboardCellContent1._content == clipboardCellContent2._content)
		{
			return clipboardCellContent1._item != clipboardCellContent2._item;
		}
		return true;
	}
}
