using System;
using Avalonia.Animation;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;

namespace Avalonia.Controls;

[TemplatePart("PART_MovingKnobs", typeof(Panel))]
[TemplatePart("PART_OffContentPresenter", typeof(ContentPresenter))]
[TemplatePart("PART_OnContentPresenter", typeof(ContentPresenter))]
[TemplatePart("PART_SwitchKnob", typeof(Panel))]
[PseudoClasses(new string[] { ":dragging" })]
public class ToggleSwitch : ToggleButton
{
	private Panel? _knobsPanel;

	private Panel? _switchKnob;

	private bool _knobsPanelPressed;

	private Point _switchStartPoint;

	private double _initLeft = -1.0;

	private bool _isDragging;

	public static readonly StyledProperty<object?> OffContentProperty;

	public static readonly StyledProperty<IDataTemplate?> OffContentTemplateProperty;

	public static readonly StyledProperty<object?> OnContentProperty;

	public static readonly StyledProperty<IDataTemplate?> OnContentTemplateProperty;

	public static readonly StyledProperty<Transitions> KnobTransitionsProperty;

	public object? OnContent
	{
		get
		{
			return GetValue(OnContentProperty);
		}
		set
		{
			SetValue(OnContentProperty, value);
		}
	}

	public object? OffContent
	{
		get
		{
			return GetValue(OffContentProperty);
		}
		set
		{
			SetValue(OffContentProperty, value);
		}
	}

	public ContentPresenter? OffContentPresenter { get; private set; }

	public ContentPresenter? OnContentPresenter { get; private set; }

	public IDataTemplate? OffContentTemplate
	{
		get
		{
			return GetValue(OffContentTemplateProperty);
		}
		set
		{
			SetValue(OffContentTemplateProperty, value);
		}
	}

	public IDataTemplate? OnContentTemplate
	{
		get
		{
			return GetValue(OnContentTemplateProperty);
		}
		set
		{
			SetValue(OnContentTemplateProperty, value);
		}
	}

	public Transitions KnobTransitions
	{
		get
		{
			return GetValue(KnobTransitionsProperty);
		}
		set
		{
			SetValue(KnobTransitionsProperty, value);
		}
	}

	static ToggleSwitch()
	{
		OffContentProperty = AvaloniaProperty.Register<ToggleSwitch, object>("OffContent", "Off");
		OffContentTemplateProperty = AvaloniaProperty.Register<ToggleSwitch, IDataTemplate>("OffContentTemplate");
		OnContentProperty = AvaloniaProperty.Register<ToggleSwitch, object>("OnContent", "On");
		OnContentTemplateProperty = AvaloniaProperty.Register<ToggleSwitch, IDataTemplate>("OnContentTemplate");
		KnobTransitionsProperty = AvaloniaProperty.Register<ToggleSwitch, Transitions>("KnobTransitions");
		OffContentProperty.Changed.AddClassHandler(delegate(ToggleSwitch x, AvaloniaPropertyChangedEventArgs e)
		{
			x.OffContentChanged(e);
		});
		OnContentProperty.Changed.AddClassHandler(delegate(ToggleSwitch x, AvaloniaPropertyChangedEventArgs e)
		{
			x.OnContentChanged(e);
		});
		ToggleButton.IsCheckedProperty.Changed.AddClassHandler(delegate(ToggleSwitch x, AvaloniaPropertyChangedEventArgs e)
		{
			if (e.NewValue != null && e.NewValue is bool value)
			{
				x.UpdateKnobPos(value);
			}
		});
		Visual.BoundsProperty.Changed.AddClassHandler(delegate(ToggleSwitch x, AvaloniaPropertyChangedEventArgs e)
		{
			if (x.IsChecked.HasValue)
			{
				x.UpdateKnobPos(x.IsChecked.Value);
			}
		});
		KnobTransitionsProperty.Changed.AddClassHandler(delegate(ToggleSwitch x, AvaloniaPropertyChangedEventArgs e)
		{
			x.UpdateKnobTransitions();
		});
	}

