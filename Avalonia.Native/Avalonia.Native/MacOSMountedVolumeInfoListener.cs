using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Avalonia.Controls.Platform;
using Avalonia.Threading;

namespace Avalonia.Native;

internal class MacOSMountedVolumeInfoListener : IDisposable
{
	private readonly IDisposable _disposable;

	private bool _beenDisposed;

	private ObservableCollection<MountedVolumeInfo> mountedDrives;

	public MacOSMountedVolumeInfoListener(ObservableCollection<MountedVolumeInfo> mountedDrives)
	{
		this.mountedDrives = mountedDrives;
		_disposable = DispatcherTimer.Run(Poll, TimeSpan.FromSeconds(1.0));
		Poll();
	}

	private bool Poll()
	{
		MountedVolumeInfo[] array = (from p in Directory.GetDirectories("/Volumes/")
			where p != null
			select new MountedVolumeInfo
			{
				VolumeLabel = Path.GetFileName(p),
				VolumePath = p,
				VolumeSizeBytes = 0uL
			}).ToArray();
		if (mountedDrives.SequenceEqual(array))
		{
			return true;
		}
		mountedDrives.Clear();
		MountedVolumeInfo[] array2 = array;
		foreach (MountedVolumeInfo item in array2)
		{
			mountedDrives.Add(item);
		}
		return true;
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!_beenDisposed)
		{
			_beenDisposed = true;
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
	}
}
