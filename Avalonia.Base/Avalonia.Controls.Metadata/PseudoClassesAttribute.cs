using System;
using System.Collections.Generic;

namespace Avalonia.Controls.Metadata;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class PseudoClassesAttribute : Attribute
{
	public IReadOnlyList<string> PseudoClasses { get; }

	public PseudoClassesAttribute(params string[] pseudoClasses)
	{
		PseudoClasses = pseudoClasses;
	}
}
