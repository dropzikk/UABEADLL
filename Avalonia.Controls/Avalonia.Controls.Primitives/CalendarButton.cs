using System;
using Avalonia.Controls.Metadata;
using Avalonia.Input;

namespace Avalonia.Controls.Primitives;

[PseudoClasses(new string[] { ":selected", ":inactive", ":btnfocused" })]
public sealed class CalendarButton : Button
{
	private bool _isCalendarButtonFocused;

	private bool _isInactive;

	private bool _isSelected;

	internal Calendar? Owner { get; set; }

	internal bool IsCalendarButtonFocused
	{
		get
		{
			return _isCalendarButtonFocused;
		}
		set
		{
			if (_isCalendarButtonFocused != value)
			{
				_isCalendarButtonFocused = value;
				SetPseudoClasses();
			}
		}
	}

	internal bool IsInactive
	{
		get
		{
			return _isInactive;
		}
		set
		{
			if (_isInactive != value)
			{
				_isInactive = value;
				SetPseudoClasses();
			}
		}
	}

	internal bool IsSelected
	{
		get
		{
			return _isSelected;
		}
		set
		{
			if (_isSelected != value)
			{
				_isSelected = value;
				SetPseudoClasses();
			}
		}
	}

	public event EventHandler<PointerPressedEventArgs>? CalendarLeftMouseButtonDown;

	public event EventHandler<PointerReleasedEventArgs>? CalendarLeftMouseButtonUp;

	public CalendarButton()
	{
		SetCurrentValue(ContentControl.ContentProperty, DateTimeHelper.GetCurrentDateFormat().AbbreviatedMonthNames[0]);
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		SetPseudoClasses();
	}

	private void SetPseudoClasses()
	{
		base.PseudoClasses.Set(":selected", IsSelected);
		base.PseudoClasses.Set(":inactive", IsInactive);
		base.PseudoClasses.Set(":btnfocused", IsCalendarButtonFocused && base.IsEnabled);
	}

	protected override void OnPointerPressed(PointerPressedEventArgs e)
	{
		base.OnPointerPressed(e);
		if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
		{
			this.CalendarLeftMouseButtonDown?.Invoke(this, e);
		}
	}

	protected override void OnPointerReleased(PointerReleasedEventArgs e)
	{
		base.OnPointerReleased(e);
		if (e.InitialPressMouseButton == MouseButton.Left)
		{
			this.CalendarLeftMouseButtonUp?.Invoke(this, e);
		}
	}
}
