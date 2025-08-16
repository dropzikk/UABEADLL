using System;

namespace Avalonia.Platform.Storage;

public class StorageItemProperties
{
	public ulong? Size { get; }

	public DateTimeOffset? DateCreated { get; }

	public DateTimeOffset? DateModified { get; }

	public StorageItemProperties(ulong? size = null, DateTimeOffset? dateCreated = null, DateTimeOffset? dateModified = null)
	{
		Size = size;
		DateCreated = dateCreated;
		DateModified = dateModified;
	}
}
