using System;
using System.Text;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Utilities;

namespace Avalonia.Diagnostics;

public static class VisualTreeDebug
{
	public static string PrintVisualTree(Visual visual)
	{
		StringBuilder stringBuilder = StringBuilderCache.Acquire();
		PrintVisualTree(visual, stringBuilder, 0);
		return StringBuilderCache.GetStringAndRelease(stringBuilder);
	}

	private static void PrintVisualTree(Visual visual, StringBuilder builder, int indent)
	{
		Control control = visual as Control;
		builder.Append(Indent(indent - 1));
		if (indent > 0)
		{
			builder.Append(" +- ");
		}
		builder.Append(visual.GetType().Name);
		if (control != null)
		{
			builder.Append(" ");
			builder.AppendLine(control.Classes.ToString());
			foreach (AvaloniaProperty item in AvaloniaPropertyRegistry.Instance.GetRegistered(control))
			{
				AvaloniaPropertyValue diagnostic = control.GetDiagnostic(item);
				if (diagnostic.Priority != BindingPriority.Unset)
				{
					builder.Append(Indent(indent));
					builder.Append(" |  ");
					builder.Append(diagnostic.Property.Name);
					builder.Append(" = ");
					builder.Append(diagnostic.Value ?? "(null)");
					builder.Append(" [");
					builder.Append(diagnostic.Priority);
					builder.AppendLine("]");
				}
			}
		}
		else
		{
			builder.AppendLine();
		}
		foreach (Visual visualChild in visual.VisualChildren)
		{
			PrintVisualTree(visualChild, builder, indent + 1);
		}
	}

	private static string Indent(int indent)
	{
		return new string(' ', Math.Max(indent, 0) * 4);
	}
}
