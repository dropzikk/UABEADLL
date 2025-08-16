using System.Runtime.CompilerServices;

namespace SixLabors.ImageSharp.Formats.Webp.Lossless;

internal class ColorCache
{
	private const uint HashMul = 506832829u;

	public uint[] Colors { get; }

	public int HashShift { get; }

	public int HashBits { get; }

	public ColorCache(int hashBits)
	{
		int num = 1 << hashBits;
		Colors = new uint[num];
		HashBits = hashBits;
		HashShift = 32 - hashBits;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Insert(uint bgra)
	{
		int num = HashPix(bgra, HashShift);
		Colors[num] = bgra;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public uint Lookup(int key)
	{
		return Colors[key];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int Contains(uint bgra)
	{
		int num = HashPix(bgra, HashShift);
		if (Colors[num] != bgra)
		{
			return -1;
		}
		return num;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int GetIndex(uint bgra)
	{
		return HashPix(bgra, HashShift);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Set(uint key, uint bgra)
	{
		Colors[key] = bgra;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int HashPix(uint argb, int shift)
	{
		return (int)(argb * 506832829 >> shift);
	}
}
