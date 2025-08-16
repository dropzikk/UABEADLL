namespace Avalonia.Markup.Xaml;

public interface IRootObjectProvider
{
	object RootObject { get; }

	object IntermediateRootObject { get; }
}
