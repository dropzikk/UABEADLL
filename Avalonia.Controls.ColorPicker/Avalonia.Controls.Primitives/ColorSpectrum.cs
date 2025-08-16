using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Shapes;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Reactive;
using Avalonia.Threading;
using Avalonia.Utilities;

namespace Avalonia.Controls.Primitives;

[TemplatePart("PART_InputTarget", typeof(Canvas))]
[TemplatePart("PART_LayoutRoot", typeof(Panel))]
[TemplatePart("PART_SelectionEllipsePanel", typeof(Panel))]
[TemplatePart("PART_SizingPanel", typeof(Panel))]
[TemplatePart("PART_SpectrumEllipse", typeof(Ellipse))]
[TemplatePart("PART_SpectrumRectangle", typeof(Rectangle))]
[TemplatePart("PART_SpectrumOverlayEllipse", typeof(Ellipse))]
[TemplatePart("PART_SpectrumOverlayRectangle", typeof(Rectangle))]
[PseudoClasses(new string[] { ":pressed", ":large-selector", ":dark-selector", ":light-selector" })]
public class ColorSpectrum : TemplatedControl
{
	protected const string pcPressed = ":pressed";

	protected const string pcDarkSelector = ":dark-selector";

	protected const string pcLargeSelector = ":large-selector";

	protected const string pcLightSelector = ":light-selector";

	private bool _updatingColor;

	private bool _updatingHsvColor;

	private bool _isPointerPressed;

	private bool _shouldShowLargeSelection;

	private List<Hsv> _hsvValues = new List<Hsv>();

	private ColorComponent _thirdComponent = ColorComponent.Component3;

	private IDisposable? _layoutRootDisposable;

	private IDisposable? _selectionEllipsePanelDisposable;

	private Panel? _layoutRoot;

	private Panel? _sizingPanel;

	private Rectangle? _spectrumRectangle;

	private Ellipse? _spectrumEllipse;

	private Rectangle? _spectrumOverlayRectangle;

	private Ellipse? _spectrumOverlayEllipse;

	private Canvas? _inputTarget;

	private Panel? _selectionEllipsePanel;

	private WriteableBitmap? _hueRedBitmap;

	private WriteableBitmap? _hueYellowBitmap;

	private WriteableBitmap? _hueGreenBitmap;

	private WriteableBitmap? _hueCyanBitmap;

	private WriteableBitmap? _hueBlueBitmap;

	private WriteableBitmap? _huePurpleBitmap;

	private WriteableBitmap? _saturationMinimumBitmap;

	private WriteableBitmap? _saturationMaximumBitmap;

	private WriteableBitmap? _valueBitmap;

	private WriteableBitmap? _minBitmap;

	private WriteableBitmap? _maxBitmap;

	private ColorSpectrumShape _shapeFromLastBitmapCreation;

	private ColorSpectrumComponents _componentsFromLastBitmapCreation = ColorSpectrumComponents.HueSaturation;

	private double _imageWidthFromLastBitmapCreation;

	private double _imageHeightFromLastBitmapCreation;

	private int _minHueFromLastBitmapCreation;

	private int _maxHueFromLastBitmapCreation;

	private int _minSaturationFromLastBitmapCreation;

	private int _maxSaturationFromLastBitmapCreation;

	private int _minValueFromLastBitmapCreation;

	private int _maxValueFromLastBitmapCreation;

