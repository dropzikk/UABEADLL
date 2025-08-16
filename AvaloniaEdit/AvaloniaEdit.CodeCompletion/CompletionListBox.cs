using System;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using AvaloniaEdit.Utils;

namespace AvaloniaEdit.CodeCompletion;

public class CompletionListBox : ListBox
{
	internal ScrollViewer ScrollViewer;

	public int FirstVisibleItem
	{
		get
		{
			if (ScrollViewer == null || ScrollViewer.Extent.Height == 0.0)
			{
				return 0;
			}
			return (int)((double)base.ItemCount * ScrollViewer.Offset.Y / ScrollViewer.Extent.Height);
		}
		set
		{
			value = value.CoerceValue(0, base.ItemCount - VisibleItemCount);
			if (ScrollViewer != null)
			{
				ScrollViewer.Offset = ScrollViewer.Offset.WithY((double)value / (double)base.ItemCount * ScrollViewer.Extent.Height);
			}
		}
	}

	public int VisibleItemCount
	{
		get
		{
			if (ScrollViewer == null || ScrollViewer.Extent.Height == 0.0)
			{
				return 10;
			}
			return Math.Max(3, (int)Math.Ceiling((double)base.ItemCount * ScrollViewer.Viewport.Height / ScrollViewer.Extent.Height));
		}
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		base.OnApplyTemplate(e);
		ScrollViewer = e.NameScope.Find("PART_ScrollViewer") as ScrollViewer;
	}

	public void ClearSelection()
	{
		base.SelectedIndex = -1;
	}

	public void SelectIndex(int index)
	{
		if (index >= base.ItemCount)
		{
			index = base.ItemCount - 1;
		}
		if (index < 0)
		{
			index = 0;
		}
		base.SelectedIndex = index;
		ScrollIntoView(base.SelectedItem);
	}

	public void CenterViewOn(int index)
	{
		FirstVisibleItem = index - VisibleItemCount / 2;
	}
}
