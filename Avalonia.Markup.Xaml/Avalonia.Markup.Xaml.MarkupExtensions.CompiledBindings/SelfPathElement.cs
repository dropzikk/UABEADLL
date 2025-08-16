namespace Avalonia.Markup.Xaml.MarkupExtensions.CompiledBindings;

internal class SelfPathElement : ICompiledBindingPathElement, IControlSourceBindingPathElement
{
	public static readonly SelfPathElement Instance = new SelfPathElement();

	public override string ToString()
	{
		return "$self";
	}
}
