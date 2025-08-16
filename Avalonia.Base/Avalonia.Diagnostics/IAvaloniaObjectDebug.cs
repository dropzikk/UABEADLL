using System;

namespace Avalonia.Diagnostics;

internal interface IAvaloniaObjectDebug
{
	Delegate[]? GetPropertyChangedSubscribers();
}
