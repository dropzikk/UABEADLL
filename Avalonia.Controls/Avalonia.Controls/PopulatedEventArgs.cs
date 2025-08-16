using System;
using System.Collections;

namespace Avalonia.Controls;

public class PopulatedEventArgs : EventArgs
{
	public IEnumerable Data { get; private set; }

	public PopulatedEventArgs(IEnumerable data)
	{
		Data = data;
	}
}
