namespace SixLabors.ImageSharp.Processing.Processors.Filters;

public sealed class ProtanomalyProcessor : FilterProcessor
{
	public ProtanomalyProcessor()
		: base(KnownFilterMatrices.ProtanomalyFilter)
	{
	}
}
