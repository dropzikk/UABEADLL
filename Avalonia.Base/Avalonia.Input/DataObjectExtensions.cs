using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Avalonia.Platform.Storage;

namespace Avalonia.Input;

public static class DataObjectExtensions
{
	public static IEnumerable<IStorageItem>? GetFiles(this IDataObject dataObject)
	{
		return dataObject.Get(DataFormats.Files) as IEnumerable<IStorageItem>;
	}

	[Obsolete("Use GetFiles, this method is supported only on desktop platforms.")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static IEnumerable<string>? GetFileNames(this IDataObject dataObject)
	{
		object obj = dataObject.Get(DataFormats.FileNames) as IEnumerable<string>;
		if (obj == null)
		{
			IEnumerable<IStorageItem>? files = dataObject.GetFiles();
			if (files == null)
			{
				return null;
			}
			obj = (from f in files
				select f.TryGetLocalPath() into p
				where !string.IsNullOrEmpty(p)
				select p).OfType<string>();
		}
		return (IEnumerable<string>?)obj;
	}

	public static string? GetText(this IDataObject dataObject)
	{
		return dataObject.Get(DataFormats.Text) as string;
	}
}
