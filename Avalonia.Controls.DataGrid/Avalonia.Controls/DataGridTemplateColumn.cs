using Avalonia.Controls.Templates;
using Avalonia.Controls.Utils;
using Avalonia.Interactivity;
using Avalonia.Metadata;

namespace Avalonia.Controls;

public class DataGridTemplateColumn : DataGridColumn
{
	private IDataTemplate _cellTemplate;

	public static readonly DirectProperty<DataGridTemplateColumn, IDataTemplate> CellTemplateProperty = AvaloniaProperty.RegisterDirect("CellTemplate", (DataGridTemplateColumn o) => o.CellTemplate, delegate(DataGridTemplateColumn o, IDataTemplate v)
	{
		o.CellTemplate = v;
	});

	private IDataTemplate _cellEditingCellTemplate;

	public static readonly DirectProperty<DataGridTemplateColumn, IDataTemplate> CellEditingTemplateProperty = AvaloniaProperty.RegisterDirect("CellEditingTemplate", (DataGridTemplateColumn o) => o.CellEditingTemplate, delegate(DataGridTemplateColumn o, IDataTemplate v)
	{
		o.CellEditingTemplate = v;
	});

	private bool _forceGenerateCellFromTemplate;

	[Content]
	[InheritDataTypeFromItems("ItemsSource", AncestorType = typeof(DataGrid))]
	public IDataTemplate CellTemplate
	{
		get
		{
			return _cellTemplate;
		}
		set
		{
			SetAndRaise(CellTemplateProperty, ref _cellTemplate, value);
		}
	}

	[InheritDataTypeFromItems("ItemsSource", AncestorType = typeof(DataGrid))]
	public IDataTemplate CellEditingTemplate
	{
		get
		{
			return _cellEditingCellTemplate;
		}
		set
		{
			SetAndRaise(CellEditingTemplateProperty, ref _cellEditingCellTemplate, value);
		}
	}

	public override bool IsReadOnly
	{
		get
		{
			if (CellEditingTemplate == null)
			{
				return true;
			}
			return base.IsReadOnly;
		}
		set
		{
			base.IsReadOnly = value;
		}
	}

	protected override void EndCellEdit()
	{
		_forceGenerateCellFromTemplate = true;
		base.EndCellEdit();
	}

	protected override Control GenerateElement(DataGridCell cell, object dataItem)
	{
		if (CellTemplate != null)
		{
			if (_forceGenerateCellFromTemplate)
			{
				_forceGenerateCellFromTemplate = false;
				return CellTemplate.Build(dataItem);
			}
			if (!(CellTemplate is IRecyclingDataTemplate recyclingDataTemplate))
			{
				return CellTemplate.Build(dataItem);
			}
			return recyclingDataTemplate.Build(dataItem, cell.Content as Control);
		}
		if (Design.IsDesignMode)
		{
			return null;
		}
		throw DataGridError.DataGridTemplateColumn.MissingTemplateForType(typeof(DataGridTemplateColumn));
	}

	protected override Control GenerateEditingElement(DataGridCell cell, object dataItem, out ICellEditBinding binding)
	{
		binding = null;
		if (CellEditingTemplate != null)
		{
			return CellEditingTemplate.Build(dataItem);
		}
		if (CellTemplate != null)
		{
			return CellTemplate.Build(dataItem);
		}
		if (Design.IsDesignMode)
		{
			return null;
		}
		throw DataGridError.DataGridTemplateColumn.MissingTemplateForType(typeof(DataGridTemplateColumn));
	}

	protected override object PrepareCellForEdit(Control editingElement, RoutedEventArgs editingEventArgs)
	{
		return null;
	}

	protected internal override void RefreshCellContent(Control element, string propertyName)
	{
		DataGridCell dataGridCell = element.Parent as DataGridCell;
		if (propertyName == "CellTemplate" && dataGridCell != null)
		{
			dataGridCell.Content = GenerateElement(dataGridCell, dataGridCell.DataContext);
		}
		base.RefreshCellContent(element, propertyName);
	}
}
