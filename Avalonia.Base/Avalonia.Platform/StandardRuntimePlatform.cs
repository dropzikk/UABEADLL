using Avalonia.Compatibility;
using Avalonia.Metadata;

namespace Avalonia.Platform;

[PrivateApi]
public class StandardRuntimePlatform : IRuntimePlatform
{
	private static readonly RuntimePlatformInfo s_info = new RuntimePlatformInfo
	{
		IsDesktop = (OperatingSystemEx.IsWindows() || OperatingSystemEx.IsMacOS() || OperatingSystemEx.IsLinux()),
		IsMobile = (OperatingSystemEx.IsAndroid() || OperatingSystemEx.IsIOS())
	};

	public virtual RuntimePlatformInfo GetRuntimeInfo()
	{
		return s_info;
	}
}
