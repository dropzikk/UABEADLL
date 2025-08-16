using System;
using System.Collections.Generic;
using Avalonia.Utilities;

namespace Avalonia.Controls;

public class NameScope : INameScope
{
	public static readonly AttachedProperty<INameScope> NameScopeProperty = AvaloniaProperty.RegisterAttached<NameScope, StyledElement, INameScope>("NameScope");

	private readonly Dictionary<string, object> _inner = new Dictionary<string, object>();

	private readonly Dictionary<string, SynchronousCompletionAsyncResultSource<object?>> _pendingSearches = new Dictionary<string, SynchronousCompletionAsyncResultSource<object>>();

	public bool IsCompleted { get; private set; }

	public static INameScope GetNameScope(StyledElement styled)
	{
		if (styled == null)
		{
			throw new ArgumentNullException("styled");
		}
		return styled.GetValue(NameScopeProperty);
	}

	public static void SetNameScope(StyledElement styled, INameScope value)
	{
		if (styled == null)
		{
			throw new ArgumentNullException("styled");
		}
		styled.SetValue(NameScopeProperty, value);
	}

	public void Register(string name, object element)
	{
		if (IsCompleted)
		{
			throw new InvalidOperationException("NameScope is completed, no further registrations are allowed");
		}
		if (name == null)
		{
			throw new ArgumentNullException("name");
		}
		if (element == null)
		{
			throw new ArgumentNullException("element");
		}
		if (_inner.TryGetValue(name, out object value))
		{
			if (value != element)
			{
				throw new ArgumentException("Control with the name '" + name + "' already registered.");
			}
			return;
		}
		_inner.Add(name, element);
		if (_pendingSearches.TryGetValue(name, out SynchronousCompletionAsyncResultSource<object> value2))
		{
			_pendingSearches.Remove(name);
			value2.SetResult(element);
		}
	}

	public SynchronousCompletionAsyncResult<object?> FindAsync(string name)
	{
		object obj = Find(name);
		if (obj != null)
		{
			return new SynchronousCompletionAsyncResult<object>(obj);
		}
		if (IsCompleted)
		{
			return new SynchronousCompletionAsyncResult<object>((object)null);
		}
		if (!_pendingSearches.TryGetValue(name, out SynchronousCompletionAsyncResultSource<object> value))
		{
			value = (_pendingSearches[name] = new SynchronousCompletionAsyncResultSource<object>());
		}
		return value.AsyncResult;
	}

	public object? Find(string name)
	{
		if (name == null)
		{
			throw new ArgumentNullException("name");
		}
		_inner.TryGetValue(name, out object value);
		return value;
	}

	public void Complete()
	{
		IsCompleted = true;
		foreach (KeyValuePair<string, SynchronousCompletionAsyncResultSource<object>> pendingSearch in _pendingSearches)
		{
			pendingSearch.Value.TrySetResult(null);
		}
		_pendingSearches.Clear();
	}
}
