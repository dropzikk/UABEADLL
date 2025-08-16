using System.Collections.Generic;
using System.Linq;

namespace Avalonia.Data.Core;

public class PropertyPath
{
	public IReadOnlyList<IPropertyPathElement> Elements { get; }

	public PropertyPath(IEnumerable<IPropertyPathElement> elements)
	{
		Elements = elements.ToArray();
	}
}
