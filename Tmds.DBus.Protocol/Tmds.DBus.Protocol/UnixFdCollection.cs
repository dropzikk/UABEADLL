using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Tmds.DBus.Protocol;

internal sealed class UnixFdCollection : IReadOnlyList<SafeHandle>, IEnumerable<SafeHandle>, IEnumerable, IReadOnlyCollection<SafeHandle>, IDisposable
{
	private readonly List<(IntPtr RawHandle, SafeHandle? Handle)>? _handles;

	private readonly List<(IntPtr RawHandle, bool OwnsHandle)>? _rawHandles;

	internal bool IsRawHandleCollection => _rawHandles != null;

	public int Count
	{
		get
		{
			if (_rawHandles == null)
			{
				return _handles.Count;
			}
			return _rawHandles.Count;
		}
	}

	public SafeHandle this[int index] => _handles[index].Handle;

	internal UnixFdCollection(bool isRawHandleCollection = true)
	{
		if (isRawHandleCollection)
		{
			_rawHandles = new List<(IntPtr, bool)>();
		}
		else
		{
			_handles = new List<(IntPtr, SafeHandle)>();
		}
	}

	internal void AddHandle(IntPtr handle)
	{
		_rawHandles.Add((handle, true));
	}

	internal void AddHandle(SafeHandle handle)
	{
		_handles.Add((handle.DangerousGetHandle(), handle));
	}

	public IntPtr DangerousGetHandle(int index)
	{
		if (_rawHandles != null)
		{
			return _rawHandles[index].RawHandle;
		}
		return _handles[index].RawHandle;
	}

	public T? RemoveHandle<T>(int index) where T : SafeHandle
	{
		if (_rawHandles != null)
		{
			(IntPtr RawHandle, bool OwnsHandle) tuple = _rawHandles[index];
			var (intPtr, _) = tuple;
			if (!tuple.OwnsHandle)
			{
				return null;
			}
			_rawHandles[index] = (intPtr, false);
			return (T)Activator.CreateInstance(typeof(T), intPtr, true);
		}
		var (item, safeHandle) = _handles[index];
		if (safeHandle == null)
		{
			return null;
		}
		if (!(safeHandle is T))
		{
			throw new ArgumentException($"Requested handle type {typeof(T).FullName} does not matched stored type {safeHandle.GetType().FullName}.");
		}
		_handles[index] = (item, null);
		return (T)safeHandle;
	}

	public IEnumerator<SafeHandle> GetEnumerator()
	{
		throw new NotSupportedException();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		throw new NotSupportedException();
	}

	public void DisposeHandles(int count = -1)
	{
		if (count != 0)
		{
			DisposeHandles(disposing: true, count);
		}
	}

	public void Dispose()
	{
		DisposeHandles(disposing: true);
	}

	~UnixFdCollection()
	{
		DisposeHandles(disposing: false);
	}

	private void DisposeHandles(bool disposing, int count = -1)
	{
		if (count == -1)
		{
			count = Count;
		}
		if (disposing && _handles != null)
		{
			for (int i = 0; i < count; i++)
			{
				_handles[i].Handle?.Dispose();
			}
			_handles.RemoveRange(0, count);
		}
		if (_rawHandles == null)
		{
			return;
		}
		for (int j = 0; j < count; j++)
		{
			(IntPtr, bool) tuple = _rawHandles[j];
			if (tuple.Item2)
			{
				close(tuple.Item1.ToInt32());
			}
		}
		_rawHandles.RemoveRange(0, count);
	}

	[DllImport("libc")]
	private static extern void close(int fd);

	internal void MoveTo(UnixFdCollection handles, int count)
	{
		if (handles.IsRawHandleCollection != IsRawHandleCollection)
		{
			throw new ArgumentException("Handle collections are not compatible.");
		}
		if (handles.IsRawHandleCollection)
		{
			for (int i = 0; i < count; i++)
			{
				handles._rawHandles.Add(_rawHandles[i]);
			}
			_rawHandles.RemoveRange(0, count);
		}
		else
		{
			for (int j = 0; j < count; j++)
			{
				handles._handles.Add(_handles[j]);
			}
			_handles.RemoveRange(0, count);
		}
	}
}
