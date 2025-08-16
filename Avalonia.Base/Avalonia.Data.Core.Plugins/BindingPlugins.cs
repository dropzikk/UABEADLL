using System.Collections.Generic;

namespace Avalonia.Data.Core.Plugins;

public static class BindingPlugins
{
	public static IList<IPropertyAccessorPlugin> PropertyAccessors => ExpressionObserver.PropertyAccessors;

	public static IList<IDataValidationPlugin> DataValidators => ExpressionObserver.DataValidators;

	public static IList<IStreamPlugin> StreamHandlers => ExpressionObserver.StreamHandlers;
}
