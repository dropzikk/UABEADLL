using System;
using Avalonia.Automation.Peers;
using Avalonia.Controls.Automation.Peers;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Reactive;

namespace Avalonia.Controls;

[TemplatePart("PART_Indicator", typeof(Border))]
[PseudoClasses(new string[] { ":vertical", ":horizontal", ":indeterminate" })]
public class ProgressBar : RangeBase
{
	public class ProgressBarTemplateSettings : AvaloniaObject
	{
		private double _container2Width;

		private double _containerWidth;

		private double _containerAnimationStartPosition;

		private double _containerAnimationEndPosition;

		private double _container2AnimationStartPosition;

		private double _container2AnimationEndPosition;

		private double _indeterminateStartingOffset;

		private double _indeterminateEndingOffset;

		public static readonly DirectProperty<ProgressBarTemplateSettings, double> ContainerAnimationStartPositionProperty = AvaloniaProperty.RegisterDirect("ContainerAnimationStartPosition", (ProgressBarTemplateSettings p) => p.ContainerAnimationStartPosition, delegate(ProgressBarTemplateSettings p, double o)
		{
			p.ContainerAnimationStartPosition = o;
		}, 0.0);

		public static readonly DirectProperty<ProgressBarTemplateSettings, double> ContainerAnimationEndPositionProperty = AvaloniaProperty.RegisterDirect("ContainerAnimationEndPosition", (ProgressBarTemplateSettings p) => p.ContainerAnimationEndPosition, delegate(ProgressBarTemplateSettings p, double o)
		{
			p.ContainerAnimationEndPosition = o;
		}, 0.0);

		public static readonly DirectProperty<ProgressBarTemplateSettings, double> Container2AnimationStartPositionProperty = AvaloniaProperty.RegisterDirect("Container2AnimationStartPosition", (ProgressBarTemplateSettings p) => p.Container2AnimationStartPosition, delegate(ProgressBarTemplateSettings p, double o)
		{
			p.Container2AnimationStartPosition = o;
		}, 0.0);

		public static readonly DirectProperty<ProgressBarTemplateSettings, double> Container2AnimationEndPositionProperty = AvaloniaProperty.RegisterDirect("Container2AnimationEndPosition", (ProgressBarTemplateSettings p) => p.Container2AnimationEndPosition, delegate(ProgressBarTemplateSettings p, double o)
		{
			p.Container2AnimationEndPosition = o;
		}, 0.0);

		public static readonly DirectProperty<ProgressBarTemplateSettings, double> Container2WidthProperty = AvaloniaProperty.RegisterDirect("Container2Width", (ProgressBarTemplateSettings p) => p.Container2Width, delegate(ProgressBarTemplateSettings p, double o)
		{
			p.Container2Width = o;
		}, 0.0);

		public static readonly DirectProperty<ProgressBarTemplateSettings, double> ContainerWidthProperty = AvaloniaProperty.RegisterDirect("ContainerWidth", (ProgressBarTemplateSettings p) => p.ContainerWidth, delegate(ProgressBarTemplateSettings p, double o)
		{
			p.ContainerWidth = o;
		}, 0.0);

		public static readonly DirectProperty<ProgressBarTemplateSettings, double> IndeterminateStartingOffsetProperty = AvaloniaProperty.RegisterDirect("IndeterminateStartingOffset", (ProgressBarTemplateSettings p) => p.IndeterminateStartingOffset, delegate(ProgressBarTemplateSettings p, double o)
		{
			p.IndeterminateStartingOffset = o;
		}, 0.0);

		public static readonly DirectProperty<ProgressBarTemplateSettings, double> IndeterminateEndingOffsetProperty = AvaloniaProperty.RegisterDirect("IndeterminateEndingOffset", (ProgressBarTemplateSettings p) => p.IndeterminateEndingOffset, delegate(ProgressBarTemplateSettings p, double o)
		{
			p.IndeterminateEndingOffset = o;
		}, 0.0);

