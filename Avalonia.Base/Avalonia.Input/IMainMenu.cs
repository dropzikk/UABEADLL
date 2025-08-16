using System;
using Avalonia.Interactivity;

namespace Avalonia.Input;

internal interface IMainMenu
{
	bool IsOpen { get; }

	event EventHandler<RoutedEventArgs>? Closed;

	void Close();

	void Open();
}
