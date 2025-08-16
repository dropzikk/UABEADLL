using System;
using System.ComponentModel;

namespace Avalonia.Data;

public enum BindingPriority
{
	Animation = -1,
	LocalValue = 0,
	StyleTrigger = 1,
	Template = 2,
	Style = 3,
	Inherited = 4,
	Unset = int.MaxValue,
	[Obsolete("Use Template priority")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	TemplatedParent = 2
}
