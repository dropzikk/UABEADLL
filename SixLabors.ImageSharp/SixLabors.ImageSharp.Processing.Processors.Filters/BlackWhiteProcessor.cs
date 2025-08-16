namespace SixLabors.ImageSharp.Processing.Processors.Filters;

public sealed class BlackWhiteProcessor : FilterProcessor
{
	public BlackWhiteProcessor()
		: base(KnownFilterMatrices.BlackWhiteFilter)
	{
	}
}
