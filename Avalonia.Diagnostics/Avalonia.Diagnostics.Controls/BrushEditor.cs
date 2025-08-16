using System;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Immutable;

namespace Avalonia.Diagnostics.Controls;

[TemplatePart("PART_ClearButton", typeof(Button))]
internal class BrushEditor : TemplatedControl
{
	private readonly EventHandler<RoutedEventArgs> clearHandler;

	private Button? _clearButton;

	private readonly ColorView _colorView = new ColorView
	{
		HexInputAlphaPosition = AlphaComponentPosition.Leading
	};

	public static readonly DirectProperty<BrushEditor, IBrush?> BrushProperty = AvaloniaProperty.RegisterDirect("Brush", (BrushEditor o) => o.Brush, delegate(BrushEditor o, IBrush? v)
	{
		o.Brush = v;
	});

	private IBrush? _brush;

	protected override Type StyleKeyOverride => typeof(BrushEditor);

	public IBrush? Brush
	{
		get
		{
			return _brush;
		}
		set
		{
			SetAndRaise(BrushProperty, ref _brush, value);
		}
	}

	public BrushEditor()
	{
		FlyoutBase.SetAttachedFlyout(this, new Flyout
		{
			Content = _colorView
		});
		_colorView.ColorChanged += delegate(object? _, ColorChangedEventArgs e)
		{
			Brush = new ImmutableSolidColorBrush(e.NewColor);
		};
		clearHandler = delegate
		{
			Brush = null;
		};
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		base.OnApplyTemplate(e);
		if (_clearButton != null)
		{
			_clearButton.Click -= clearHandler;
		}
		_clearButton = e.NameScope.Find<Button>("PART_ClearButton");
		Button clearButton = _clearButton;
		if (clearButton != null)
		{
			clearButton.Click += clearHandler;
		}
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (change.Property == BrushProperty)
		{
			if (Brush is ISolidColorBrush solidColorBrush)
			{
				_colorView.Color = solidColorBrush.Color;
			}
			ToolTip.SetTip(this, Brush?.GetType().Name ?? "(null)");
			InvalidateVisual();
		}
	}

	protected override void OnPointerPressed(PointerPressedEventArgs e)
	{
		base.OnPointerPressed(e);
		FlyoutBase.ShowAttachedFlyout(this);
	}

	public override void Render(DrawingContext context)
	{
		base.Render(context);
		IBrush brush = Brush ?? Brushes.Black;
		context.FillRectangle(brush, base.Bounds);
		FormattedText formattedText = new FormattedText((Brush as ISolidColorBrush)?.Color.ToString() ?? "(null)", CultureInfo.CurrentCulture, FlowDirection.LeftToRight, Typeface.Default, 10.0, GetTextBrush(brush));
		context.DrawText(formattedText, new Point(base.Bounds.Width / 2.0 - formattedText.Width / 2.0, base.Bounds.Height / 2.0 - formattedText.Height / 2.0));
	}

	private static IBrush GetTextBrush(IBrush brush)
	{
		if (brush is ISolidColorBrush solidColorBrush)
		{
			if (!(ColorHelper.GetRelativeLuminance(solidColorBrush.Color) < 0.5))
			{
				return Brushes.Black;
			}
			return Brushes.White;
		}
		return Brushes.White;
	}
}
