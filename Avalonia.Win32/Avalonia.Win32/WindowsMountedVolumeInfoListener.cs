using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Avalonia.Controls.Platform;
using Avalonia.Logging;
using Avalonia.Threading;

namespace Avalonia.Win32;

internal class WindowsMountedVolumeInfoListener : IDisposable
{
	private readonly IDisposable _disposable;

	private bool _beenDisposed;

	private readonly ObservableCollection<MountedVolumeInfo> _mountedDrives;

	public WindowsMountedVolumeInfoListener(ObservableCollection<MountedVolumeInfo> mountedDrives)
	{
		_mountedDrives = mountedDrives;
		_disposable = DispatcherTimer.Run(Poll, TimeSpan.FromSeconds(1.0));
		Poll();
	}

	private bool Poll()
	{
		MountedVolumeInfo[] array = (from p in DriveInfo.GetDrives().Where(delegate(DriveInfo p)
			{
				try
				{
					return p.IsReady;
				}
				catch (Exception ex)
				{
					Logger.TryGet(LogEventLevel.Warning, "Control")?.Log(this, "Error in Windows drive enumeration: " + ex.Message);
				}
				return false;
			})
			select new MountedVolumeInfo
			{
				VolumeLabel = (string.IsNullOrEmpty(p.VolumeLabel.Trim()) ? p.RootDirectory.FullName : (p.VolumeLabel + " (" + p.Name + ")")),
				VolumePath = p.RootDirectory.FullName,
				VolumeSizeBytes = (ulong)p.TotalSize
			}).ToArray();
		if (_mountedDrives.SequenceEqual(array))
		{
			return true;
		}
		_mountedDrives.Clear();
		MountedVolumeInfo[] array2 = array;
		foreach (MountedVolumeInfo item in array2)
		{
			_mountedDrives.Add(item);
		}
		return true;
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!_beenDisposed)
		{
			if (disposing)
			{
				_disposable.Dispose();
			}
			_beenDisposed = true;
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
	}
}