		public double ContainerWidth
		{
			get
			{
				return _containerWidth;
			}
			set
			{
				SetAndRaise(ContainerWidthProperty, ref _containerWidth, value);
			}
		}

		public double Container2Width
		{
			get
			{
				return _container2Width;
			}
			set
			{
				SetAndRaise(Container2WidthProperty, ref _container2Width, value);
			}
		}

		public double ContainerAnimationStartPosition
		{
			get
			{
				return _containerAnimationStartPosition;
			}
			set
			{
				SetAndRaise(ContainerAnimationStartPositionProperty, ref _containerAnimationStartPosition, value);
			}
		}

		public double ContainerAnimationEndPosition
		{
			get
			{
				return _containerAnimationEndPosition;
			}
			set
			{
				SetAndRaise(ContainerAnimationEndPositionProperty, ref _containerAnimationEndPosition, value);
			}
		}

		public double Container2AnimationStartPosition
		{
			get
			{
				return _container2AnimationStartPosition;
			}
			set
			{
				SetAndRaise(Container2AnimationStartPositionProperty, ref _container2AnimationStartPosition, value);
			}
		}

		public double Container2AnimationEndPosition
		{
			get
			{
				return _container2AnimationEndPosition;
			}
			set
			{
				SetAndRaise(Container2AnimationEndPositionProperty, ref _container2AnimationEndPosition, value);
			}
		}

		public double IndeterminateStartingOffset
		{
			get
			{
				return _indeterminateStartingOffset;
			}
			set
			{
				SetAndRaise(IndeterminateStartingOffsetProperty, ref _indeterminateStartingOffset, value);
			}
		}

		public double IndeterminateEndingOffset
		{
			get
			{
				return _indeterminateEndingOffset;
			}
			set
			{
				SetAndRaise(IndeterminateEndingOffsetProperty, ref _indeterminateEndingOffset, value);
			}
		}
	}

	private double _percentage;

	private Border? _indicator;

	private IDisposable? _trackSizeChangedListener;

	public static readonly StyledProperty<bool> IsIndeterminateProperty;

	public static readonly StyledProperty<bool> ShowProgressTextProperty;

	public static readonly StyledProperty<string> ProgressTextFormatProperty;

	public static readonly StyledProperty<Orientation> OrientationProperty;

	public static readonly DirectProperty<ProgressBar, double> PercentageProperty;

	public double Percentage
	{
		get
		{
			return _percentage;
		}
		private set
		{
			SetAndRaise(PercentageProperty, ref _percentage, value);
		}
	}

	public ProgressBarTemplateSettings TemplateSettings { get; } = new ProgressBarTemplateSettings();

	public bool IsIndeterminate
	{
		get
		{
			return GetValue(IsIndeterminateProperty);
		}
		set
		{
			SetValue(IsIndeterminateProperty, value);
		}
	}

	public bool ShowProgressText
	{
		get
		{
			return GetValue(ShowProgressTextProperty);
		}
		set
		{
			SetValue(ShowProgressTextProperty, value);
		}
	}

	public string ProgressTextFormat
	{
		get
		{
			return GetValue(ProgressTextFormatProperty);
		}
		set
		{
			SetValue(ProgressTextFormatProperty, value);
		}
	}

	public Orientation Orientation
	{
		get
		{
			return GetValue(OrientationProperty);
		}
		set
		{
			SetValue(OrientationProperty, value);
		}
	}

