using System;

namespace Tmds.DBus.SourceGenerator;

public record PropertyChanges<TProperties>(TProperties Properties, string[] Invalidated, string[] Changed)
{
	public bool HasChanged(string property)
	{
		return Array.IndexOf(Changed, property) != -1;
	}

	public bool IsInvalidated(string property)
	{
		return Array.IndexOf(Invalidated, property) != -1;
	}
}
