using System;
using Avalonia.Collections;
using Avalonia.Threading;

namespace Avalonia.Animation;

public sealed class Transitions : AvaloniaList<ITransition>
{
	public Transitions()
	{
		base.ResetBehavior = ResetBehavior.Remove;
		base.Validate = ValidateTransition;
	}

	private void ValidateTransition(ITransition obj)
	{
		Dispatcher.UIThread.VerifyAccess();
		if (obj.Property.IsDirect)
		{
			throw new InvalidOperationException("Cannot animate a direct property.");
		}
	}
}
