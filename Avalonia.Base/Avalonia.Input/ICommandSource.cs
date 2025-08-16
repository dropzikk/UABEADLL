using System;
using System.Windows.Input;

namespace Avalonia.Input;

public interface ICommandSource
{
	ICommand? Command { get; }

	object? CommandParameter { get; }

	bool IsEffectivelyEnabled { get; }

	void CanExecuteChanged(object sender, EventArgs e);
}
