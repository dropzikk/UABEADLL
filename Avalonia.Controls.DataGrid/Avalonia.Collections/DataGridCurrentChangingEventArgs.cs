using System;

namespace Avalonia.Collections;

public class DataGridCurrentChangingEventArgs : EventArgs
{
	private bool _cancel;

	private bool _isCancelable;

	public bool IsCancelable => _isCancelable;

	public bool Cancel
	{
		get
		{
			return _cancel;
		}
		set
		{
			if (IsCancelable)
			{
				_cancel = value;
			}
			else if (value)
			{
				throw new InvalidOperationException("CurrentChanging Cannot Be Canceled");
			}
		}
	}

	public DataGridCurrentChangingEventArgs()
	{
		Initialize(isCancelable: true);
	}

	public DataGridCurrentChangingEventArgs(bool isCancelable)
	{
		Initialize(isCancelable);
	}

	private void Initialize(bool isCancelable)
	{
		_isCancelable = isCancelable;
	}
}
