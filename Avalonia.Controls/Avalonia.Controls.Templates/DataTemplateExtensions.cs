using Avalonia.LogicalTree;

namespace Avalonia.Controls.Templates;

public static class DataTemplateExtensions
{
	public static IDataTemplate? FindDataTemplate(this Control control, object? data, IDataTemplate? primary = null)
	{
		if (primary != null && primary.Match(data))
		{
			return primary;
		}
		for (ILogical logical = control; logical != null; logical = logical.LogicalParent)
		{
			if (logical is IDataTemplateHost { IsDataTemplatesInitialized: not false } dataTemplateHost)
			{
				foreach (IDataTemplate dataTemplate in dataTemplateHost.DataTemplates)
				{
					if (dataTemplate.Match(data))
					{
						return dataTemplate;
					}
				}
			}
		}
		IGlobalDataTemplates service = AvaloniaLocator.Current.GetService<IGlobalDataTemplates>();
		if (service != null && service.IsDataTemplatesInitialized)
		{
			foreach (IDataTemplate dataTemplate2 in service.DataTemplates)
			{
				if (dataTemplate2.Match(data))
				{
					return dataTemplate2;
				}
			}
		}
		return null;
	}
}
