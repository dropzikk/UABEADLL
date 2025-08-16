namespace Avalonia.Media;

public interface IMutableEffect : IEffect
{
	internal IImmutableEffect ToImmutable();
}
