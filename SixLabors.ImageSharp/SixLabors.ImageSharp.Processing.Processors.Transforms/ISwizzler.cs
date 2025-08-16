namespace SixLabors.ImageSharp.Processing.Processors.Transforms;

public interface ISwizzler
{
	Size DestinationSize { get; }

	Point Transform(Point point);
}
