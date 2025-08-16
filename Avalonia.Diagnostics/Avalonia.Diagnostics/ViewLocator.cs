using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Diagnostics.ViewModels;

namespace Avalonia.Diagnostics;

internal class ViewLocator : IDataTemplate, ITemplate<object?, Control?>
{
	public Control? Build(object? data)
	{
		if (data == null)
		{
			return null;
		}
		string text = data.GetType().FullName.Replace("ViewModel", "View");
		Type type = Type.GetType(text);
		if (type != null)
		{
			return (Control)Activator.CreateInstance(type);
		}
		return new TextBlock
		{
			Text = text
		};
	}

	public bool Match(object? data)
	{
		return data is ViewModelBase;
	}
}
