using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

namespace Avalonia;

public static class ClassicDesktopStyleApplicationLifetimeExtensions
{
	public static int StartWithClassicDesktopLifetime(this AppBuilder builder, string[] args, ShutdownMode shutdownMode = ShutdownMode.OnLastWindowClose)
	{
		ClassicDesktopStyleApplicationLifetime classicDesktopStyleApplicationLifetime = new ClassicDesktopStyleApplicationLifetime
		{
			Args = args,
			ShutdownMode = shutdownMode
		};
		builder.SetupWithLifetime(classicDesktopStyleApplicationLifetime);
		return classicDesktopStyleApplicationLifetime.Start(args);
	}
}
