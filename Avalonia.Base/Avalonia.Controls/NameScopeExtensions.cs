using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.LogicalTree;

namespace Avalonia.Controls;

public static class NameScopeExtensions
{
	public static T? Find<T>(this INameScope nameScope, string name) where T : class
	{
		if (nameScope == null)
		{
			throw new ArgumentNullException("nameScope");
		}
		if (name == null)
		{
			throw new ArgumentNullException("name");
		}
		object obj = nameScope.Find(name);
		if (obj == null)
		{
			return null;
		}
		if (obj is T result)
		{
			return result;
		}
		throw new InvalidOperationException($"Expected control '{name}' to be '{typeof(T)} but it was '{obj.GetType()}'.");
	}

	public static T? Find<T>(this ILogical anchor, string name) where T : class
	{
		if (anchor == null)
		{
			throw new ArgumentNullException("anchor");
		}
		if (name == null)
		{
			throw new ArgumentNullException("name");
		}
		if (!(anchor is StyledElement styled))
		{
			return null;
		}
		INameScope obj = (anchor as INameScope) ?? NameScope.GetNameScope(styled);
		if (obj == null)
		{
			return null;
		}
		return obj.Find<T>(name);
	}

	public static T Get<T>(this INameScope nameScope, string name) where T : class
	{
		if (nameScope == null)
		{
			throw new ArgumentNullException("nameScope");
		}
		if (name == null)
		{
			throw new ArgumentNullException("name");
		}
		object obj = nameScope.Find(name);
		if (obj == null)
		{
			throw new KeyNotFoundException("Could not find control '" + name + "'.");
		}
		if (obj is T result)
		{
			return result;
		}
		throw new InvalidOperationException($"Expected control '{name}' to be '{typeof(T)} but it was '{obj.GetType()}'.");
	}

	public static T Get<T>(this ILogical anchor, string name) where T : class
	{
		if (anchor == null)
		{
			throw new ArgumentNullException("anchor");
		}
		if (name == null)
		{
			throw new ArgumentNullException("name");
		}
		return (((anchor as INameScope) ?? NameScope.GetNameScope((StyledElement)anchor)) ?? throw new InvalidOperationException("The control doesn't have an associated name scope, probably no registrations has been done yet")).Get<T>(name);
	}

	public static INameScope? FindNameScope(this ILogical control)
	{
		if (control == null)
		{
			throw new ArgumentNullException("control");
		}
		return (from x in control.GetSelfAndLogicalAncestors().OfType<StyledElement>()
			select (x as INameScope) ?? NameScope.GetNameScope(x)).FirstOrDefault((INameScope x) => x != null);
	}
}
