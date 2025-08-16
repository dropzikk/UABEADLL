using System;
using System.Collections.Generic;
using System.Linq;

namespace Avalonia.Controls.ApplicationLifetimes;

public class ControlledApplicationLifetimeStartupEventArgs : EventArgs
{
	public string[] Args { get; }

	public ControlledApplicationLifetimeStartupEventArgs(IEnumerable<string> args)
	{
		Args = args?.ToArray() ?? Array.Empty<string>();
	}
}
