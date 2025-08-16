using System;
using System.Collections.Generic;
using System.Linq;

namespace AvaloniaEdit.Utils;

internal static class BusyManager
{
	public struct BusyLock : IDisposable
	{
		public static readonly BusyLock Failed = new BusyLock(null);

		private readonly List<object> _objectList;

		public bool Success => _objectList != null;

		internal BusyLock(List<object> objectList)
		{
			_objectList = objectList;
		}

		public void Dispose()
		{
			_objectList?.RemoveAt(_objectList.Count - 1);
		}
	}

	[ThreadStatic]
	private static List<object> _activeObjects;

	public static BusyLock Enter(object obj)
	{
		List<object> list = _activeObjects ?? (_activeObjects = new List<object>());
		if (list.Any((object t) => t == obj))
		{
			return BusyLock.Failed;
		}
		list.Add(obj);
		return new BusyLock(list);
	}
}
