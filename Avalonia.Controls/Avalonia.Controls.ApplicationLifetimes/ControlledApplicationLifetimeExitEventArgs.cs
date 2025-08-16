using System;

namespace Avalonia.Controls.ApplicationLifetimes;

public class ControlledApplicationLifetimeExitEventArgs : EventArgs
{
	public int ApplicationExitCode { get; set; }

	public ControlledApplicationLifetimeExitEventArgs(int applicationExitCode)
	{
		ApplicationExitCode = applicationExitCode;
	}
}