	private Color _oldColor = Color.FromArgb(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

	private HsvColor _oldHsvColor = HsvColor.FromAhsv(0.0, 0.0, 1.0, 1.0);

	public static readonly StyledProperty<Color> ColorProperty = AvaloniaProperty.Register<ColorSpectrum, Color>("Color", Colors.White, inherits: false, BindingMode.TwoWay);

	public static readonly StyledProperty<ColorSpectrumComponents> ComponentsProperty = AvaloniaProperty.Register<ColorSpectrum, ColorSpectrumComponents>("Components", ColorSpectrumComponents.HueSaturation);

	public static readonly StyledProperty<HsvColor> HsvColorProperty = AvaloniaProperty.Register<ColorSpectrum, HsvColor>("HsvColor", Colors.White.ToHsv(), inherits: false, BindingMode.TwoWay);

	public static readonly StyledProperty<int> MaxHueProperty = AvaloniaProperty.Register<ColorSpectrum, int>("MaxHue", 359);

	public static readonly StyledProperty<int> MaxSaturationProperty = AvaloniaProperty.Register<ColorSpectrum, int>("MaxSaturation", 100);

	public static readonly StyledProperty<int> MaxValueProperty = AvaloniaProperty.Register<ColorSpectrum, int>("MaxValue", 100);

	public static readonly StyledProperty<int> MinHueProperty = AvaloniaProperty.Register<ColorSpectrum, int>("MinHue", 0);

	public static readonly StyledProperty<int> MinSaturationProperty = AvaloniaProperty.Register<ColorSpectrum, int>("MinSaturation", 0);

	public static readonly StyledProperty<int> MinValueProperty = AvaloniaProperty.Register<ColorSpectrum, int>("MinValue", 0);

	public static readonly StyledProperty<ColorSpectrumShape> ShapeProperty = AvaloniaProperty.Register<ColorSpectrum, ColorSpectrumShape>("Shape", ColorSpectrumShape.Box);

	public static readonly DirectProperty<ColorSpectrum, ColorComponent> ThirdComponentProperty = AvaloniaProperty.RegisterDirect("ThirdComponent", (ColorSpectrum o) => o.ThirdComponent, null, ColorComponent.Alpha);

	public Color Color
	{
		get
		{
			return GetValue(ColorProperty);
		}
		set
		{
			SetValue(ColorProperty, value);
		}
	}

	public ColorSpectrumComponents Components
	{
		get
		{
			return GetValue(ComponentsProperty);
		}
		set
		{
			SetValue(ComponentsProperty, value);
		}
	}

	public HsvColor HsvColor
	{
		get
		{
			return GetValue(HsvColorProperty);
		}
		set
		{
			SetValue(HsvColorProperty, value);
		}
	}

	public int MaxHue
	{
		get
		{
			return GetValue(MaxHueProperty);
		}
		set
		{
			SetValue(MaxHueProperty, value);
		}
	}

	public int MaxSaturation
	{
		get
		{
			return GetValue(MaxSaturationProperty);
		}
		set
		{
			SetValue(MaxSaturationProperty, value);
		}
	}

	public int MaxValue
	{
		get
		{
			return GetValue(MaxValueProperty);
		}
		set
		{
			SetValue(MaxValueProperty, value);
		}
	}

	public int MinHue
	{
		get
		{
			return GetValue(MinHueProperty);
		}
		set
		{
			SetValue(MinHueProperty, value);
		}
	}

	public int MinSaturation
	{
		get
		{
			return GetValue(MinSaturationProperty);
		}
		set
		{
			SetValue(MinSaturationProperty, value);
		}
	}

	public int MinValue
	{
		get
		{
			return GetValue(MinValueProperty);
		}
		set
		{
			SetValue(MinValueProperty, value);
		}
	}

	public ColorSpectrumShape Shape
	{
		get
		{
			return GetValue(ShapeProperty);
		}
		set
		{
			SetValue(ShapeProperty, value);
		}
	}

	public ColorComponent ThirdComponent
	{
		get
		{
			return _thirdComponent;
		}
		private set
		{
			SetAndRaise(ThirdComponentProperty, ref _thirdComponent, value);
		}
	}

	public event EventHandler<ColorChangedEventArgs>? ColorChanged;

	public ColorSpectrum()
	{
		_shapeFromLastBitmapCreation = Shape;
		_componentsFromLastBitmapCreation = Components;
		_imageWidthFromLastBitmapCreation = 0.0;
		_imageHeightFromLastBitmapCreation = 0.0;
		_minHueFromLastBitmapCreation = MinHue;
		_maxHueFromLastBitmapCreation = MaxHue;
		_minSaturationFromLastBitmapCreation = MinSaturation;
		_maxSaturationFromLastBitmapCreation = MaxSaturation;
		_minValueFromLastBitmapCreation = MinValue;
		_maxValueFromLastBitmapCreation = MaxValue;
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		base.OnApplyTemplate(e);
		UnregisterEvents();
		_inputTarget = e.NameScope.Find<Canvas>("PART_InputTarget");
		_layoutRoot = e.NameScope.Find<Panel>("PART_LayoutRoot");
		_selectionEllipsePanel = e.NameScope.Find<Panel>("PART_SelectionEllipsePanel");
		_sizingPanel = e.NameScope.Find<Panel>("PART_SizingPanel");
		_spectrumEllipse = e.NameScope.Find<Ellipse>("PART_SpectrumEllipse");
		_spectrumRectangle = e.NameScope.Find<Rectangle>("PART_SpectrumRectangle");
		_spectrumOverlayEllipse = e.NameScope.Find<Ellipse>("PART_SpectrumOverlayEllipse");
		_spectrumOverlayRectangle = e.NameScope.Find<Rectangle>("PART_SpectrumOverlayRectangle");
		if (_inputTarget != null)
		{
			_inputTarget.PointerEntered += InputTarget_PointerEntered;
			_inputTarget.PointerExited += InputTarget_PointerExited;
			_inputTarget.PointerPressed += InputTarget_PointerPressed;
			_inputTarget.PointerMoved += InputTarget_PointerMoved;
			_inputTarget.PointerReleased += InputTarget_PointerReleased;
		}
		if (_layoutRoot != null)
		{
			_layoutRootDisposable = _layoutRoot.GetObservable(Visual.BoundsProperty).Subscribe(delegate
			{
				CreateBitmapsAndColorMap();
			});
		}
		if (_selectionEllipsePanel != null)
		{
			_selectionEllipsePanelDisposable = _selectionEllipsePanel.GetObservable(Visual.FlowDirectionProperty).Subscribe(delegate
			{
				UpdateEllipse();
			});
		}
		if (_selectionEllipsePanel != null && ColorHelper.ToDisplayNameExists)
		{
			ToolTip.SetTip(_selectionEllipsePanel, ColorHelper.ToDisplayName(Color));
		}
		if (_hsvValues.Count == 0)
		{
			CreateBitmapsAndColorMap();
		}
		UpdateEllipse();
		UpdatePseudoClasses();
	}

	protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
	{
		base.OnAttachedToVisualTree(e);
		UpdateEllipse();
	}

	protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
	{
		base.OnDetachedFromVisualTree(e);
	}

	private void UnregisterEvents()
	{
		_layoutRootDisposable?.Dispose();
		_layoutRootDisposable = null;
		_selectionEllipsePanelDisposable?.Dispose();
		_selectionEllipsePanelDisposable = null;
		if (_inputTarget != null)
		{
			_inputTarget.PointerEntered -= InputTarget_PointerEntered;
			_inputTarget.PointerExited -= InputTarget_PointerExited;
			_inputTarget.PointerPressed -= InputTarget_PointerPressed;
			_inputTarget.PointerMoved -= InputTarget_PointerMoved;
			_inputTarget.PointerReleased -= InputTarget_PointerReleased;
		}
	}

	protected override void OnKeyDown(KeyEventArgs e)
	{
		Key key = e.Key;
		if (key != Key.Left && key != Key.Right && key != Key.Up && key != Key.Down)
		{
			base.OnKeyDown(e);
			return;
		}
		bool flag = e.KeyModifiers.HasFlag(KeyModifiers.Control);
		HsvComponent hsvComponent = HsvComponent.Hue;
		bool flag2 = false;
		switch (key)
		{
		case Key.Left:
		case Key.Right:
			switch (Components)
			{
			case ColorSpectrumComponents.HueValue:
			case ColorSpectrumComponents.HueSaturation:
				hsvComponent = HsvComponent.Hue;
				break;
			case ColorSpectrumComponents.SaturationValue:
				flag2 = true;
				goto case ColorSpectrumComponents.SaturationHue;
			case ColorSpectrumComponents.SaturationHue:
				hsvComponent = HsvComponent.Saturation;
				break;
			case ColorSpectrumComponents.ValueHue:
			case ColorSpectrumComponents.ValueSaturation:
				hsvComponent = HsvComponent.Value;
				break;
			}
			break;
		case Key.Up:
		case Key.Down:
			switch (Components)
			{
			case ColorSpectrumComponents.ValueHue:
			case ColorSpectrumComponents.SaturationHue:
				hsvComponent = HsvComponent.Hue;
				break;
			case ColorSpectrumComponents.HueSaturation:
			case ColorSpectrumComponents.ValueSaturation:
				hsvComponent = HsvComponent.Saturation;
				break;
			case ColorSpectrumComponents.SaturationValue:
				flag2 = true;
				goto case ColorSpectrumComponents.HueValue;
			case ColorSpectrumComponents.HueValue:
				hsvComponent = HsvComponent.Value;
				break;
			}
			break;
		}
		double minBound = 0.0;
		double maxBound = 0.0;
		switch (hsvComponent)
		{
		case HsvComponent.Hue:
			minBound = MinHue;
			maxBound = MaxHue;
			break;
		case HsvComponent.Saturation:
			minBound = MinSaturation;
			maxBound = MaxSaturation;
			break;
		case HsvComponent.Value:
			minBound = MinValue;
			maxBound = MaxValue;
			break;
		}
		IncrementDirection incrementDirection = (((hsvComponent != HsvComponent.Hue || (key != Key.Left && key != Key.Up)) && (hsvComponent == HsvComponent.Hue || (key != Key.Right && key != Key.Down))) ? IncrementDirection.Higher : IncrementDirection.Lower);
		if (base.FlowDirection == FlowDirection.RightToLeft != flag2 && (key == Key.Left || key == Key.Right))
		{
			incrementDirection = ((incrementDirection != IncrementDirection.Higher) ? IncrementDirection.Higher : IncrementDirection.Lower);
		}
		IncrementAmount amount = (flag ? IncrementAmount.Large : IncrementAmount.Small);
		HsvColor hsvColor = HsvColor;
		UpdateColor(ColorPickerHelpers.IncrementColorComponent(new Hsv(hsvColor), hsvComponent, incrementDirection, amount, shouldWrap: true, minBound, maxBound));
		e.Handled = true;
	}

	protected override void OnGotFocus(GotFocusEventArgs e)
	{
		if (_selectionEllipsePanel != null && ColorHelper.ToDisplayNameExists)
		{
			ToolTip.SetIsOpen(_selectionEllipsePanel, value: true);
		}
		UpdatePseudoClasses();
		base.OnGotFocus(e);
	}

	protected override void OnLostFocus(RoutedEventArgs e)
	{
		if (_selectionEllipsePanel != null && ColorHelper.ToDisplayNameExists)
		{
			ToolTip.SetIsOpen(_selectionEllipsePanel, value: false);
		}
		UpdatePseudoClasses();
		base.OnLostFocus(e);
	}

	protected override void OnPointerExited(PointerEventArgs e)
	{
		if (_selectionEllipsePanel != null && ColorHelper.ToDisplayNameExists)
		{
			ToolTip.SetIsOpen(_selectionEllipsePanel, value: false);
		}
		UpdatePseudoClasses();
		base.OnPointerExited(e);
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		if (change.Property == ColorProperty)
		{
			if (!_updatingColor)
			{
				Color color = Color;
				_updatingHsvColor = true;
				Hsv hsv = new Rgb(color).ToHsv();
				SetCurrentValue(HsvColorProperty, hsv.ToHsvColor((double)(int)color.A / 255.0));
				_updatingHsvColor = false;
				UpdateEllipse();
				UpdateBitmapSources();
			}
			_oldColor = change.GetOldValue<Color>();
		}
		else if (change.Property == HsvColorProperty)
		{
			if (!_updatingHsvColor)
			{
				SetColor();
			}
			_oldHsvColor = change.GetOldValue<HsvColor>();
		}
		else if (change.Property == MinHueProperty || change.Property == MaxHueProperty)
		{
			int minHue = MinHue;
			int maxHue = MaxHue;
			if (minHue < 0 || minHue > 359)
			{
				throw new ArgumentException("MinHue must be between 0 and 359.");
			}
			if (maxHue < 0 || maxHue > 359)
			{
				throw new ArgumentException("MaxHue must be between 0 and 359.");
			}
			ColorSpectrumComponents components = Components;
			if (components != ColorSpectrumComponents.SaturationValue && components != ColorSpectrumComponents.ValueSaturation)
			{
				CreateBitmapsAndColorMap();
			}
		}
		else if (change.Property == MinSaturationProperty || change.Property == MaxSaturationProperty)
		{
			int minSaturation = MinSaturation;
			int maxSaturation = MaxSaturation;
			if (minSaturation < 0 || minSaturation > 100)
			{
				throw new ArgumentException("MinSaturation must be between 0 and 100.");
			}
			if (maxSaturation < 0 || maxSaturation > 100)
			{
				throw new ArgumentException("MaxSaturation must be between 0 and 100.");
			}
			ColorSpectrumComponents components2 = Components;
			if (components2 != 0 && components2 != ColorSpectrumComponents.ValueHue)
			{
				CreateBitmapsAndColorMap();
			}
		}
		else if (change.Property == MinValueProperty || change.Property == MaxValueProperty)
		{
			int minValue = MinValue;
			int maxValue = MaxValue;
			if (minValue < 0 || minValue > 100)
			{
				throw new ArgumentException("MinValue must be between 0 and 100.");
			}
			if (maxValue < 0 || maxValue > 100)
			{
				throw new ArgumentException("MaxValue must be between 0 and 100.");
			}
			ColorSpectrumComponents components3 = Components;
			if (components3 != ColorSpectrumComponents.HueSaturation && components3 != ColorSpectrumComponents.SaturationHue)
			{
				CreateBitmapsAndColorMap();
			}
		}
		else if (change.Property == ShapeProperty)
		{
			CreateBitmapsAndColorMap();
		}
		else if (change.Property == ComponentsProperty)
		{
			switch (Components)
			{
			case ColorSpectrumComponents.HueSaturation:
			case ColorSpectrumComponents.SaturationHue:
				ThirdComponent = ColorComponent.Component3;
				break;
			case ColorSpectrumComponents.HueValue:
			case ColorSpectrumComponents.ValueHue:
				ThirdComponent = ColorComponent.Component2;
				break;
			case ColorSpectrumComponents.SaturationValue:
			case ColorSpectrumComponents.ValueSaturation:
				ThirdComponent = ColorComponent.Component1;
				break;
			}
			CreateBitmapsAndColorMap();
		}
		base.OnPropertyChanged(change);
	}

	private void SetColor()
	{
		HsvColor hsvColor = HsvColor;
		_updatingColor = true;
		Rgb rgb = new Hsv(hsvColor).ToRgb();
		SetCurrentValue(ColorProperty, rgb.ToColor(hsvColor.A));
		_updatingColor = false;
		UpdateEllipse();
		UpdateBitmapSources();
		RaiseColorChanged();
	}

	public void RaiseColorChanged()
	{
		Color color = Color;
		bool num = _oldColor.A != color.A || _oldColor.R != color.R || _oldColor.G != color.G || _oldColor.B != color.B;
		bool flag = (_oldColor.R == color.R && color.R == 0) || (_oldColor.G == color.G && color.G == 0) || (_oldColor.B == color.B && color.B == 0);
		if (num || flag)
		{
			ColorChangedEventArgs e = new ColorChangedEventArgs(_oldColor, color);
			this.ColorChanged?.Invoke(this, e);
			if (_selectionEllipsePanel != null && ColorHelper.ToDisplayNameExists)
			{
				ToolTip.SetTip(_selectionEllipsePanel, ColorHelper.ToDisplayName(Color));
			}
		}
	}

	private void UpdatePseudoClasses()
	{
		base.PseudoClasses.Set(":pressed", _isPointerPressed);
		if (_isPointerPressed)
		{
			base.PseudoClasses.Set(":large-selector", _shouldShowLargeSelection);
		}
		else
		{
			base.PseudoClasses.Set(":large-selector", value: false);
		}
		if (SelectionEllipseShouldBeLight())
		{
			base.PseudoClasses.Set(":dark-selector", value: false);
			base.PseudoClasses.Set(":light-selector", value: true);
		}
		else
		{
			base.PseudoClasses.Set(":dark-selector", value: true);
			base.PseudoClasses.Set(":light-selector", value: false);
		}
	}

	private void UpdateColor(Hsv newHsv)
	{
		_updatingColor = true;
		_updatingHsvColor = true;
		double num = HsvColor.A;
		if (base.IsLoaded)
		{
			bool flag = num == 0.0;
			bool flag2 = false;
			switch (Components)
			{
			case ColorSpectrumComponents.HueValue:
			case ColorSpectrumComponents.ValueHue:
				flag2 = newHsv.S == 0.0;
				break;
			case ColorSpectrumComponents.HueSaturation:
			case ColorSpectrumComponents.SaturationHue:
				flag2 = newHsv.V == 0.0;
				break;
			case ColorSpectrumComponents.SaturationValue:
			case ColorSpectrumComponents.ValueSaturation:
				flag2 = newHsv.H == 0.0;
				break;
			}
			if (flag && flag2)
			{
				num = 1.0;
				switch (Components)
				{
				case ColorSpectrumComponents.HueValue:
				case ColorSpectrumComponents.ValueHue:
					newHsv.S = 1.0;
					break;
				case ColorSpectrumComponents.HueSaturation:
				case ColorSpectrumComponents.SaturationHue:
					newHsv.V = 1.0;
					break;
				case ColorSpectrumComponents.SaturationValue:
				case ColorSpectrumComponents.ValueSaturation:
					newHsv.H = 360.0;
					break;
				}
			}
		}
		Rgb rgb = newHsv.ToRgb();
		SetCurrentValue(ColorProperty, rgb.ToColor(num));
		SetCurrentValue(HsvColorProperty, newHsv.ToHsvColor(num));
		UpdateEllipse();
		UpdatePseudoClasses();
		_updatingHsvColor = false;
		_updatingColor = false;
		RaiseColorChanged();
	}

	private void UpdateColorFromPoint(PointerPoint point)
	{
		if (_hsvValues.Count != 0)
		{
			double layoutScale = LayoutHelper.GetLayoutScale(this);
			double num = point.Position.X * layoutScale;
			double num2 = point.Position.Y * layoutScale;
			double num3 = Math.Min(_imageWidthFromLastBitmapCreation, _imageHeightFromLastBitmapCreation) / 2.0;
			double num4 = Math.Sqrt(Math.Pow(num - num3, 2.0) + Math.Pow(num2 - num3, 2.0));
			ColorSpectrumShape shape = Shape;
			if (num4 > num3 && shape == ColorSpectrumShape.Ring)
			{
				num = num3 / num4 * (num - num3) + num3;
				num2 = num3 / num4 * (num2 - num3) + num3;
			}
			int num5 = (int)Math.Round(num);
			int num6 = (int)Math.Round(num2);
			int num7 = (int)Math.Round(_imageWidthFromLastBitmapCreation);
			if (num5 < 0)
			{
				num5 = 0;
			}
			else if ((double)num5 >= _imageWidthFromLastBitmapCreation)
			{
				num5 = (int)Math.Round(_imageWidthFromLastBitmapCreation) - 1;
			}
			if (num6 < 0)
			{
				num6 = 0;
			}
			else if ((double)num6 >= _imageHeightFromLastBitmapCreation)
			{
				num6 = (int)Math.Round(_imageHeightFromLastBitmapCreation) - 1;
			}
			Hsv newHsv = _hsvValues[MathUtilities.Clamp(num6 * num7 + num5, 0, _hsvValues.Count - 1)];
			HsvColor hsvColor = HsvColor;
			switch (Components)
			{
			case ColorSpectrumComponents.HueValue:
			case ColorSpectrumComponents.ValueHue:
				newHsv.S = hsvColor.S;
				break;
			case ColorSpectrumComponents.HueSaturation:
			case ColorSpectrumComponents.SaturationHue:
				newHsv.V = hsvColor.V;
				break;
			case ColorSpectrumComponents.SaturationValue:
			case ColorSpectrumComponents.ValueSaturation:
				newHsv.H = hsvColor.H;
				break;
			}
			UpdateColor(newHsv);
		}
	}

	private void UpdateEllipse()
	{
		if (_selectionEllipsePanel == null)
		{
			return;
		}
		if (_imageWidthFromLastBitmapCreation == 0.0 || _imageHeightFromLastBitmapCreation == 0.0)
		{
			_selectionEllipsePanel.IsVisible = false;
			return;
		}
		_selectionEllipsePanel.IsVisible = true;
		Hsv hsv = new Hsv(HsvColor);
		hsv.H = MathUtilities.Clamp(hsv.H, _minHueFromLastBitmapCreation, _maxHueFromLastBitmapCreation);
		hsv.S = MathUtilities.Clamp(hsv.S, (double)_minSaturationFromLastBitmapCreation / 100.0, (double)_maxSaturationFromLastBitmapCreation / 100.0);
		hsv.V = MathUtilities.Clamp(hsv.V, (double)_minValueFromLastBitmapCreation / 100.0, (double)_maxValueFromLastBitmapCreation / 100.0);
		double num6;
		double num7;
		if (_shapeFromLastBitmapCreation == ColorSpectrumShape.Box)
		{
			double num = 0.0;
			double num2 = 0.0;
			double num3 = (hsv.H - (double)_minHueFromLastBitmapCreation) / (double)(_maxHueFromLastBitmapCreation - _minHueFromLastBitmapCreation);
			double num4 = (hsv.S * 100.0 - (double)_minSaturationFromLastBitmapCreation) / (double)(_maxSaturationFromLastBitmapCreation - _minSaturationFromLastBitmapCreation);
			double num5 = (hsv.V * 100.0 - (double)_minValueFromLastBitmapCreation) / (double)(_maxValueFromLastBitmapCreation - _minValueFromLastBitmapCreation);
			if (_componentsFromLastBitmapCreation == ColorSpectrumComponents.HueSaturation || _componentsFromLastBitmapCreation == ColorSpectrumComponents.SaturationHue)
			{
				num4 = 1.0 - num4;
			}
			else
			{
				num5 = 1.0 - num5;
			}
			switch (_componentsFromLastBitmapCreation)
			{
			case ColorSpectrumComponents.HueValue:
				num = num3;
				num2 = num5;
				break;
			case ColorSpectrumComponents.HueSaturation:
				num = num3;
				num2 = num4;
				break;
			case ColorSpectrumComponents.ValueHue:
				num = num5;
				num2 = num3;
				break;
			case ColorSpectrumComponents.ValueSaturation:
				num = num5;
				num2 = num4;
				break;
			case ColorSpectrumComponents.SaturationHue:
				num = num4;
				num2 = num3;
				break;
			case ColorSpectrumComponents.SaturationValue:
				num = num4;
				num2 = num5;
				break;
			}
			num6 = _imageWidthFromLastBitmapCreation * num;
			num7 = _imageHeightFromLastBitmapCreation * num2;
		}
		else
		{
			double num8 = 0.0;
			double num9 = 0.0;
			double num10 = ((_maxHueFromLastBitmapCreation != _minHueFromLastBitmapCreation) ? (360.0 * (hsv.H - (double)_minHueFromLastBitmapCreation) / (double)(_maxHueFromLastBitmapCreation - _minHueFromLastBitmapCreation)) : 0.0);
			double num11 = ((_maxSaturationFromLastBitmapCreation != _minSaturationFromLastBitmapCreation) ? (360.0 * (hsv.S * 100.0 - (double)_minSaturationFromLastBitmapCreation) / (double)(_maxSaturationFromLastBitmapCreation - _minSaturationFromLastBitmapCreation)) : 0.0);
			double num12 = ((_maxValueFromLastBitmapCreation != _minValueFromLastBitmapCreation) ? (360.0 * (hsv.V * 100.0 - (double)_minValueFromLastBitmapCreation) / (double)(_maxValueFromLastBitmapCreation - _minValueFromLastBitmapCreation)) : 0.0);
			double num13 = ((_maxHueFromLastBitmapCreation != _minHueFromLastBitmapCreation) ? ((hsv.H - (double)_minHueFromLastBitmapCreation) / (double)(_maxHueFromLastBitmapCreation - _minHueFromLastBitmapCreation) - 1.0) : 0.0);
			double num14 = ((_maxSaturationFromLastBitmapCreation != _minSaturationFromLastBitmapCreation) ? ((hsv.S * 100.0 - (double)_minSaturationFromLastBitmapCreation) / (double)(_maxSaturationFromLastBitmapCreation - _minSaturationFromLastBitmapCreation) - 1.0) : 0.0);
			double num15 = ((_maxValueFromLastBitmapCreation != _minValueFromLastBitmapCreation) ? ((hsv.V * 100.0 - (double)_minValueFromLastBitmapCreation) / (double)(_maxValueFromLastBitmapCreation - _minValueFromLastBitmapCreation) - 1.0) : 0.0);
			if (_componentsFromLastBitmapCreation == ColorSpectrumComponents.HueSaturation || _componentsFromLastBitmapCreation == ColorSpectrumComponents.SaturationHue)
			{
				num11 = 360.0 - num11;
				num14 = 0.0 - num14 - 1.0;
			}
			else
			{
				num12 = 360.0 - num12;
				num15 = 0.0 - num15 - 1.0;
			}
			switch (_componentsFromLastBitmapCreation)
			{
			case ColorSpectrumComponents.HueValue:
				num8 = num10;
				num9 = num15;
				break;
			case ColorSpectrumComponents.HueSaturation:
				num8 = num10;
				num9 = num14;
				break;
			case ColorSpectrumComponents.ValueHue:
				num8 = num12;
				num9 = num13;
				break;
			case ColorSpectrumComponents.ValueSaturation:
				num8 = num12;
				num9 = num14;
				break;
			case ColorSpectrumComponents.SaturationHue:
				num8 = num11;
				num9 = num13;
				break;
			case ColorSpectrumComponents.SaturationValue:
				num8 = num11;
				num9 = num15;
				break;
			}
			double num16 = Math.Min(_imageWidthFromLastBitmapCreation, _imageHeightFromLastBitmapCreation) / 2.0;
			num6 = Math.Cos(num8 * Math.PI / 180.0 + Math.PI) * num16 * num9 + num16;
			num7 = Math.Sin(num8 * Math.PI / 180.0 + Math.PI) * num16 * num9 + num16;
		}
		double layoutScale = LayoutHelper.GetLayoutScale(this);
		Canvas.SetLeft(_selectionEllipsePanel, num6 / layoutScale - _selectionEllipsePanel.Width / 2.0);
		Canvas.SetTop(_selectionEllipsePanel, num7 / layoutScale - _selectionEllipsePanel.Height / 2.0);
		if (base.IsFocused && _selectionEllipsePanel != null && ColorHelper.ToDisplayNameExists)
		{
			ToolTip.SetIsOpen(_selectionEllipsePanel, value: true);
		}
		UpdatePseudoClasses();
	}

	private void InputTarget_PointerEntered(object? sender, PointerEventArgs args)
	{
		UpdatePseudoClasses();
		args.Handled = true;
	}

	private void InputTarget_PointerExited(object? sender, PointerEventArgs args)
	{
		UpdatePseudoClasses();
		args.Handled = true;
	}

	private void InputTarget_PointerPressed(object? sender, PointerPressedEventArgs args)
	{
		Canvas inputTarget = _inputTarget;
		Focus();
		_isPointerPressed = true;
		_shouldShowLargeSelection = args.Pointer.Type == PointerType.Touch;
		args.Pointer.Capture(inputTarget);
		UpdateColorFromPoint(args.GetCurrentPoint(inputTarget));
		UpdatePseudoClasses();
		UpdateEllipse();
		args.Handled = true;
	}

	private void InputTarget_PointerMoved(object? sender, PointerEventArgs args)
	{
		if (_isPointerPressed)
		{
			UpdateColorFromPoint(args.GetCurrentPoint(_inputTarget));
			args.Handled = true;
		}
	}

	private void InputTarget_PointerReleased(object? sender, PointerReleasedEventArgs args)
	{
		_isPointerPressed = false;
		_shouldShowLargeSelection = false;
		args.Pointer.Capture(null);
		UpdatePseudoClasses();
		UpdateEllipse();
		args.Handled = true;
	}

	private async void CreateBitmapsAndColorMap()
	{
		if (_layoutRoot == null || _sizingPanel == null || _inputTarget == null || _spectrumRectangle == null || _spectrumEllipse == null || _spectrumOverlayRectangle == null || _spectrumOverlayEllipse == null)
		{
			return;
		}
		double num = Math.Min(_layoutRoot.Bounds.Width, _layoutRoot.Bounds.Height);
		if (num == 0.0)
		{
			return;
		}
		_sizingPanel.Width = num;
		_sizingPanel.Height = num;
		_inputTarget.Width = num;
		_inputTarget.Height = num;
		_spectrumRectangle.Width = num;
		_spectrumRectangle.Height = num;
		_spectrumEllipse.Width = num;
		_spectrumEllipse.Height = num;
		_spectrumOverlayRectangle.Width = num;
		_spectrumOverlayRectangle.Height = num;
		_spectrumOverlayEllipse.Width = num;
		_spectrumOverlayEllipse.Height = num;
		HsvColor hsvColor = HsvColor;
		int minHue = MinHue;
		int maxHue = MaxHue;
		int minSaturation = MinSaturation;
		int maxSaturation = MaxSaturation;
		int minValue = MinValue;
		int maxValue = MaxValue;
		ColorSpectrumShape shape = Shape;
		ColorSpectrumComponents components = Components;
		if (minHue >= maxHue)
		{
			maxHue = minHue;
		}
		if (minSaturation >= maxSaturation)
		{
			maxSaturation = minSaturation;
		}
		if (minValue >= maxValue)
		{
			maxValue = minValue;
		}
		Hsv hsv = new Hsv(hsvColor);
		double layoutScale = LayoutHelper.GetLayoutScale(this);
		int pixelDimension = (int)Math.Round(num * layoutScale);
		int num2 = pixelDimension * pixelDimension;
		int capacity = num2 * 4;
		ArrayList<byte> bgraMinPixelData = new ArrayList<byte>(capacity);
		ArrayList<byte> bgraMaxPixelData = new ArrayList<byte>(capacity);
		List<Hsv> newHsvValues = new List<Hsv>(num2);
		ArrayList<byte> bgraMiddle1PixelData;
		ArrayList<byte> bgraMiddle2PixelData;
		ArrayList<byte> bgraMiddle3PixelData;
		ArrayList<byte> bgraMiddle4PixelData;
		if (components == ColorSpectrumComponents.ValueSaturation || components == ColorSpectrumComponents.SaturationValue)
		{
			bgraMiddle1PixelData = new ArrayList<byte>(capacity);
			bgraMiddle2PixelData = new ArrayList<byte>(capacity);
			bgraMiddle3PixelData = new ArrayList<byte>(capacity);
			bgraMiddle4PixelData = new ArrayList<byte>(capacity);
		}
		else
		{
			bgraMiddle1PixelData = new ArrayList<byte>(0);
			bgraMiddle2PixelData = new ArrayList<byte>(0);
			bgraMiddle3PixelData = new ArrayList<byte>(0);
			bgraMiddle4PixelData = new ArrayList<byte>(0);
		}
		await Task.Run(delegate
		{
			if (shape == ColorSpectrumShape.Box)
			{
				for (int num3 = pixelDimension - 1; num3 >= 0; num3--)
				{
					for (int num4 = pixelDimension - 1; num4 >= 0; num4--)
					{
						FillPixelForBox(num3, num4, hsv, pixelDimension, components, minHue, maxHue, minSaturation, maxSaturation, minValue, maxValue, bgraMinPixelData, bgraMiddle1PixelData, bgraMiddle2PixelData, bgraMiddle3PixelData, bgraMiddle4PixelData, bgraMaxPixelData, newHsvValues);
					}
				}
			}
			else
			{
				for (int i = 0; i < pixelDimension; i++)
				{
					for (int j = 0; j < pixelDimension; j++)
					{
						FillPixelForRing(j, i, (double)pixelDimension / 2.0, hsv, components, minHue, maxHue, minSaturation, maxSaturation, minValue, maxValue, bgraMinPixelData, bgraMiddle1PixelData, bgraMiddle2PixelData, bgraMiddle3PixelData, bgraMiddle4PixelData, bgraMaxPixelData, newHsvValues);
					}
				}
			}
		});
		Dispatcher.UIThread.Post(delegate
		{
			int pixelWidth = pixelDimension;
			int pixelHeight = pixelDimension;
			ColorSpectrumComponents components2 = Components;
			_minBitmap = ColorPickerHelpers.CreateBitmapFromPixelData(bgraMinPixelData, pixelWidth, pixelHeight);
			_maxBitmap = ColorPickerHelpers.CreateBitmapFromPixelData(bgraMaxPixelData, pixelWidth, pixelHeight);
			switch (components2)
			{
			case ColorSpectrumComponents.HueValue:
			case ColorSpectrumComponents.ValueHue:
				_saturationMinimumBitmap = _minBitmap;
				_saturationMaximumBitmap = _maxBitmap;
				break;
			case ColorSpectrumComponents.HueSaturation:
			case ColorSpectrumComponents.SaturationHue:
				_valueBitmap = _maxBitmap;
				break;
			case ColorSpectrumComponents.SaturationValue:
			case ColorSpectrumComponents.ValueSaturation:
				_hueRedBitmap = _minBitmap;
				_hueYellowBitmap = ColorPickerHelpers.CreateBitmapFromPixelData(bgraMiddle1PixelData, pixelWidth, pixelHeight);
				_hueGreenBitmap = ColorPickerHelpers.CreateBitmapFromPixelData(bgraMiddle2PixelData, pixelWidth, pixelHeight);
				_hueCyanBitmap = ColorPickerHelpers.CreateBitmapFromPixelData(bgraMiddle3PixelData, pixelWidth, pixelHeight);
				_hueBlueBitmap = ColorPickerHelpers.CreateBitmapFromPixelData(bgraMiddle4PixelData, pixelWidth, pixelHeight);
				_huePurpleBitmap = _maxBitmap;
				break;
			}
			_shapeFromLastBitmapCreation = Shape;
			_componentsFromLastBitmapCreation = Components;
			_imageWidthFromLastBitmapCreation = pixelDimension;
			_imageHeightFromLastBitmapCreation = pixelDimension;
			_minHueFromLastBitmapCreation = MinHue;
			_maxHueFromLastBitmapCreation = MaxHue;
			_minSaturationFromLastBitmapCreation = MinSaturation;
			_maxSaturationFromLastBitmapCreation = MaxSaturation;
			_minValueFromLastBitmapCreation = MinValue;
			_maxValueFromLastBitmapCreation = MaxValue;
			_hsvValues = newHsvValues;
			UpdateBitmapSources();
			UpdateEllipse();
		});
	}

	private static void FillPixelForBox(double x, double y, Hsv baseHsv, int minDimension, ColorSpectrumComponents components, double minHue, double maxHue, double minSaturation, double maxSaturation, double minValue, double maxValue, ArrayList<byte> bgraMinPixelData, ArrayList<byte> bgraMiddle1PixelData, ArrayList<byte> bgraMiddle2PixelData, ArrayList<byte> bgraMiddle3PixelData, ArrayList<byte> bgraMiddle4PixelData, ArrayList<byte> bgraMaxPixelData, List<Hsv> newHsvValues)
	{
		double num = minSaturation / 100.0;
		double num2 = maxSaturation / 100.0;
		double num3 = minValue / 100.0;
		double num4 = maxValue / 100.0;
		Hsv item = baseHsv;
		Hsv hsv = baseHsv;
		Hsv hsv2 = baseHsv;
		Hsv hsv3 = baseHsv;
		Hsv hsv4 = baseHsv;
		Hsv hsv5 = baseHsv;
		double num5 = ((double)(minDimension - 1) - x) / (double)(minDimension - 1);
		double num6 = ((double)(minDimension - 1) - y) / (double)(minDimension - 1);
		switch (components)
		{
		case ColorSpectrumComponents.HueValue:
			item.H = (hsv.H = (hsv2.H = (hsv3.H = (hsv4.H = (hsv5.H = minHue + num6 * (maxHue - minHue))))));
			item.V = (hsv.V = (hsv2.V = (hsv3.V = (hsv4.V = (hsv5.V = num3 + num5 * (num4 - num3))))));
			item.S = 0.0;
			hsv5.S = 1.0;
			break;
		case ColorSpectrumComponents.HueSaturation:
			item.H = (hsv.H = (hsv2.H = (hsv3.H = (hsv4.H = (hsv5.H = minHue + num6 * (maxHue - minHue))))));
			item.S = (hsv.S = (hsv2.S = (hsv3.S = (hsv4.S = (hsv5.S = num + num5 * (num2 - num))))));
			item.V = 0.0;
			hsv5.V = 1.0;
			break;
		case ColorSpectrumComponents.ValueHue:
			item.V = (hsv.V = (hsv2.V = (hsv3.V = (hsv4.V = (hsv5.V = num3 + num6 * (num4 - num3))))));
			item.H = (hsv.H = (hsv2.H = (hsv3.H = (hsv4.H = (hsv5.H = minHue + num5 * (maxHue - minHue))))));
			item.S = 0.0;
			hsv5.S = 1.0;
			break;
		case ColorSpectrumComponents.ValueSaturation:
			item.V = (hsv.V = (hsv2.V = (hsv3.V = (hsv4.V = (hsv5.V = num3 + num6 * (num4 - num3))))));
			item.S = (hsv.S = (hsv2.S = (hsv3.S = (hsv4.S = (hsv5.S = num + num5 * (num2 - num))))));
			item.H = 0.0;
			hsv.H = 60.0;
			hsv2.H = 120.0;
			hsv3.H = 180.0;
			hsv4.H = 240.0;
			hsv5.H = 300.0;
			break;
		case ColorSpectrumComponents.SaturationHue:
			item.S = (hsv.S = (hsv2.S = (hsv3.S = (hsv4.S = (hsv5.S = num + num6 * (num2 - num))))));
			item.H = (hsv.H = (hsv2.H = (hsv3.H = (hsv4.H = (hsv5.H = minHue + num5 * (maxHue - minHue))))));
			item.V = 0.0;
			hsv5.V = 1.0;
			break;
		case ColorSpectrumComponents.SaturationValue:
			item.S = (hsv.S = (hsv2.S = (hsv3.S = (hsv4.S = (hsv5.S = num + num6 * (num2 - num))))));
			item.V = (hsv.V = (hsv2.V = (hsv3.V = (hsv4.V = (hsv5.V = num3 + num5 * (num4 - num3))))));
			item.H = 0.0;
			hsv.H = 60.0;
			hsv2.H = 120.0;
			hsv3.H = 180.0;
			hsv4.H = 240.0;
			hsv5.H = 300.0;
			break;
		}
		if (components == ColorSpectrumComponents.HueSaturation || components == ColorSpectrumComponents.SaturationHue)
		{
			item.S = num2 - item.S + num;
			hsv.S = num2 - hsv.S + num;
			hsv2.S = num2 - hsv2.S + num;
			hsv3.S = num2 - hsv3.S + num;
			hsv4.S = num2 - hsv4.S + num;
			hsv5.S = num2 - hsv5.S + num;
		}
		else
		{
			item.V = num4 - item.V + num3;
			hsv.V = num4 - hsv.V + num3;
			hsv2.V = num4 - hsv2.V + num3;
			hsv3.V = num4 - hsv3.V + num3;
			hsv4.V = num4 - hsv4.V + num3;
			hsv5.V = num4 - hsv5.V + num3;
		}
		newHsvValues.Add(item);
		Rgb rgb = item.ToRgb();
		bgraMinPixelData.Add((byte)Math.Round(rgb.B * 255.0));
		bgraMinPixelData.Add((byte)Math.Round(rgb.G * 255.0));
		bgraMinPixelData.Add((byte)Math.Round(rgb.R * 255.0));
		bgraMinPixelData.Add(byte.MaxValue);
		if (components == ColorSpectrumComponents.ValueSaturation || components == ColorSpectrumComponents.SaturationValue)
		{
			Rgb rgb2 = hsv.ToRgb();
			bgraMiddle1PixelData.Add((byte)Math.Round(rgb2.B * 255.0));
			bgraMiddle1PixelData.Add((byte)Math.Round(rgb2.G * 255.0));
			bgraMiddle1PixelData.Add((byte)Math.Round(rgb2.R * 255.0));
			bgraMiddle1PixelData.Add(byte.MaxValue);
			Rgb rgb3 = hsv2.ToRgb();
			bgraMiddle2PixelData.Add((byte)Math.Round(rgb3.B * 255.0));
			bgraMiddle2PixelData.Add((byte)Math.Round(rgb3.G * 255.0));
			bgraMiddle2PixelData.Add((byte)Math.Round(rgb3.R * 255.0));
			bgraMiddle2PixelData.Add(byte.MaxValue);
			Rgb rgb4 = hsv3.ToRgb();
			bgraMiddle3PixelData.Add((byte)Math.Round(rgb4.B * 255.0));
			bgraMiddle3PixelData.Add((byte)Math.Round(rgb4.G * 255.0));
			bgraMiddle3PixelData.Add((byte)Math.Round(rgb4.R * 255.0));
			bgraMiddle3PixelData.Add(byte.MaxValue);
			Rgb rgb5 = hsv4.ToRgb();
			bgraMiddle4PixelData.Add((byte)Math.Round(rgb5.B * 255.0));
			bgraMiddle4PixelData.Add((byte)Math.Round(rgb5.G * 255.0));
			bgraMiddle4PixelData.Add((byte)Math.Round(rgb5.R * 255.0));
			bgraMiddle4PixelData.Add(byte.MaxValue);
		}
		Rgb rgb6 = hsv5.ToRgb();
		bgraMaxPixelData.Add((byte)Math.Round(rgb6.B * 255.0));
		bgraMaxPixelData.Add((byte)Math.Round(rgb6.G * 255.0));
		bgraMaxPixelData.Add((byte)Math.Round(rgb6.R * 255.0));
		bgraMaxPixelData.Add(byte.MaxValue);
	}

	private void FillPixelForRing(double x, double y, double radius, Hsv baseHsv, ColorSpectrumComponents components, double minHue, double maxHue, double minSaturation, double maxSaturation, double minValue, double maxValue, ArrayList<byte> bgraMinPixelData, ArrayList<byte> bgraMiddle1PixelData, ArrayList<byte> bgraMiddle2PixelData, ArrayList<byte> bgraMiddle3PixelData, ArrayList<byte> bgraMiddle4PixelData, ArrayList<byte> bgraMaxPixelData, List<Hsv> newHsvValues)
	{
		double num = minSaturation / 100.0;
		double num2 = maxSaturation / 100.0;
		double num3 = minValue / 100.0;
		double num4 = maxValue / 100.0;
		double num5 = Math.Sqrt(Math.Pow(x - radius, 2.0) + Math.Pow(y - radius, 2.0));
		double num6 = x;
		double num7 = y;
		if (num5 > radius)
		{
			num6 = radius / num5 * (x - radius) + radius;
			num7 = radius / num5 * (y - radius) + radius;
			num5 = radius;
		}
		Hsv item = baseHsv;
		Hsv hsv = baseHsv;
		Hsv hsv2 = baseHsv;
		Hsv hsv3 = baseHsv;
		Hsv hsv4 = baseHsv;
		Hsv hsv5 = baseHsv;
		double num8 = 1.0 - num5 / radius;
		double num9 = Math.Atan2(radius - num7, radius - num6) * 180.0 / Math.PI;
		num9 += 180.0;
		for (num9 = Math.Floor(num9); num9 > 360.0; num9 -= 360.0)
		{
		}
		double num10 = num9 / 360.0;
		switch (components)
		{
		case ColorSpectrumComponents.HueValue:
			item.H = (hsv.H = (hsv2.H = (hsv3.H = (hsv4.H = (hsv5.H = minHue + num10 * (maxHue - minHue))))));
			item.V = (hsv.V = (hsv2.V = (hsv3.V = (hsv4.V = (hsv5.V = num3 + num8 * (num4 - num3))))));
			item.S = 0.0;
			hsv5.S = 1.0;
			break;
		case ColorSpectrumComponents.HueSaturation:
			item.H = (hsv.H = (hsv2.H = (hsv3.H = (hsv4.H = (hsv5.H = minHue + num10 * (maxHue - minHue))))));
			item.S = (hsv.S = (hsv2.S = (hsv3.S = (hsv4.S = (hsv5.S = num + num8 * (num2 - num))))));
			item.V = 0.0;
			hsv5.V = 1.0;
			break;
		case ColorSpectrumComponents.ValueHue:
			item.V = (hsv.V = (hsv2.V = (hsv3.V = (hsv4.V = (hsv5.V = num3 + num10 * (num4 - num3))))));
			item.H = (hsv.H = (hsv2.H = (hsv3.H = (hsv4.H = (hsv5.H = minHue + num8 * (maxHue - minHue))))));
			item.S = 0.0;
			hsv5.S = 1.0;
			break;
		case ColorSpectrumComponents.ValueSaturation:
			item.V = (hsv.V = (hsv2.V = (hsv3.V = (hsv4.V = (hsv5.V = num3 + num10 * (num4 - num3))))));
			item.S = (hsv.S = (hsv2.S = (hsv3.S = (hsv4.S = (hsv5.S = num + num8 * (num2 - num))))));
			item.H = 0.0;
			hsv.H = 60.0;
			hsv2.H = 120.0;
			hsv3.H = 180.0;
			hsv4.H = 240.0;
			hsv5.H = 300.0;
			break;
		case ColorSpectrumComponents.SaturationHue:
			item.S = (hsv.S = (hsv2.S = (hsv3.S = (hsv4.S = (hsv5.S = num + num10 * (num2 - num))))));
			item.H = (hsv.H = (hsv2.H = (hsv3.H = (hsv4.H = (hsv5.H = minHue + num8 * (maxHue - minHue))))));
			item.V = 0.0;
			hsv5.V = 1.0;
			break;
		case ColorSpectrumComponents.SaturationValue:
			item.S = (hsv.S = (hsv2.S = (hsv3.S = (hsv4.S = (hsv5.S = num + num10 * (num2 - num))))));
			item.V = (hsv.V = (hsv2.V = (hsv3.V = (hsv4.V = (hsv5.V = num3 + num8 * (num4 - num3))))));
			item.H = 0.0;
			hsv.H = 60.0;
			hsv2.H = 120.0;
			hsv3.H = 180.0;
			hsv4.H = 240.0;
			hsv5.H = 300.0;
			break;
		}
		if (components == ColorSpectrumComponents.HueSaturation || components == ColorSpectrumComponents.SaturationHue)
		{
			item.S = num2 - item.S + num;
			hsv.S = num2 - hsv.S + num;
			hsv2.S = num2 - hsv2.S + num;
			hsv3.S = num2 - hsv3.S + num;
			hsv4.S = num2 - hsv4.S + num;
			hsv5.S = num2 - hsv5.S + num;
		}
		else
		{
			item.V = num4 - item.V + num3;
			hsv.V = num4 - hsv.V + num3;
			hsv2.V = num4 - hsv2.V + num3;
			hsv3.V = num4 - hsv3.V + num3;
			hsv4.V = num4 - hsv4.V + num3;
			hsv5.V = num4 - hsv5.V + num3;
		}
		newHsvValues.Add(item);
		Rgb rgb = item.ToRgb();
		bgraMinPixelData.Add((byte)Math.Round(rgb.B * 255.0));
		bgraMinPixelData.Add((byte)Math.Round(rgb.G * 255.0));
		bgraMinPixelData.Add((byte)Math.Round(rgb.R * 255.0));
		bgraMinPixelData.Add(byte.MaxValue);
		if (components == ColorSpectrumComponents.ValueSaturation || components == ColorSpectrumComponents.SaturationValue)
		{
			Rgb rgb2 = hsv.ToRgb();
			bgraMiddle1PixelData.Add((byte)Math.Round(rgb2.B * 255.0));
			bgraMiddle1PixelData.Add((byte)Math.Round(rgb2.G * 255.0));
			bgraMiddle1PixelData.Add((byte)Math.Round(rgb2.R * 255.0));
			bgraMiddle1PixelData.Add(byte.MaxValue);
			Rgb rgb3 = hsv2.ToRgb();
			bgraMiddle2PixelData.Add((byte)Math.Round(rgb3.B * 255.0));
			bgraMiddle2PixelData.Add((byte)Math.Round(rgb3.G * 255.0));
			bgraMiddle2PixelData.Add((byte)Math.Round(rgb3.R * 255.0));
			bgraMiddle2PixelData.Add(byte.MaxValue);
			Rgb rgb4 = hsv3.ToRgb();
			bgraMiddle3PixelData.Add((byte)Math.Round(rgb4.B * 255.0));
			bgraMiddle3PixelData.Add((byte)Math.Round(rgb4.G * 255.0));
			bgraMiddle3PixelData.Add((byte)Math.Round(rgb4.R * 255.0));
			bgraMiddle3PixelData.Add(byte.MaxValue);
			Rgb rgb5 = hsv4.ToRgb();
			bgraMiddle4PixelData.Add((byte)Math.Round(rgb5.B * 255.0));
			bgraMiddle4PixelData.Add((byte)Math.Round(rgb5.G * 255.0));
			bgraMiddle4PixelData.Add((byte)Math.Round(rgb5.R * 255.0));
			bgraMiddle4PixelData.Add(byte.MaxValue);
		}
		Rgb rgb6 = hsv5.ToRgb();
		bgraMaxPixelData.Add((byte)Math.Round(rgb6.B * 255.0));
		bgraMaxPixelData.Add((byte)Math.Round(rgb6.G * 255.0));
		bgraMaxPixelData.Add((byte)Math.Round(rgb6.R * 255.0));
		bgraMaxPixelData.Add(byte.MaxValue);
	}

	private void UpdateBitmapSources()
	{
		if (_spectrumOverlayRectangle == null || _spectrumOverlayEllipse == null || _spectrumRectangle == null || _spectrumEllipse == null)
		{
			return;
		}
		HsvColor hsvColor = HsvColor;
		switch (Components)
		{
		case ColorSpectrumComponents.HueValue:
		case ColorSpectrumComponents.ValueHue:
			if (_saturationMinimumBitmap != null && _saturationMaximumBitmap != null)
			{
				ImageBrush fill3 = new ImageBrush(_saturationMinimumBitmap);
				ImageBrush fill4 = new ImageBrush(_saturationMaximumBitmap);
				_spectrumOverlayRectangle.Opacity = hsvColor.S;
				_spectrumOverlayEllipse.Opacity = hsvColor.S;
				_spectrumRectangle.Fill = fill3;
				_spectrumEllipse.Fill = fill3;
				_spectrumOverlayRectangle.Fill = fill4;
				_spectrumOverlayRectangle.Fill = fill4;
			}
			break;
		case ColorSpectrumComponents.HueSaturation:
		case ColorSpectrumComponents.SaturationHue:
			if (_valueBitmap != null)
			{
				ImageBrush fill5 = new ImageBrush(_valueBitmap);
				ImageBrush fill6 = new ImageBrush(_valueBitmap);
				_spectrumOverlayRectangle.Opacity = 1.0;
				_spectrumOverlayEllipse.Opacity = 1.0;
				_spectrumRectangle.Fill = fill5;
				_spectrumEllipse.Fill = fill5;
				_spectrumOverlayRectangle.Fill = fill6;
				_spectrumOverlayRectangle.Fill = fill6;
			}
			break;
		case ColorSpectrumComponents.SaturationValue:
		case ColorSpectrumComponents.ValueSaturation:
			if (_hueRedBitmap != null && _hueYellowBitmap != null && _hueGreenBitmap != null && _hueCyanBitmap != null && _hueBlueBitmap != null && _huePurpleBitmap != null)
			{
				double num = hsvColor.H / 60.0;
				ImageBrush fill;
				ImageBrush fill2;
				if (num < 1.0)
				{
					fill = new ImageBrush(_hueRedBitmap);
					fill2 = new ImageBrush(_hueYellowBitmap);
				}
				else if (num >= 1.0 && num < 2.0)
				{
					fill = new ImageBrush(_hueYellowBitmap);
					fill2 = new ImageBrush(_hueGreenBitmap);
				}
				else if (num >= 2.0 && num < 3.0)
				{
					fill = new ImageBrush(_hueGreenBitmap);
					fill2 = new ImageBrush(_hueCyanBitmap);
				}
				else if (num >= 3.0 && num < 4.0)
				{
					fill = new ImageBrush(_hueCyanBitmap);
					fill2 = new ImageBrush(_hueBlueBitmap);
				}
				else if (num >= 4.0 && num < 5.0)
				{
					fill = new ImageBrush(_hueBlueBitmap);
					fill2 = new ImageBrush(_huePurpleBitmap);
				}
				else
				{
					fill = new ImageBrush(_huePurpleBitmap);
					fill2 = new ImageBrush(_hueRedBitmap);
				}
				_spectrumOverlayRectangle.Opacity = num - (double)(int)num;
				_spectrumOverlayEllipse.Opacity = num - (double)(int)num;
				_spectrumRectangle.Fill = fill;
				_spectrumEllipse.Fill = fill;
				_spectrumOverlayRectangle.Fill = fill2;
				_spectrumOverlayRectangle.Fill = fill2;
			}
			break;
		}
	}

	private bool SelectionEllipseShouldBeLight()
	{
		Color color;
		if (Components == ColorSpectrumComponents.HueSaturation || Components == ColorSpectrumComponents.SaturationHue)
		{
			HsvColor hsvColor = HsvColor;
			color = new Hsv(hsvColor.H, hsvColor.S, 1.0).ToRgb().ToColor(hsvColor.A);
		}
		else
		{
			color = Color;
		}
		return ColorHelper.GetRelativeLuminance(color) <= 0.5;
	}
}