	static ProgressBar()
	{
		IsIndeterminateProperty = AvaloniaProperty.Register<ProgressBar, bool>("IsIndeterminate", defaultValue: false);
		ShowProgressTextProperty = AvaloniaProperty.Register<ProgressBar, bool>("ShowProgressText", defaultValue: false);
		ProgressTextFormatProperty = AvaloniaProperty.Register<ProgressBar, string>("ProgressTextFormat", "{1:0}%");
		OrientationProperty = AvaloniaProperty.Register<ProgressBar, Orientation>("Orientation", Orientation.Horizontal);
		PercentageProperty = AvaloniaProperty.RegisterDirect("Percentage", (ProgressBar o) => o.Percentage, null, 0.0);
		RangeBase.ValueProperty.OverrideMetadata<ProgressBar>(new StyledPropertyMetadata<double>(default(Optional<double>), BindingMode.OneWay));
	}

	public ProgressBar()
	{
		UpdatePseudoClasses(IsIndeterminate, Orientation);
	}

	protected override Size ArrangeOverride(Size finalSize)
	{
		Size result = base.ArrangeOverride(finalSize);
		UpdateIndicator();
		return result;
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (change.Property == RangeBase.ValueProperty || change.Property == RangeBase.MinimumProperty || change.Property == RangeBase.MaximumProperty || change.Property == IsIndeterminateProperty || change.Property == OrientationProperty)
		{
			UpdateIndicator();
		}
		if (change.Property == IsIndeterminateProperty)
		{
			UpdatePseudoClasses(change.GetNewValue<bool>(), null);
		}
		else if (change.Property == OrientationProperty)
		{
			UpdatePseudoClasses(null, change.GetNewValue<Orientation>());
		}
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		_trackSizeChangedListener?.Dispose();
		_indicator = e.NameScope.Get<Border>("PART_Indicator");
		_trackSizeChangedListener = _indicator.Parent?.GetPropertyChangedObservable(Visual.BoundsProperty).Subscribe(delegate
		{
			UpdateIndicator();
		});
		UpdateIndicator();
	}

	protected override AutomationPeer OnCreateAutomationPeer()
	{
		return new ProgressBarAutomationPeer(this);
	}

	private void UpdateIndicator()
	{
		Size size = _indicator?.VisualParent?.Bounds.Size ?? base.Bounds.Size;
		if (_indicator == null)
		{
			return;
		}
		if (IsIndeterminate)
		{
			double num = ((Orientation == Orientation.Horizontal) ? size.Width : size.Height);
			double num2 = num * 0.4;
			double num3 = num * 0.6;
			TemplateSettings.ContainerWidth = num2;
			TemplateSettings.Container2Width = num3;
			TemplateSettings.ContainerAnimationStartPosition = num2 * -1.8;
			TemplateSettings.ContainerAnimationEndPosition = num2 * 3.0;
			TemplateSettings.Container2AnimationStartPosition = num3 * -1.5;
			TemplateSettings.Container2AnimationEndPosition = num3 * 1.66;
			TemplateSettings.IndeterminateStartingOffset = 0.0 - num;
			TemplateSettings.IndeterminateEndingOffset = num;
		}
		else
		{
			double num4 = ((Math.Abs(base.Maximum - base.Minimum) < double.Epsilon) ? 1.0 : ((base.Value - base.Minimum) / (base.Maximum - base.Minimum)));
			if (Orientation == Orientation.Horizontal)
			{
				_indicator.Width = (size.Width - _indicator.Margin.Left - _indicator.Margin.Right) * num4;
				_indicator.Height = double.NaN;
			}
			else
			{
				_indicator.Width = double.NaN;
				_indicator.Height = (size.Height - _indicator.Margin.Top - _indicator.Margin.Bottom) * num4;
			}
			Percentage = num4 * 100.0;
		}
	}

	private void UpdatePseudoClasses(bool? isIndeterminate, Orientation? o)
	{
		if (isIndeterminate.HasValue)
		{
			base.PseudoClasses.Set(":indeterminate", isIndeterminate.Value);
		}
		if (o.HasValue)
		{
			base.PseudoClasses.Set(":vertical", o == Orientation.Vertical);
			base.PseudoClasses.Set(":horizontal", o == Orientation.Horizontal);
		}
	}
}
