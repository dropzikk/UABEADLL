using System.Collections.Generic;

namespace AvaloniaEdit;

public interface IRoutedCommandBindable
{
	IList<RoutedCommandBinding> CommandBindings { get; }
}
