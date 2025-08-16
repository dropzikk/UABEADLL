using System;
using System.Collections.Generic;
using SixLabors.ImageSharp.Formats.Png.Chunks;

namespace SixLabors.ImageSharp.Formats.Png;

public class PngMetadata : IDeepCloneable
{
	public PngBitDepth? BitDepth { get; set; }

	public PngColorType? ColorType { get; set; }

	public PngInterlaceMode? InterlaceMethod { get; set; } = PngInterlaceMode.None;

	public float Gamma { get; set; }

	public ReadOnlyMemory<Color>? ColorTable { get; set; }

	public Color? TransparentColor { get; set; }

	public IList<PngTextData> TextData { get; set; } = new List<PngTextData>();

	public uint RepeatCount { get; set; } = 1u;

	public bool AnimateRootFrame { get; set; } = true;

	public PngMetadata()
	{
	}

	private PngMetadata(PngMetadata other)
	{
		BitDepth = other.BitDepth;
		ColorType = other.ColorType;
		Gamma = other.Gamma;
		InterlaceMethod = other.InterlaceMethod;
		TransparentColor = other.TransparentColor;
		RepeatCount = other.RepeatCount;
		AnimateRootFrame = other.AnimateRootFrame;
		ReadOnlyMemory<Color>? colorTable = other.ColorTable;
		if (colorTable.HasValue && colorTable.GetValueOrDefault().Length > 0)
		{
			ColorTable = other.ColorTable.Value.ToArray();
		}
		for (int i = 0; i < other.TextData.Count; i++)
		{
			TextData.Add(other.TextData[i]);
		}
	}

	public IDeepCloneable DeepClone()
	{
		return new PngMetadata(this);
	}

	internal static PngMetadata FromAnimatedMetadata(AnimatedImageMetadata metadata)
	{
		Color[] array = (metadata.ColorTable.HasValue ? metadata.ColorTable.Value.ToArray() : null);
		if (array != null)
		{
			for (int i = 0; i < array.Length; i++)
			{
				ref Color reference = ref array[i];
				if (reference == metadata.BackgroundColor)
				{
					reference = Color.Transparent;
					break;
				}
			}
		}
		return new PngMetadata
		{
			ColorType = ((array != null) ? new PngColorType?(PngColorType.Palette) : ((PngColorType?)null)),
			BitDepth = ((array != null) ? new PngBitDepth?((PngBitDepth)Numerics.Clamp(ColorNumerics.GetBitsNeededForColorDepth(array.Length), 1, 8)) : ((PngBitDepth?)null)),
			ColorTable = array,
			RepeatCount = metadata.RepeatCount
		};
	}
}
