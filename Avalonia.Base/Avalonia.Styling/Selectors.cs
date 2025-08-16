using System;
using System.Collections.Generic;

namespace Avalonia.Styling;

public static class Selectors
{
	public static Selector Child(this Selector previous)
	{
		return new ChildSelector(previous);
	}

	public static Selector Class(this Selector? previous, string name)
	{
		if (name == null)
		{
			throw new ArgumentNullException("name");
		}
		if (string.IsNullOrWhiteSpace(name))
		{
			throw new ArgumentException("Name may not be empty", "name");
		}
		if (previous is TypeNameAndClassSelector typeNameAndClassSelector)
		{
			typeNameAndClassSelector.Classes.Add(name);
			return typeNameAndClassSelector;
		}
		return TypeNameAndClassSelector.ForClass(previous, name);
	}

	public static Selector Descendant(this Selector? previous)
	{
		return new DescendantSelector(previous);
	}

	public static Selector Is(this Selector? previous, Type type)
	{
		if ((object)type == null)
		{
			throw new ArgumentNullException("type");
		}
		return TypeNameAndClassSelector.Is(previous, type);
	}

	public static Selector Is<T>(this Selector? previous) where T : StyledElement
	{
		return previous.Is(typeof(T));
	}

	public static Selector Name(this Selector? previous, string name)
	{
		if (name == null)
		{
			throw new ArgumentNullException("name");
		}
		if (string.IsNullOrWhiteSpace(name))
		{
			throw new ArgumentException("Name may not be empty", "name");
		}
		if (previous is TypeNameAndClassSelector typeNameAndClassSelector)
		{
			typeNameAndClassSelector.Name = name;
			return typeNameAndClassSelector;
		}
		return TypeNameAndClassSelector.ForName(previous, name);
	}

	public static Selector Nesting(this Selector? previous)
	{
		return new NestingSelector();
	}

	public static Selector Not(this Selector? previous, Func<Selector?, Selector> argument)
	{
		return new NotSelector(previous, argument(null));
	}

	public static Selector Not(this Selector? previous, Selector argument)
	{
		return new NotSelector(previous, argument);
	}

	public static Selector NthChild(this Selector? previous, int step, int offset)
	{
		return new NthChildSelector(previous, step, offset);
	}

	public static Selector NthLastChild(this Selector? previous, int step, int offset)
	{
		return new NthLastChildSelector(previous, step, offset);
	}

	public static Selector OfType(this Selector? previous, Type type)
	{
		if ((object)type == null)
		{
			throw new ArgumentNullException("type");
		}
		return TypeNameAndClassSelector.OfType(previous, type);
	}

	public static Selector OfType<T>(this Selector? previous) where T : StyledElement
	{
		return previous.OfType(typeof(T));
	}

	public static Selector Or(params Selector[] selectors)
	{
		return new OrSelector(selectors);
	}

	public static Selector Or(IReadOnlyList<Selector> selectors)
	{
		return new OrSelector(selectors);
	}

	public static Selector PropertyEquals<T>(this Selector? previous, AvaloniaProperty<T> property, object? value)
	{
		if ((object)property == null)
		{
			throw new ArgumentNullException("property");
		}
		return new PropertyEqualsSelector(previous, property, value);
	}

	public static Selector PropertyEquals(this Selector? previous, AvaloniaProperty property, object? value)
	{
		if ((object)property == null)
		{
			throw new ArgumentNullException("property");
		}
		return new PropertyEqualsSelector(previous, property, value);
	}

	public static Selector Template(this Selector previous)
	{
		return new TemplateSelector(previous);
	}
}
