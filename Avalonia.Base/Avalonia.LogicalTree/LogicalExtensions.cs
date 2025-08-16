using System;
using System.Collections.Generic;
using Avalonia.Collections;

namespace Avalonia.LogicalTree;

public static class LogicalExtensions
{
	public static IEnumerable<ILogical> GetLogicalAncestors(this ILogical logical)
	{
		if (logical == null)
		{
			throw new ArgumentNullException("logical");
		}
		for (ILogical l = logical.LogicalParent; l != null; l = l.LogicalParent)
		{
			yield return l;
		}
	}

	public static IEnumerable<ILogical> GetSelfAndLogicalAncestors(this ILogical logical)
	{
		yield return logical;
		foreach (ILogical logicalAncestor in logical.GetLogicalAncestors())
		{
			yield return logicalAncestor;
		}
	}

	public static T? FindLogicalAncestorOfType<T>(this ILogical? logical, bool includeSelf = false) where T : class
	{
		if (logical == null)
		{
			return null;
		}
		for (ILogical logical2 = (includeSelf ? logical : logical.LogicalParent); logical2 != null; logical2 = logical2.LogicalParent)
		{
			if (logical2 is T result)
			{
				return result;
			}
		}
		return null;
	}

	public static IEnumerable<ILogical> GetLogicalChildren(this ILogical logical)
	{
		return logical.LogicalChildren;
	}

	public static IEnumerable<ILogical> GetLogicalDescendants(this ILogical logical)
	{
		foreach (ILogical child in logical.LogicalChildren)
		{
			yield return child;
			foreach (ILogical logicalDescendant in child.GetLogicalDescendants())
			{
				yield return logicalDescendant;
			}
		}
	}

	public static IEnumerable<ILogical> GetSelfAndLogicalDescendants(this ILogical logical)
	{
		yield return logical;
		foreach (ILogical logicalDescendant in logical.GetLogicalDescendants())
		{
			yield return logicalDescendant;
		}
	}

	public static T? FindLogicalDescendantOfType<T>(this ILogical? logical, bool includeSelf = false) where T : class
	{
		if (logical == null)
		{
			return null;
		}
		if (includeSelf && logical is T result)
		{
			return result;
		}
		return FindDescendantOfTypeCore<T>(logical);
	}

	public static ILogical? GetLogicalParent(this ILogical logical)
	{
		return logical.LogicalParent;
	}

	public static T? GetLogicalParent<T>(this ILogical logical) where T : class
	{
		return logical.LogicalParent as T;
	}

	public static IEnumerable<ILogical> GetLogicalSiblings(this ILogical logical)
	{
		ILogical logicalParent = logical.LogicalParent;
		if (logicalParent == null)
		{
			yield break;
		}
		foreach (ILogical logicalChild in logicalParent.LogicalChildren)
		{
			yield return logicalChild;
		}
	}

	public static bool IsLogicalAncestorOf(this ILogical? logical, ILogical? target)
	{
		for (ILogical logical2 = target?.LogicalParent; logical2 != null; logical2 = logical2.LogicalParent)
		{
			if (logical2 == logical)
			{
				return true;
			}
		}
		return false;
	}

	private static T? FindDescendantOfTypeCore<T>(ILogical logical) where T : class
	{
		IAvaloniaReadOnlyList<ILogical> logicalChildren = logical.LogicalChildren;
		int count = logicalChildren.Count;
		for (int i = 0; i < count; i++)
		{
			ILogical logical2 = logicalChildren[i];
			if (logical2 is T result)
			{
				return result;
			}
			T val = FindDescendantOfTypeCore<T>(logical2);
			if (val != null)
			{
				return val;
			}
		}
		return null;
	}
}
