using System.Collections.Generic;

namespace SixLabors.ImageSharp.Formats.Webp.Lossless;

internal class Vp8LBackwardRefs
{
	public int BlockSize { get; set; }

	public List<PixOrCopy> Refs { get; }

	public Vp8LBackwardRefs(int pixels)
	{
		Refs = new List<PixOrCopy>(pixels);
	}

	public void Add(PixOrCopy pixOrCopy)
	{
		Refs.Add(pixOrCopy);
	}
}
