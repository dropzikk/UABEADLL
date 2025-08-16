using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Avalonia.Controls.Platform;
using Avalonia.Threading;

namespace Avalonia.FreeDesktop;

internal class LinuxMountedVolumeInfoListener : IDisposable
{
	private const string DevByLabelDir = "/dev/disk/by-label/";

	private const string ProcPartitionsDir = "/proc/partitions";

	private const string ProcMountsDir = "/proc/mounts";

	private IDisposable _disposable;

	private ObservableCollection<MountedVolumeInfo> _targetObs;

	private bool _beenDisposed;

	public LinuxMountedVolumeInfoListener(ref ObservableCollection<MountedVolumeInfo> target)
	{
		_targetObs = target;
		_disposable = DispatcherTimer.Run(Poll, TimeSpan.FromSeconds(1.0));
		Poll();
	}

	private static string GetSymlinkTarget(string x)
	{
		return Path.GetFullPath(Path.Combine("/dev/disk/by-label/", NativeMethods.ReadLink(x)));
	}

	private static string UnescapeString(string input, string regexText, int escapeBase)
	{
		return new Regex(regexText).Replace(input, (Match m) => Convert.ToChar(Convert.ToByte(m.Groups[1].Value, escapeBase)).ToString());
	}

	private static string UnescapePathFromProcMounts(string input)
	{
		return UnescapeString(input, "\\\\(\\d{3})", 8);
	}

	private static string UnescapeDeviceLabel(string input)
	{
		return UnescapeString(input, "\\\\x([0-9a-f]{2})", 16);
	}

	private bool Poll()
	{
		IEnumerable<(ulong, string)> inner = from p in File.ReadAllLines("/proc/partitions").Skip(1)
			where !string.IsNullOrEmpty(p)
			select Regex.Replace(p, "\\s{2,}", " ").Trim().Split(' ') into p
			select (p[2].Trim(), p[3].Trim()) into p
			select (Convert.ToUInt64(p.Item1) * 1024, "/dev/" + p.Item2);
		IEnumerable<(string, string)> outer = from x in File.ReadAllLines("/proc/mounts")
			select x.Split(' ') into x
			select (x[0], UnescapePathFromProcMounts(x[1])) into x
			where !x.Item2.StartsWith("/snap/", StringComparison.InvariantCultureIgnoreCase)
			select x;
		IEnumerable<FileInfo> source;
		if (!Directory.Exists("/dev/disk/by-label/"))
		{
			source = Enumerable.Empty<FileInfo>();
		}
		else
		{
			IEnumerable<FileInfo> files = new DirectoryInfo("/dev/disk/by-label/").GetFiles();
			source = files;
		}
		MountedVolumeInfo[] array = (from _003C_003Eh__TransparentIdentifier1 in Enumerable.GroupJoin(inner: source.Select((FileInfo x) => (GetSymlinkTarget(x.FullName), UnescapeDeviceLabel(x.Name))), outer: from mount in outer
				join device in inner on mount.Item1 equals device.Item2
				select new { mount, device }, outerKeySelector: _003C_003Eh__TransparentIdentifier0 => _003C_003Eh__TransparentIdentifier0.device.Item2, innerKeySelector: ((string, string) label) => label.Item1, resultSelector: (_003C_003Eh__TransparentIdentifier0, IEnumerable<(string, string)> labelMatches) => new { _003C_003Eh__TransparentIdentifier0, labelMatches })
			from x in _003C_003Eh__TransparentIdentifier1.labelMatches.DefaultIfEmpty()
			select new MountedVolumeInfo
			{
				VolumePath = _003C_003Eh__TransparentIdentifier1._003C_003Eh__TransparentIdentifier0.mount.Item2,
				VolumeSizeBytes = _003C_003Eh__TransparentIdentifier1._003C_003Eh__TransparentIdentifier0.device.Item1,
				VolumeLabel = x.Item2
			}).ToArray();
		if (_targetObs.SequenceEqual(array))
		{
			return true;
		}
		_targetObs.Clear();
		MountedVolumeInfo[] array2 = array;
		foreach (MountedVolumeInfo item in array2)
		{
			_targetObs.Add(item);
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
				_targetObs.Clear();
			}
			_beenDisposed = true;
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
	}
}
