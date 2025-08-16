using System;
using Avalonia.Metadata;

namespace Avalonia.Controls.ApplicationLifetimes;

[NotClientImplementable]
public interface IControlledApplicationLifetime : IApplicationLifetime
{
	event EventHandler<ControlledApplicationLifetimeStartupEventArgs> Startup;

	event EventHandler<ControlledApplicationLifetimeExitEventArgs> Exit;

	void Shutdown(int exitCode = 0);
}
