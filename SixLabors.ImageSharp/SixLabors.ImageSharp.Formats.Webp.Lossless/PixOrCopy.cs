using System.Diagnostics;

namespace SixLabors.ImageSharp.Formats.Webp.Lossless;

[DebuggerDisplay("Mode: {Mode}, Len: {Len}, BgraOrDistance: {BgraOrDistance}")]
internal sealed class PixOrCopy
{
	public PixOrCopyMode Mode { get; set; }

	public ushort Len { get; set; }

	public uint BgraOrDistance { get; set; }

	public static PixOrCopy CreateCacheIdx(int idx)
	{
		return new PixOrCopy
		{
			Mode = PixOrCopyMode.CacheIdx,
			BgraOrDistance = (uint)idx,
			Len = 1
		};
	}

	public static PixOrCopy CreateLiteral(uint bgra)
	{
		return new PixOrCopy
		{
			Mode = PixOrCopyMode.Literal,
			BgraOrDistance = bgra,
			Len = 1
		};
	}

	public static PixOrCopy CreateCopy(uint distance, ushort len)
	{
		return new PixOrCopy
		{
			Mode = PixOrCopyMode.Copy,
			BgraOrDistance = distance,
			Len = len
		};
	}

	public int Literal(int component)
	{
		return (int)((BgraOrDistance >> component * 8) & 0xFF);
	}

	public uint CacheIdx()
	{
		return BgraOrDistance;
	}

	public ushort Length()
	{
		return Len;
	}

	public uint Distance()
	{
		return BgraOrDistance;
	}

	public bool IsLiteral()
	{
		return Mode == PixOrCopyMode.Literal;
	}

	public bool IsCacheIdx()
	{
		return Mode == PixOrCopyMode.CacheIdx;
	}

	public bool IsCopy()
	{
		return Mode == PixOrCopyMode.Copy;
	}
}
