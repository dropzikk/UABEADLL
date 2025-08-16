using System.Collections.Generic;
using Avalonia.Controls.Primitives;
using Avalonia.VisualTree;

namespace Avalonia.Controls.Templates;

public static class TemplateExtensions
{
	public static IEnumerable<Control> GetTemplateChildren(this TemplatedControl control)
	{
		foreach (Control templateChild in GetTemplateChildren(control, control))
		{
			yield return templateChild;
		}
	}

	private static IEnumerable<Control> GetTemplateChildren(Control control, TemplatedControl templatedParent)
	{
		foreach (Control child in control.GetVisualChildren())
		{
			AvaloniaObject childTemplatedParent = child.TemplatedParent;
			if (childTemplatedParent == templatedParent)
			{
				yield return child;
			}
			if (childTemplatedParent == null)
			{
				continue;
			}
			foreach (Control templateChild in GetTemplateChildren(child, templatedParent))
			{
				yield return templateChild;
			}
		}
	}
}
