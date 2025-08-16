using System;

namespace Avalonia.Metadata;

[AttributeUsage(AttributeTargets.All)]
public sealed class UnstableAttribute : Attribute
{
	public string? Message { get; }

	public UnstableAttribute()
	{
	}

	public UnstableAttribute(string? message)
	{
		Message = message;
	}
}
