using System;
using System.Collections.ObjectModel;
using Avalonia.Metadata;

namespace Avalonia.Controls.Platform;

[Unstable]
public interface IMountedVolumeInfoProvider
{
	IDisposable Listen(ObservableCollection<MountedVolumeInfo> mountedDrives);
}
