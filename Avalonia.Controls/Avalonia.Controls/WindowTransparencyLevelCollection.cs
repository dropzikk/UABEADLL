using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Avalonia.Controls;

public class WindowTransparencyLevelCollection : ReadOnlyCollection<WindowTransparencyLevel>
{
	public WindowTransparencyLevelCollection(IList<WindowTransparencyLevel> list)
		: base(list)
	{
	}
}
