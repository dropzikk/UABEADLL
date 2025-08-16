using System.Runtime.CompilerServices;

namespace AssetRipper.TextureDecoder.Rgb;

internal static class ColorExtensions
{
	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	internal static void SetConvertedChannels<TThis, TThisArg, TSourceArg>(this ref TThis color, TSourceArg r, TSourceArg g, TSourceArg b, TSourceArg a) where TThis : unmanaged, IColor<TThisArg> where TThisArg : unmanaged where TSourceArg : unmanaged
	{
		color.SetChannels(ConversionUtilities.ConvertValue<TSourceArg, TThisArg>(r), ConversionUtilities.ConvertValue<TSourceArg, TThisArg>(g), ConversionUtilities.ConvertValue<TSourceArg, TThisArg>(b), ConversionUtilities.ConvertValue<TSourceArg, TThisArg>(a));
	}
}
