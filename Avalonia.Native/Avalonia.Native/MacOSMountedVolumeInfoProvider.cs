using System;
using System.Collections.ObjectModel;
using Avalonia.Controls.Platform;

namespace Avalonia.Native;

internal class MacOSMountedVolumeInfoProvider : IMountedVolumeInfoProvider
{
	public IDisposable Listen(ObservableCollection<MountedVolumeInfo> mountedDrives)
	{
		return new MacOSMountedVolumeInfoListener(mountedDrives);
	}
}
