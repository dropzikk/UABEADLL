using System;

namespace Avalonia.Metadata;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class AvaloniaListAttribute : Attribute
{
	public string[]? Separators { get; init; }

	public StringSplitOptions SplitOptions { get; init; } = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;
}
