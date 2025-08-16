using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Tiff.PhotometricInterpretation;

internal static class TiffColorDecoderFactory<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
	public static TiffBaseColorDecoder<TPixel> Create(Configuration configuration, MemoryAllocator memoryAllocator, TiffColorType colorType, TiffBitsPerSample bitsPerSample, TiffExtraSampleType? extraSampleType, ushort[] colorMap, Rational[] referenceBlackAndWhite, Rational[] ycbcrCoefficients, ushort[] ycbcrSubSampling, ByteOrder byteOrder)
	{
		return colorType switch
		{
			TiffColorType.WhiteIsZero => new WhiteIsZeroTiffColor<TPixel>(bitsPerSample), 
			TiffColorType.WhiteIsZero1 => new WhiteIsZero1TiffColor<TPixel>(), 
			TiffColorType.WhiteIsZero4 => new WhiteIsZero4TiffColor<TPixel>(), 
			TiffColorType.WhiteIsZero8 => new WhiteIsZero8TiffColor<TPixel>(), 
			TiffColorType.WhiteIsZero16 => new WhiteIsZero16TiffColor<TPixel>(byteOrder == ByteOrder.BigEndian), 
			TiffColorType.WhiteIsZero24 => new WhiteIsZero24TiffColor<TPixel>(byteOrder == ByteOrder.BigEndian), 
			TiffColorType.WhiteIsZero32 => new WhiteIsZero32TiffColor<TPixel>(byteOrder == ByteOrder.BigEndian), 
			TiffColorType.WhiteIsZero32Float => new WhiteIsZero32FloatTiffColor<TPixel>(byteOrder == ByteOrder.BigEndian), 
			TiffColorType.BlackIsZero => new BlackIsZeroTiffColor<TPixel>(bitsPerSample), 
			TiffColorType.BlackIsZero1 => new BlackIsZero1TiffColor<TPixel>(), 
			TiffColorType.BlackIsZero4 => new BlackIsZero4TiffColor<TPixel>(), 
			TiffColorType.BlackIsZero8 => new BlackIsZero8TiffColor<TPixel>(configuration), 
			TiffColorType.BlackIsZero16 => new BlackIsZero16TiffColor<TPixel>(configuration, byteOrder == ByteOrder.BigEndian), 
			TiffColorType.BlackIsZero24 => new BlackIsZero24TiffColor<TPixel>(byteOrder == ByteOrder.BigEndian), 
			TiffColorType.BlackIsZero32 => new BlackIsZero32TiffColor<TPixel>(byteOrder == ByteOrder.BigEndian), 
			TiffColorType.BlackIsZero32Float => new BlackIsZero32FloatTiffColor<TPixel>(byteOrder == ByteOrder.BigEndian), 
			TiffColorType.Rgb => new RgbTiffColor<TPixel>(bitsPerSample), 
			TiffColorType.Rgb222 => new RgbTiffColor<TPixel>(bitsPerSample), 
			TiffColorType.Rgba2222 => new RgbaTiffColor<TPixel>(extraSampleType, bitsPerSample), 
			TiffColorType.Rgb333 => new RgbTiffColor<TPixel>(bitsPerSample), 
			TiffColorType.Rgba3333 => new RgbaTiffColor<TPixel>(extraSampleType, bitsPerSample), 
			TiffColorType.Rgb444 => new Rgb444TiffColor<TPixel>(), 
			TiffColorType.Rgba4444 => new RgbaTiffColor<TPixel>(extraSampleType, bitsPerSample), 
			TiffColorType.Rgb555 => new RgbTiffColor<TPixel>(bitsPerSample), 
			TiffColorType.Rgba5555 => new RgbaTiffColor<TPixel>(extraSampleType, bitsPerSample), 
			TiffColorType.Rgb666 => new RgbTiffColor<TPixel>(bitsPerSample), 
			TiffColorType.Rgba6666 => new RgbaTiffColor<TPixel>(extraSampleType, bitsPerSample), 
			TiffColorType.Rgb888 => new Rgb888TiffColor<TPixel>(configuration), 
			TiffColorType.Rgba8888 => new Rgba8888TiffColor<TPixel>(configuration, memoryAllocator, extraSampleType), 
			TiffColorType.Rgb101010 => new RgbTiffColor<TPixel>(bitsPerSample), 
			TiffColorType.Rgba10101010 => new RgbaTiffColor<TPixel>(extraSampleType, bitsPerSample), 
			TiffColorType.Rgb121212 => new RgbTiffColor<TPixel>(bitsPerSample), 
			TiffColorType.Rgba12121212 => new RgbaTiffColor<TPixel>(extraSampleType, bitsPerSample), 
			TiffColorType.Rgb141414 => new RgbTiffColor<TPixel>(bitsPerSample), 
			TiffColorType.Rgba14141414 => new RgbaTiffColor<TPixel>(extraSampleType, bitsPerSample), 
			TiffColorType.Rgb161616 => new Rgb161616TiffColor<TPixel>(configuration, byteOrder == ByteOrder.BigEndian), 
			TiffColorType.Rgba16161616 => new Rgba16161616TiffColor<TPixel>(configuration, memoryAllocator, extraSampleType, byteOrder == ByteOrder.BigEndian), 
			TiffColorType.Rgb242424 => new Rgb242424TiffColor<TPixel>(byteOrder == ByteOrder.BigEndian), 
			TiffColorType.Rgba24242424 => new Rgba24242424TiffColor<TPixel>(extraSampleType, byteOrder == ByteOrder.BigEndian), 
			TiffColorType.Rgb323232 => new Rgb323232TiffColor<TPixel>(byteOrder == ByteOrder.BigEndian), 
			TiffColorType.Rgba32323232 => new Rgba32323232TiffColor<TPixel>(extraSampleType, byteOrder == ByteOrder.BigEndian), 
			TiffColorType.RgbFloat323232 => new RgbFloat323232TiffColor<TPixel>(byteOrder == ByteOrder.BigEndian), 
			TiffColorType.RgbaFloat32323232 => new RgbaFloat32323232TiffColor<TPixel>(byteOrder == ByteOrder.BigEndian), 
			TiffColorType.PaletteColor => new PaletteTiffColor<TPixel>(bitsPerSample, colorMap), 
			TiffColorType.YCbCr => new YCbCrTiffColor<TPixel>(memoryAllocator, referenceBlackAndWhite, ycbcrCoefficients, ycbcrSubSampling), 
			TiffColorType.CieLab => new CieLabTiffColor<TPixel>(), 
			TiffColorType.Cmyk => new CmykTiffColor<TPixel>(), 
			_ => throw TiffThrowHelper.InvalidColorType(colorType.ToString()), 
		};
	}

	public static TiffBasePlanarColorDecoder<TPixel> CreatePlanar(TiffColorType colorType, TiffBitsPerSample bitsPerSample, TiffExtraSampleType? extraSampleType, ushort[] colorMap, Rational[] referenceBlackAndWhite, Rational[] ycbcrCoefficients, ushort[] ycbcrSubSampling, ByteOrder byteOrder)
	{
		return colorType switch
		{
			TiffColorType.Rgb888Planar => new RgbPlanarTiffColor<TPixel>(bitsPerSample), 
			TiffColorType.Rgba8888Planar => new RgbaPlanarTiffColor<TPixel>(extraSampleType, bitsPerSample), 
			TiffColorType.YCbCrPlanar => new YCbCrPlanarTiffColor<TPixel>(referenceBlackAndWhite, ycbcrCoefficients, ycbcrSubSampling), 
			TiffColorType.CieLabPlanar => new CieLabPlanarTiffColor<TPixel>(), 
			TiffColorType.Rgb161616Planar => new Rgb16PlanarTiffColor<TPixel>(byteOrder == ByteOrder.BigEndian), 
			TiffColorType.Rgba16161616Planar => new Rgba16PlanarTiffColor<TPixel>(extraSampleType, byteOrder == ByteOrder.BigEndian), 
			TiffColorType.Rgb242424Planar => new Rgb24PlanarTiffColor<TPixel>(byteOrder == ByteOrder.BigEndian), 
			TiffColorType.Rgba24242424Planar => new Rgba24PlanarTiffColor<TPixel>(extraSampleType, byteOrder == ByteOrder.BigEndian), 
			TiffColorType.Rgb323232Planar => new Rgb32PlanarTiffColor<TPixel>(byteOrder == ByteOrder.BigEndian), 
			TiffColorType.Rgba32323232Planar => new Rgba32PlanarTiffColor<TPixel>(extraSampleType, byteOrder == ByteOrder.BigEndian), 
			_ => throw TiffThrowHelper.InvalidColorType(colorType.ToString()), 
		};
	}
}
