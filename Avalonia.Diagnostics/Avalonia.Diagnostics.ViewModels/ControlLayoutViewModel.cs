using System;
using System.ComponentModel;
using System.Text;
using Avalonia.Controls;
using Avalonia.Layout;

namespace Avalonia.Diagnostics.ViewModels;

internal class ControlLayoutViewModel : ViewModelBase
{
	private readonly Visual _control;

	private Thickness _borderThickness;

	private double _height;

	private string? _heightConstraint;

	private HorizontalAlignment _horizontalAlignment;

	private Thickness _marginThickness;

	private Thickness _paddingThickness;

	private bool _updatingFromControl;

	private VerticalAlignment _verticalAlignment;

	private double _width;

	private string? _widthConstraint;

	public Thickness MarginThickness
	{
		get
		{
			return _marginThickness;
		}
		set
		{
			RaiseAndSetIfChanged(ref _marginThickness, value, "MarginThickness");
		}
	}

	public Thickness BorderThickness
	{
		get
		{
			return _borderThickness;
		}
		set
		{
			RaiseAndSetIfChanged(ref _borderThickness, value, "BorderThickness");
		}
	}

	public Thickness PaddingThickness
	{
		get
		{
			return _paddingThickness;
		}
		set
		{
			RaiseAndSetIfChanged(ref _paddingThickness, value, "PaddingThickness");
		}
	}

	public double Width
	{
		get
		{
			return _width;
		}
		private set
		{
			RaiseAndSetIfChanged(ref _width, value, "Width");
		}
	}

	public double Height
	{
		get
		{
			return _height;
		}
		private set
		{
			RaiseAndSetIfChanged(ref _height, value, "Height");
		}
	}

	public string? WidthConstraint
	{
		get
		{
			return _widthConstraint;
		}
		private set
		{
			RaiseAndSetIfChanged(ref _widthConstraint, value, "WidthConstraint");
		}
	}

	public string? HeightConstraint
	{
		get
		{
			return _heightConstraint;
		}
		private set
		{
			RaiseAndSetIfChanged(ref _heightConstraint, value, "HeightConstraint");
		}
	}

	public HorizontalAlignment HorizontalAlignment
	{
		get
		{
			return _horizontalAlignment;
		}
		set
		{
			RaiseAndSetIfChanged(ref _horizontalAlignment, value, "HorizontalAlignment");
		}
	}

	public VerticalAlignment VerticalAlignment
	{
		get
		{
			return _verticalAlignment;
		}
		set
		{
			RaiseAndSetIfChanged(ref _verticalAlignment, value, "VerticalAlignment");
		}
	}

	public bool HasPadding { get; }

	public bool HasBorder { get; }

	public ControlLayoutViewModel(Visual control)
	{
		_control = control;
		HasPadding = AvaloniaPropertyRegistry.Instance.IsRegistered(control, Decorator.PaddingProperty);
		HasBorder = AvaloniaPropertyRegistry.Instance.IsRegistered(control, Border.BorderThicknessProperty);
		if (control != null)
		{
			try
			{
				_updatingFromControl = true;
				MarginThickness = control.GetValue(Layoutable.MarginProperty);
				if (HasPadding)
				{
					PaddingThickness = control.GetValue(Decorator.PaddingProperty);
				}
				if (HasBorder)
				{
					BorderThickness = control.GetValue(Border.BorderThicknessProperty);
				}
				HorizontalAlignment = control.GetValue(Layoutable.HorizontalAlignmentProperty);
				VerticalAlignment = control.GetValue(Layoutable.VerticalAlignmentProperty);
			}
			finally
			{
				_updatingFromControl = false;
			}
		}
		UpdateSize();
		UpdateSizeConstraints();
	}

	private void UpdateSizeConstraints()
	{
		Visual control = _control;
		AvaloniaObject ao = control;
		if (ao != null)
		{
			WidthConstraint = CreateConstraintInfo(Layoutable.MinWidthProperty, Layoutable.MaxWidthProperty);
			HeightConstraint = CreateConstraintInfo(Layoutable.MinHeightProperty, Layoutable.MaxHeightProperty);
		}
		string? CreateConstraintInfo(StyledProperty<double> minProperty, StyledProperty<double> maxProperty)
		{
			bool flag = ao.IsSet(minProperty);
			bool flag2 = ao.IsSet(maxProperty);
			if (flag || flag2)
			{
				StringBuilder stringBuilder = new StringBuilder();
				if (flag)
				{
					double value = ao.GetValue(minProperty);
					stringBuilder.AppendFormat("Min: {0}", Math.Round(value, 2));
					stringBuilder.AppendLine();
				}
				if (flag2)
				{
					double value2 = ao.GetValue(maxProperty);
					stringBuilder.AppendFormat("Max: {0}", Math.Round(value2, 2));
				}
				return stringBuilder.ToString();
			}
			return null;
		}
	}

	protected override void OnPropertyChanged(PropertyChangedEventArgs e)
	{
		base.OnPropertyChanged(e);
		if (_updatingFromControl)
		{
			return;
		}
		AvaloniaObject control = _control;
		if (control != null)
		{
			if (e.PropertyName == "MarginThickness")
			{
				control.SetValue(Layoutable.MarginProperty, MarginThickness);
			}
			else if (HasPadding && e.PropertyName == "PaddingThickness")
			{
				control.SetValue(Decorator.PaddingProperty, PaddingThickness);
			}
			else if (HasBorder && e.PropertyName == "BorderThickness")
			{
				control.SetValue(Border.BorderThicknessProperty, BorderThickness);
			}
			else if (e.PropertyName == "HorizontalAlignment")
			{
				control.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment);
			}
			else if (e.PropertyName == "VerticalAlignment")
			{
				control.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment);
			}
		}
	}

	public void ControlPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
	{
		try
		{
			_updatingFromControl = true;
			if (e.Property == Visual.BoundsProperty)
			{
				UpdateSize();
				return;
			}
			AvaloniaObject control = _control;
			if (control != null)
			{
				if (e.Property == Layoutable.MarginProperty)
				{
					MarginThickness = control.GetValue(Layoutable.MarginProperty);
				}
				else if (e.Property == Decorator.PaddingProperty)
				{
					PaddingThickness = control.GetValue(Decorator.PaddingProperty);
				}
				else if (e.Property == Border.BorderThicknessProperty)
				{
					BorderThickness = control.GetValue(Border.BorderThicknessProperty);
				}
				else if (e.Property == Layoutable.MinWidthProperty || e.Property == Layoutable.MaxWidthProperty || e.Property == Layoutable.MinHeightProperty || e.Property == Layoutable.MaxHeightProperty)
				{
					UpdateSizeConstraints();
				}
				else if (e.Property == Layoutable.HorizontalAlignmentProperty)
				{
					HorizontalAlignment = control.GetValue(Layoutable.HorizontalAlignmentProperty);
				}
				else if (e.Property == Layoutable.VerticalAlignmentProperty)
				{
					VerticalAlignment = control.GetValue(Layoutable.VerticalAlignmentProperty);
				}
			}
		}
		finally
		{
			_updatingFromControl = false;
		}
	}

	private void UpdateSize()
	{
		Rect bounds = _control.Bounds;
		Width = Math.Round(bounds.Width, 2);
		Height = Math.Round(bounds.Height, 2);
	}
}
