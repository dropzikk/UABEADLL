using Avalonia.Data;

namespace Avalonia.Diagnostics;

public class AvaloniaPropertyValue
{
	public AvaloniaProperty Property { get; }

	public object? Value { get; }

	public BindingPriority Priority { get; }

	public string? Diagnostic { get; }

	public bool IsOverriddenCurrentValue { get; }

	internal AvaloniaPropertyValue(AvaloniaProperty property, object? value, BindingPriority priority, string? diagnostic, bool isOverriddenCurrentValue)
	{
		Property = property;
		Value = value;
		Priority = priority;
		Diagnostic = diagnostic;
		IsOverriddenCurrentValue = isOverriddenCurrentValue;
	}
}
