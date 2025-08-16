using System;
using Avalonia.Collections;

namespace Avalonia.Controls.Templates;

public class DataTemplates : AvaloniaList<IDataTemplate>
{
	public DataTemplates()
	{
		base.ResetBehavior = ResetBehavior.Remove;
		base.Validate = (Action<IDataTemplate>)Delegate.Combine(base.Validate, new Action<IDataTemplate>(ValidateDataTemplate));
	}

	private static void ValidateDataTemplate(IDataTemplate template)
	{
		if (template is ITypedDataTemplate typedDataTemplate && (object)typedDataTemplate.DataType == null)
		{
			throw new InvalidOperationException("DataTemplate inside of DataTemplates must have a DataType set. Set DataType property or use ItemTemplate with single template instead.");
		}
	}
}
