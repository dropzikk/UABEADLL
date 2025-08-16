using System;

namespace Avalonia.Controls;

public static class PseudolassesExtensions
{
	public static void Set(this IPseudoClasses classes, string name, bool value)
	{
		if (classes == null)
		{
			throw new ArgumentNullException("classes");
		}
		if (value)
		{
			classes.Add(name);
		}
		else
		{
			classes.Remove(name);
		}
	}
}
