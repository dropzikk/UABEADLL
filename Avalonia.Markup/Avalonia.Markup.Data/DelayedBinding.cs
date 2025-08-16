using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Avalonia.Data;
using Avalonia.Logging;

namespace Avalonia.Markup.Data;

internal static class DelayedBinding
{
	private abstract class Entry
	{
		public abstract void Apply(StyledElement control);
	}

	private class BindingEntry : Entry
	{
		public IBinding Binding { get; }

		public AvaloniaProperty Property { get; }

		public BindingEntry(AvaloniaProperty property, IBinding binding)
		{
			Binding = binding;
			Property = property;
		}

		public override void Apply(StyledElement control)
		{
			control.Bind(Property, Binding);
		}
	}

	private class ClrPropertyValueEntry : Entry
	{
		public PropertyInfo Property { get; }

		public Func<StyledElement, object?> Value { get; }

		public ClrPropertyValueEntry(PropertyInfo property, Func<StyledElement, object?> value)
		{
			Property = property;
			Value = value;
		}

		public override void Apply(StyledElement control)
		{
			try
			{
				Property.SetValue(control, Value(control));
			}
			catch (Exception propertyValue)
			{
				Logger.TryGet(LogEventLevel.Error, "Property")?.Log(control, "Error setting {Property} on {Target}: {Exception}", Property.Name, control, propertyValue);
			}
		}
	}

	private static readonly ConditionalWeakTable<StyledElement, List<Entry>> _entries = new ConditionalWeakTable<StyledElement, List<Entry>>();

	public static void Add(StyledElement target, AvaloniaProperty property, IBinding binding)
	{
		if (target.IsInitialized)
		{
			target.Bind(property, binding);
			return;
		}
		if (!_entries.TryGetValue(target, out List<Entry> value))
		{
			value = new List<Entry>();
			_entries.Add(target, value);
			target.Initialized += ApplyBindings;
		}
		value.Add(new BindingEntry(property, binding));
	}

	public static void Add(StyledElement target, PropertyInfo property, Func<StyledElement, object?> value)
	{
		if (target.IsInitialized)
		{
			property.SetValue(target, value(target));
			return;
		}
		if (!_entries.TryGetValue(target, out List<Entry> value2))
		{
			value2 = new List<Entry>();
			_entries.Add(target, value2);
			target.Initialized += ApplyBindings;
		}
		value2.Add(new ClrPropertyValueEntry(property, value));
	}

	public static void ApplyBindings(StyledElement control)
	{
		if (!_entries.TryGetValue(control, out List<Entry> value))
		{
			return;
		}
		foreach (Entry item in value)
		{
			item.Apply(control);
		}
		_entries.Remove(control);
	}

	private static void ApplyBindings(object? sender, EventArgs e)
	{
		StyledElement obj = (StyledElement)sender;
		ApplyBindings(obj);
		obj.Initialized -= ApplyBindings;
	}
}
