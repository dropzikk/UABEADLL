using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Avalonia.Controls.Platform;
using Avalonia.Utilities;

namespace Avalonia.Dialogs.Internal;

internal class ManagedFileChooserSources
{
	public static readonly ObservableCollection<MountedVolumeInfo> MountedVolumes = new ObservableCollection<MountedVolumeInfo>();

	private static Environment.SpecialFolder[] s_folders = new Environment.SpecialFolder[6]
	{
		Environment.SpecialFolder.Desktop,
		Environment.SpecialFolder.UserProfile,
		Environment.SpecialFolder.Personal,
		Environment.SpecialFolder.MyMusic,
		Environment.SpecialFolder.MyPictures,
		Environment.SpecialFolder.MyVideos
	};

	public Func<ManagedFileChooserNavigationItem[]> GetUserDirectories { get; set; } = DefaultGetUserDirectories;

	public Func<ManagedFileChooserNavigationItem[]> GetFileSystemRoots { get; set; } = DefaultGetFileSystemRoots;

	public Func<ManagedFileChooserSources, ManagedFileChooserNavigationItem[]> GetAllItemsDelegate { get; set; } = DefaultGetAllItems;

	public ManagedFileChooserNavigationItem[] GetAllItems()
	{
		return GetAllItemsDelegate(this);
	}

	public static ManagedFileChooserNavigationItem[] DefaultGetAllItems(ManagedFileChooserSources sources)
	{
		return sources.GetUserDirectories().Concat(sources.GetFileSystemRoots()).ToArray();
	}

	public static ManagedFileChooserNavigationItem[] DefaultGetUserDirectories()
	{
		return (from d in (from d in s_folders.Select(Environment.GetFolderPath).Distinct()
				where !string.IsNullOrWhiteSpace(d)
				select d).Where(Directory.Exists)
			select new ManagedFileChooserNavigationItem
			{
				ItemType = ManagedFileChooserItemType.Folder,
				Path = d,
				DisplayName = Path.GetFileName(d)
			}).ToArray();
	}

	public static ManagedFileChooserNavigationItem[] DefaultGetFileSystemRoots()
	{
		return (from x in MountedVolumes.Select(delegate(MountedVolumeInfo x)
			{
				string text = x.VolumeLabel;
				if ((text == null) & (x.VolumeSizeBytes != 0))
				{
					text = ByteSizeHelper.ToString(x.VolumeSizeBytes, separate: true) + " Volume";
				}
				try
				{
					Directory.GetFiles(x.VolumePath);
				}
				catch (Exception)
				{
					return (ManagedFileChooserNavigationItem)null;
				}
				return new ManagedFileChooserNavigationItem
				{
					ItemType = ManagedFileChooserItemType.Volume,
					DisplayName = text,
					Path = x.VolumePath
				};
			})
			where x != null
			select x).ToArray();
	}
}
