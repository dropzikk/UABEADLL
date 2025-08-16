using Avalonia.Native;

namespace Avalonia;

public static class AvaloniaNativePlatformExtensions
{
	public static AppBuilder UseAvaloniaNative(this AppBuilder builder)
	{
		builder.UseStandardRuntimePlatformSubsystem().UseWindowingSubsystem(delegate
		{
			AvaloniaNativePlatform platform = AvaloniaNativePlatform.Initialize(AvaloniaLocator.Current.GetService<AvaloniaNativePlatformOptions>() ?? new AvaloniaNativePlatformOptions());
			builder.AfterSetup(delegate
			{
				platform.SetupApplicationName();
				platform.SetupApplicationMenuExporter();
			});
		});
		return builder;
	}
}
