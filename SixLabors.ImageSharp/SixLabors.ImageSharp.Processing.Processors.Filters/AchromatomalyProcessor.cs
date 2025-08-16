namespace SixLabors.ImageSharp.Processing.Processors.Filters;

public sealed class AchromatomalyProcessor : FilterProcessor
{
	public AchromatomalyProcessor()
		: base(KnownFilterMatrices.AchromatomalyFilter)
	{
	}
}
