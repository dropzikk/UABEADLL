using System;
using System.Collections.Concurrent;

namespace Avalonia.Skia;

internal abstract class SKCacheBase<TCachedItem, TCache> where TCachedItem : IDisposable, new() where TCache : new()
{
	protected readonly ConcurrentBag<TCachedItem> Cache;

	public static readonly TCache Shared = new TCache();

	protected SKCacheBase()
	{
		Cache = new ConcurrentBag<TCachedItem>();
	}

	public TCachedItem Get()
	{
		if (!Cache.TryTake(out var result))
		{
			return new TCachedItem();
		}
		return result;
	}

	public void Return(TCachedItem item)
	{
		Cache.Add(item);
	}

	public void Clear()
	{
		TCachedItem result;
		while (Cache.TryTake(out result))
		{
			result.Dispose();
		}
	}
}
