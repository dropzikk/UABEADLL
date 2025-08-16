namespace Avalonia.Media.Immutable;

public class ImmutableTransform : ITransform
{
	public Matrix Value { get; }

	public ImmutableTransform(Matrix matrix)
	{
		Value = matrix;
	}
}
