using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Dialogs.Internal;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;

namespace Avalonia.Dialogs;

[TemplatePart("PART_QuickLinks", typeof(Control))]
[TemplatePart("PART_Files", typeof(ListBox))]
public class ManagedFileChooser : TemplatedControl
{
	private Control _quickLinksRoot;

	private ListBox _filesView;

	private ManagedFileChooserViewModel Model => base.DataContext as ManagedFileChooserViewModel;

	public ManagedFileChooser()
	{
		AddHandler(InputElement.PointerPressedEvent, OnPointerPressed, RoutingStrategies.Tunnel);
	}

	private void OnPointerPressed(object sender, PointerPressedEventArgs e)
	{
		if (!((e.Source as StyledElement)?.DataContext is ManagedFileChooserItemViewModel managedFileChooserItemViewModel) || _quickLinksRoot == null)
		{
			return;
		}
		bool flag = _quickLinksRoot.IsLogicalAncestorOf(e.Source as Control);
		if (e.ClickCount == 2 || flag)
		{
			if (managedFileChooserItemViewModel.ItemType == ManagedFileChooserItemType.File)
			{
				Model?.SelectSingleFile(managedFileChooserItemViewModel);
			}
			else
			{
				Model?.Navigate(managedFileChooserItemViewModel.Path);
			}
			e.Handled = true;
		}
	}

	protected override async void OnDataContextChanged(EventArgs e)
	{
		base.OnDataContextChanged(e);
		if (!(base.DataContext is ManagedFileChooserViewModel model))
		{
			return;
		}
		ManagedFileChooserItemViewModel preselected = model.SelectedItems.FirstOrDefault();
		if (preselected == null)
		{
			return;
		}
		await Task.Delay(100);
		if (preselected == model.SelectedItems.FirstOrDefault())
		{
			int num = model.Items.IndexOf(preselected);
			if (_filesView != null && num > 1)
			{
				_filesView.ScrollIntoView(num - 1);
			}
		}
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		_quickLinksRoot = e.NameScope.Get<Control>("PART_QuickLinks");
		_filesView = e.NameScope.Get<ListBox>("PART_Files");
	}
}
