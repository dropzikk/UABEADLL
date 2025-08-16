using System;
using System.Collections.Generic;
using Avalonia.Collections;

namespace Avalonia.Controls;

public class Controls : AvaloniaList<Control>
{
	public Controls()
	{
		Configure();
	}

	public Controls(IEnumerable<Control> items)
	{
		Configure();
		AddRange(items);
	}

	private void Configure()
	{
		base.ResetBehavior = ResetBehavior.Remove;
		base.Validate = delegate(Control item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item", "A null control cannot be added to a Controls collection.");
			}
		};
	}
}
