using System;
using System.Globalization;
using Avalonia.Controls.Metadata;
using Avalonia.Input;

namespace Avalonia.Controls.Primitives;

[PseudoClasses(new string[] { ":pressed", ":disabled", ":selected", ":inactive", ":today", ":blackout", ":dayfocused" })]
public sealed class CalendarDayButton : Button
{
	private const int DefaultContent = 1;

	private bool _isCurrent;

	private bool _ignoringMouseOverState;

	private bool _isBlackout;

	private bool _isToday;

	private bool _isInactive;

	private bool _isSelected;

	internal Calendar? Owner { get; set; }

	internal int Index { get; set; }

	internal bool IsCurrent
	{
		get
		{
			return _isCurrent;
		}
		set
		{
			if (_isCurrent != value)
			{
				_isCurrent = value;
				SetPseudoClasses();
			}
		}
	}

	internal bool IsBlackout
	{
		get
		{
			return _isBlackout;
		}
		set
		{
			if (_isBlackout != value)
			{
				_isBlackout = value;
				SetPseudoClasses();
			}
		}
	}

	internal bool IsToday
	{
		get
		{
			return _isToday;
		}
		set
		{
			if (_isToday != value)
			{
				_isToday = value;
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

	public event EventHandler<PointerPressedEventArgs>? CalendarDayButtonMouseDown;

	public event EventHandler<PointerReleasedEventArgs>? CalendarDayButtonMouseUp;

	public CalendarDayButton()
	{
		SetCurrentValue(ContentControl.ContentProperty, 1.ToString(CultureInfo.CurrentCulture));
	}

	internal void IgnoreMouseOverState()
	{
		_ignoringMouseOverState = false;
		if (base.IsPointerOver)
		{
			_ignoringMouseOverState = true;
			SetPseudoClasses();
		}
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		SetPseudoClasses();
	}

	private void SetPseudoClasses()
	{
		if (_ignoringMouseOverState)
		{
			base.PseudoClasses.Set(":pressed", base.IsPressed);
			base.PseudoClasses.Set(":disabled", !base.IsEnabled);
		}
		base.PseudoClasses.Set(":selected", IsSelected);
		base.PseudoClasses.Set(":inactive", IsInactive);
		base.PseudoClasses.Set(":today", IsToday);
		base.PseudoClasses.Set(":blackout", IsBlackout);
		base.PseudoClasses.Set(":dayfocused", IsCurrent && base.IsEnabled);
	}

	protected override void OnPointerPressed(PointerPressedEventArgs e)
	{
		base.OnPointerPressed(e);
		if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
		{
			this.CalendarDayButtonMouseDown?.Invoke(this, e);
		}
	}

	protected override void OnPointerReleased(PointerReleasedEventArgs e)
	{
		base.OnPointerReleased(e);
		if (e.InitialPressMouseButton == MouseButton.Left)
		{
			this.CalendarDayButtonMouseUp?.Invoke(this, e);
		}
	}
}
