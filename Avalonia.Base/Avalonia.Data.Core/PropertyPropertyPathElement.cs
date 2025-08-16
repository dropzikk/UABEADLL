namespace Avalonia.Data.Core;

public class PropertyPropertyPathElement : IPropertyPathElement
{
	public IPropertyInfo Property { get; }

	public PropertyPropertyPathElement(IPropertyInfo property)
	{
		Property = property;
	}
}
