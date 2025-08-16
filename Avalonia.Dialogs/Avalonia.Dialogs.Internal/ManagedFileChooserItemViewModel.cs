using System;

namespace Avalonia.Dialogs.Internal;

public class ManagedFileChooserItemViewModel : AvaloniaDialogsInternalViewModelBase
{
	private string _displayName;

	private string _path;

	private DateTime _modified;

	private string _type;

	private long _size;

	private ManagedFileChooserItemType _itemType;

	public string DisplayName
	{
		get
		{
			return _displayName;
		}
		set
		{
			RaiseAndSetIfChanged(ref _displayName, value, "DisplayName");
		}
	}

	public string Path
	{
		get
		{
			return _path;
		}
		set
		{
			RaiseAndSetIfChanged(ref _path, value, "Path");
		}
	}

	public DateTime Modified
	{
		get
		{
			return _modified;
		}
		set
		{
			RaiseAndSetIfChanged(ref _modified, value, "Modified");
		}
	}

	public string Type
	{
		get
		{
			return _type;
		}
		set
		{
			RaiseAndSetIfChanged(ref _type, value, "Type");
		}
	}

	public long Size
	{
		get
		{
			return _size;
		}
		set
		{
			RaiseAndSetIfChanged(ref _size, value, "Size");
		}
	}

	public ManagedFileChooserItemType ItemType
	{
		get
		{
			return _itemType;
		}
		set
		{
			RaiseAndSetIfChanged(ref _itemType, value, "ItemType");
		}
	}

	public string IconKey => ItemType switch
	{
		ManagedFileChooserItemType.Folder => "Icon_Folder", 
		ManagedFileChooserItemType.Volume => "Icon_Volume", 
		_ => "Icon_File", 
	};

	public ManagedFileChooserItemViewModel()
	{
	}

	public ManagedFileChooserItemViewModel(ManagedFileChooserNavigationItem item)
	{
		ItemType = item.ItemType;
		Path = item.Path;
		DisplayName = item.DisplayName;
	}
}
