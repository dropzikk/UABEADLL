using System;
using Avalonia.Data;
using Avalonia.Input.Platform;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media;

namespace Avalonia.Diagnostics.ViewModels;

internal class BindingSetterViewModel : SetterViewModel
{
	public IBrush Tint { get; }

	public string ValueTypeTooltip { get; }

	public string Path { get; }

	public BindingSetterViewModel(AvaloniaProperty property, object? value, IClipboard? clipboard)
		: base(property, value, clipboard)
	{
		if (!(value is Binding binding))
		{
			if (!(value is CompiledBindingExtension compiledBindingExtension))
			{
				if (!(value is TemplateBinding templateBinding))
				{
					throw new ArgumentException("Invalid binding type", "value");
				}
				AvaloniaProperty property2 = templateBinding.Property;
				if ((object)property2 != null)
				{
					Path = property2.OwnerType.Name + "." + property2.Name;
				}
				else
				{
					Path = "Unassigned";
				}
				Tint = Brushes.OrangeRed;
				ValueTypeTooltip = "Template Binding";
			}
			else
			{
				Path = compiledBindingExtension.Path.ToString();
				Tint = Brushes.DarkGreen;
				ValueTypeTooltip = "Compiled Binding";
			}
		}
		else
		{
			Path = binding.Path;
			Tint = Brushes.CornflowerBlue;
			ValueTypeTooltip = "Reflection Binding";
		}
	}

	public override void CopyValue()
	{
		CopyToClipboard(Path);
	}
}
