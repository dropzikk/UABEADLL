using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;

namespace Avalonia.Markup.Xaml.Templates;

public static class TemplateContent
{
	public static TemplateResult<Control>? Load(object? templateContent)
	{
		if (templateContent is Func<IServiceProvider, object> func)
		{
			return (TemplateResult<Control>)func(null);
		}
		if (templateContent == null)
		{
			return null;
		}
		throw new ArgumentException($"Unexpected content {templateContent.GetType()}", "templateContent");
	}

	public static TemplateResult<T>? Load<T>(object? templateContent)
	{
		if (templateContent is Func<IServiceProvider, object> func)
		{
			return (TemplateResult<T>)func(null);
		}
		if (templateContent == null)
		{
			return null;
		}
		throw new ArgumentException($"Unexpected content {templateContent.GetType()}", "templateContent");
	}
}
