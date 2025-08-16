using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Avalonia.Utilities;

internal class RefTrackingDictionary<TKey> : Dictionary<TKey, int> where TKey : class
{
	public bool AddRef(TKey key)
	{
		bool exists;
		ref int valueRefOrAddDefault = ref CollectionsMarshal.GetValueRefOrAddDefault(this, key, out exists);
		valueRefOrAddDefault++;
		return valueRefOrAddDefault == 1;
	}

	public bool ReleaseRef(TKey key)
	{
		ref int valueRefOrNullRef = ref CollectionsMarshal.GetValueRefOrNullRef(this, key);
		if (Unsafe.IsNullRef(ref valueRefOrNullRef))
		{
			return false;
		}
		valueRefOrNullRef--;
		if (valueRefOrNullRef == 0)
		{
			Remove(key);
			return true;
		}
		return false;
	}
}
