using System;
using Avalonia.Interactivity;

namespace Avalonia.Input;

internal interface IClickableControl
{
	bool IsEffectivelyEnabled { get; }

	event EventHandler<RoutedEventArgs> Click;

	void RaiseClick();
}