	private void OffContentChanged(AvaloniaPropertyChangedEventArgs e)
	{
		if (e.OldValue is ILogical item)
		{
			base.LogicalChildren.Remove(item);
		}
		if (e.NewValue is ILogical item2)
		{
			base.LogicalChildren.Add(item2);
		}
	}

	private void OnContentChanged(AvaloniaPropertyChangedEventArgs e)
	{
		if (e.OldValue is ILogical item)
		{
			base.LogicalChildren.Remove(item);
		}
		if (e.NewValue is ILogical item2)
		{
			base.LogicalChildren.Add(item2);
		}
	}

	protected override bool RegisterContentPresenter(ContentPresenter presenter)
	{
		bool result = base.RegisterContentPresenter(presenter);
		if (presenter.Name == "Part_OnContentPresenter")
		{
			OnContentPresenter = presenter;
			result = true;
		}
		else if (presenter.Name == "PART_OffContentPresenter")
		{
			OffContentPresenter = presenter;
			result = true;
		}
		return result;
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		base.OnApplyTemplate(e);
		_switchKnob = e.NameScope.Find<Panel>("PART_SwitchKnob");
		_knobsPanel = e.NameScope.Get<Panel>("PART_MovingKnobs");
		_knobsPanel.PointerPressed += KnobsPanel_PointerPressed;
		_knobsPanel.PointerReleased += KnobsPanel_PointerReleased;
		_knobsPanel.PointerMoved += KnobsPanel_PointerMoved;
		if (base.IsChecked.HasValue)
		{
			UpdateKnobPos(base.IsChecked.Value);
		}
	}

	protected override void OnLoaded(RoutedEventArgs e)
	{
		base.OnLoaded(e);
		UpdateKnobTransitions();
	}

	private void UpdateKnobTransitions()
	{
		if (_knobsPanel != null)
		{
			_knobsPanel.Transitions = KnobTransitions;
		}
	}

	private void KnobsPanel_PointerPressed(object? sender, PointerPressedEventArgs e)
	{
		_switchStartPoint = e.GetPosition(_switchKnob);
		_initLeft = Canvas.GetLeft(_knobsPanel);
		_isDragging = false;
		_knobsPanelPressed = true;
	}

	private void KnobsPanel_PointerReleased(object? sender, PointerReleasedEventArgs e)
	{
		if (_isDragging)
		{
			bool flag = Canvas.GetLeft(_knobsPanel) >= _switchKnob.Bounds.Width / 2.0;
			_knobsPanel.ClearValue(Canvas.LeftProperty);
			base.PseudoClasses.Set(":dragging", value: false);
			if (flag == base.IsChecked)
			{
				UpdateKnobPos(flag);
			}
			else
			{
				SetCurrentValue(ToggleButton.IsCheckedProperty, flag);
			}
			UpdateKnobTransitions();
		}
		else
		{
			base.Toggle();
		}
		_isDragging = false;
		_knobsPanelPressed = false;
	}

	private void KnobsPanel_PointerMoved(object? sender, PointerEventArgs e)
	{
		if (_knobsPanelPressed)
		{
			if (_knobsPanel != null)
			{
				_knobsPanel.Transitions = null;
			}
			Point point = e.GetPosition(_switchKnob) - _switchStartPoint;
			if (!_isDragging && Math.Abs(point.X) > 3.0)
			{
				_isDragging = true;
				base.PseudoClasses.Set(":dragging", value: true);
			}
			if (_isDragging)
			{
				Canvas.SetLeft(_knobsPanel, Math.Min(_switchKnob.Bounds.Width, Math.Max(0.0, _initLeft + point.X)));
			}
		}
	}

	protected override void Toggle()
	{
		if (_switchKnob != null && !_switchKnob.IsPointerOver)
		{
			base.Toggle();
		}
	}

	private void UpdateKnobPos(bool value)
	{
		if (_switchKnob != null && _knobsPanel != null)
		{
			if (value)
			{
				Canvas.SetLeft(_knobsPanel, _switchKnob.Bounds.Width);
			}
			else
			{
				Canvas.SetLeft(_knobsPanel, 0.0);
			}
		}
	}
}
