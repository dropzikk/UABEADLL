using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Avalonia.Media.TextFormatting;

internal sealed class FormattingObjectPool
{
	internal sealed class ListPool<T>
	{
		private const int MaxSize = 16;

		private readonly RentedList<T>[] _lists = new RentedList<T>[16];

		private int _size;

		private int _pendingReturnCount;

		public RentedList<T> Rent()
		{
			RentedList<T> result = ((_size > 0) ? _lists[--_size] : new RentedList<T>());
			_pendingReturnCount++;
			return result;
		}

		public void Return(ref RentedList<T>? rentedList)
		{
			if (rentedList != null)
			{
				_pendingReturnCount--;
				FormattingBufferHelper.ClearThenResetIfTooLarge(rentedList);
				if (_size < 16)
				{
					_lists[_size++] = rentedList;
				}
				rentedList = null;
			}
		}

		[Conditional("DEBUG")]
		public void VerifyAllReturned()
		{
			int pendingReturnCount = _pendingReturnCount;
			_pendingReturnCount = 0;
			if (pendingReturnCount > 0)
			{
				throw new InvalidOperationException($"{pendingReturnCount} RentedList<{typeof(T).Name}> haven't been returned to the pool!");
			}
			if (pendingReturnCount < 0)
			{
				throw new InvalidOperationException($"{-pendingReturnCount} RentedList<{typeof(T).Name}> extra lists have been returned to the pool!");
			}
		}
	}

	internal sealed class RentedList<T> : List<T>
	{
	}

	[ThreadStatic]
	private static FormattingObjectPool? t_instance;

	public static FormattingObjectPool Instance => t_instance ?? (t_instance = new FormattingObjectPool());

	public ListPool<TextRun> TextRunLists { get; } = new ListPool<TextRun>();

	public ListPool<UnshapedTextRun> UnshapedTextRunLists { get; } = new ListPool<UnshapedTextRun>();

	public ListPool<TextLine> TextLines { get; } = new ListPool<TextLine>();

	[Conditional("DEBUG")]
	public void VerifyAllReturned()
	{
	}
}
