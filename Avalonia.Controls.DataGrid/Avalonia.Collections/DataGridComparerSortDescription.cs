using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace Avalonia.Collections;

public class DataGridComparerSortDescription : DataGridSortDescription
{
	private readonly IComparer _innerComparer;

	private readonly ListSortDirection _direction;

	private readonly IComparer<object> _comparer;

	public IComparer SourceComparer => _innerComparer;

	public override IComparer<object> Comparer => _comparer;

	public override ListSortDirection Direction => _direction;

	public DataGridComparerSortDescription(IComparer comparer, ListSortDirection direction)
	{
		_innerComparer = comparer;
		_direction = direction;
		_comparer = Comparer<object>.Create((object x, object y) => Compare(x, y));
	}

	private int Compare(object x, object y)
	{
		int num = _innerComparer.Compare(x, y);
		if (Direction == ListSortDirection.Descending)
		{
			return -num;
		}
		return num;
	}

	public override DataGridSortDescription SwitchSortDirection()
	{
		ListSortDirection direction = ((_direction == ListSortDirection.Ascending) ? ListSortDirection.Descending : ListSortDirection.Ascending);
		return new DataGridComparerSortDescription(_innerComparer, direction);
	}
}
