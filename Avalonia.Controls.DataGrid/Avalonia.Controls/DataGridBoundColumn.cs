using System;
using Avalonia.Controls.Utils;
using Avalonia.Data;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Metadata;
using Avalonia.Reactive;

namespace Avalonia.Controls;

public abstract class DataGridBoundColumn : DataGridColumn
{
	private IBinding _binding;

	[AssignBinding]
	[InheritDataTypeFromItems("ItemsSource", AncestorType = typeof(DataGrid))]
	public virtual IBinding Binding
	{
		get
		{
			return _binding;
		}
		set
		{
			if (_binding == value)
			{
				return;
			}
			if (base.OwningGrid != null && !base.OwningGrid.CommitEdit(DataGridEditingUnit.Row, exitEditingMode: true))
			{
				base.OwningGrid.CancelEdit(DataGridEditingUnit.Row, raiseEvents: false);
			}
			_binding = value;
			if (_binding != null)
			{
				if (_binding is BindingBase bindingBase)
				{
					if (bindingBase.Mode == BindingMode.OneWayToSource)
					{
						throw new InvalidOperationException("DataGridColumn doesn't support BindingMode.OneWayToSource. Use BindingMode.TwoWay instead.");
					}
					if (!string.IsNullOrEmpty((bindingBase as Binding)?.Path ?? (bindingBase as CompiledBindingExtension)?.Path.ToString()) && bindingBase.Mode == BindingMode.Default)
					{
						bindingBase.Mode = BindingMode.TwoWay;
					}
					if (bindingBase.Converter == null && string.IsNullOrEmpty(bindingBase.StringFormat))
					{
						bindingBase.Converter = DataGridValueConverter.Instance;
					}
				}
				if (base.OwningGrid != null)
				{
					base.OwningGrid.OnColumnBindingChanged(this);
				}
			}
			RemoveEditingElement();
		}
	}

	public override IBinding ClipboardContentBinding
	{
		get
		{
			return base.ClipboardContentBinding ?? Binding;
		}
		set
		{
			base.ClipboardContentBinding = value;
		}
	}

	protected AvaloniaProperty BindingTarget { get; set; }

	protected sealed override Control GenerateEditingElement(DataGridCell cell, object dataItem, out ICellEditBinding editBinding)
	{
		Control control = GenerateEditingElementDirect(cell, dataItem);
		editBinding = null;
		if (Binding != null)
		{
			editBinding = BindEditingElement(control, BindingTarget, Binding);
		}
		return control;
	}

	private static ICellEditBinding BindEditingElement(AvaloniaObject target, AvaloniaProperty property, IBinding binding)
	{
		InstancedBinding instancedBinding = binding.Initiate(target, property, null, enableDataValidation: true);
		if (instancedBinding != null)
		{
			if (instancedBinding.Source is IAvaloniaSubject<object> bindingSourceSubject)
			{
				CellEditBinding cellEditBinding = new CellEditBinding(bindingSourceSubject);
				InstancedBinding binding2 = new InstancedBinding(cellEditBinding.InternalSubject, instancedBinding.Mode, instancedBinding.Priority);
				BindingOperations.Apply(target, property, binding2, null);
				return cellEditBinding;
			}
			BindingOperations.Apply(target, property, instancedBinding, null);
		}
		return null;
	}

	protected abstract Control GenerateEditingElementDirect(DataGridCell cell, object dataItem);

	internal void SetHeaderFromBinding()
	{
		if (base.OwningGrid == null || !(base.OwningGrid.DataConnection.DataType != null) || base.Header != null || Binding == null || !(Binding is BindingBase bindingBase))
		{
			return;
		}
		string text = (bindingBase as Binding)?.Path ?? (bindingBase as CompiledBindingExtension)?.Path.ToString();
		if (!string.IsNullOrWhiteSpace(text))
		{
			string displayName = base.OwningGrid.DataConnection.DataType.GetDisplayName(text);
			if (displayName != null)
			{
				base.Header = displayName;
			}
		}
	}
}
