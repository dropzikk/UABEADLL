using System;
using System.Collections.Generic;
using System.Text;

namespace LibCpp2IL;

public static class Extensions
{
	public static T[] SubArray<T>(this T[] data, int index, int length)
	{
		T[] array = new T[length];
		Array.Copy(data, index, array, 0, length);
		return array;
	}

	public static T RemoveAndReturn<T>(this List<T> data, int index)
	{
		T result = data[index];
		data.RemoveAt(index);
		return result;
	}

	public static string Repeat(this string source, int count)
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < count; i++)
		{
			stringBuilder.Append(source);
		}
		return stringBuilder.ToString();
	}

	public static string ToStringEnumerable<T>(this IEnumerable<T> enumerable)
	{
		StringBuilder stringBuilder = new StringBuilder("[");
		stringBuilder.Append(string.Join(", ", enumerable));
		stringBuilder.Append("]");
		return stringBuilder.ToString();
	}

	public static TValue? GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue? defaultValue)
	{
		if (dictionary == null)
		{
			throw new ArgumentNullException("dictionary");
		}
		if (dictionary.TryGetValue(key, out TValue value))
		{
			return value;
		}
		return defaultValue;
	}

	public static TValue? GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
	{
		return dictionary.GetValueOrDefault(key, default(TValue));
	}

	public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> pair, out TKey one, out TValue two)
	{
		one = pair.Key;
		two = pair.Value;
	}

	public static uint Bits(this uint x, int low, int count)
	{
		return (x >> low) & (uint)((1 << count) - 1);
	}

	public static bool TryAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
	{
		if (dictionary.ContainsKey(key))
		{
			return false;
		}
		dictionary.Add(key, value);
		return true;
	}

	public static void SortByExtractedKey<T, K>(this List<T> list, Func<T, K> keyObtainer) where K : IComparable<K>
	{
		list.Sort(delegate(T a, T b)
		{
			K val = keyObtainer(a);
			K other = keyObtainer(b);
			return val.CompareTo(other);
		});
	}

	public static int ParseDigit(this char c)
	{
		return c - 48;
	}
}
