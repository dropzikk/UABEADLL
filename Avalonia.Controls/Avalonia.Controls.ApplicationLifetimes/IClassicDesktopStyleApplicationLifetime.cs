using System;
using System.Collections.Generic;
using Avalonia.Metadata;

namespace Avalonia.Controls.ApplicationLifetimes;

[NotClientImplementable]
public interface IClassicDesktopStyleApplicationLifetime : IControlledApplicationLifetime, IApplicationLifetime
{
	string[]? Args { get; }

	ShutdownMode ShutdownMode { get; set; }

	Window? MainWindow { get; set; }

	IReadOnlyList<Window> Windows { get; }

	event EventHandler<ShutdownRequestedEventArgs>? ShutdownRequested;

	bool TryShutdown(int exitCode = 0);
}
