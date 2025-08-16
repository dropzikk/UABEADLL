using System.Runtime.CompilerServices;

namespace SixLabors.ImageSharp;

internal static class InliningOptions
{
	public const MethodImplOptions AlwaysInline = MethodImplOptions.AggressiveInlining;

	public const MethodImplOptions HotPath = MethodImplOptions.AggressiveOptimization;

	public const MethodImplOptions ShortMethod = MethodImplOptions.AggressiveInlining;

	public const MethodImplOptions ColdPath = MethodImplOptions.NoInlining;
}
