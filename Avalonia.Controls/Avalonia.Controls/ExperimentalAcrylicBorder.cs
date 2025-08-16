using System;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Reactive;
using Avalonia.Rendering.Composition;

namespace Avalonia.Controls;

public class ExperimentalAcrylicBorder : Decorator
{
	public static readonly StyledProperty<CornerRadius> CornerRadiusProperty;

	public static readonly StyledProperty<ExperimentalAcrylicMaterial> MaterialProperty;

	private IDisposable? _subscription;

	private IDisposable? _materialSubscription;

	public CornerRadius CornerRadius
	{
		get
		{
			return GetValue(CornerRadiusProperty);
		}
		set
		{
			SetValue(CornerRadiusProperty, value);
		}
	}

	public ExperimentalAcrylicMaterial Material
	{
		get
		{
			return GetValue(MaterialProperty);
		}
		set
		{
			SetValue(MaterialProperty, value);
		}
	}

	static ExperimentalAcrylicBorder()
	{
		CornerRadiusProperty = Border.CornerRadiusProperty.AddOwner<ExperimentalAcrylicBorder>();
		MaterialProperty = AvaloniaProperty.Register<ExperimentalAcrylicBorder, ExperimentalAcrylicMaterial>("Material");
		Visual.AffectsRender<ExperimentalAcrylicBorder>(new AvaloniaProperty[2] { MaterialProperty, CornerRadiusProperty });
	}

	protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
	{
		base.OnAttachedToVisualTree(e);
		TopLevel tl = (TopLevel)e.Root;
		_subscription = tl.GetObservable(TopLevel.ActualTransparencyLevelProperty).Subscribe(delegate(WindowTransparencyLevel x)
		{
			if (tl.PlatformImpl != null)
			{
				if (x == WindowTransparencyLevel.Transparent || x == WindowTransparencyLevel.None)
				{
					Material.PlatformTransparencyCompensationLevel = tl.PlatformImpl.AcrylicCompensationLevels.TransparentLevel;
				}
				else if (x == WindowTransparencyLevel.Blur)
				{
					Material.PlatformTransparencyCompensationLevel = tl.PlatformImpl.AcrylicCompensationLevels.BlurLevel;
				}
				else if (x == WindowTransparencyLevel.AcrylicBlur)
				{
					Material.PlatformTransparencyCompensationLevel = tl.PlatformImpl.AcrylicCompensationLevels.AcrylicBlurLevel;
				}
			}
		});
		UpdateMaterialSubscription();
	}

	private void UpdateMaterialSubscription()
	{
		_materialSubscription?.Dispose();
		_materialSubscription = null;
		if (base.CompositionVisual != null && Material != null)
		{
			_materialSubscription = Observable.FromEventPattern(delegate(EventHandler<AvaloniaPropertyChangedEventArgs> h)
			{
				Material.PropertyChanged += h;
			}, delegate(EventHandler<AvaloniaPropertyChangedEventArgs> h)
			{
				Material.PropertyChanged -= h;
			}).Subscribe(delegate
			{
				UpdateMaterialSubscription();
			});
			SyncMaterial(base.CompositionVisual);
		}
	}

	private void SyncMaterial(CompositionVisual? visual)
	{
		if (visual is CompositionExperimentalAcrylicVisual compositionExperimentalAcrylicVisual)
		{
			compositionExperimentalAcrylicVisual.CornerRadius = CornerRadius;
			compositionExperimentalAcrylicVisual.Material = (ImmutableExperimentalAcrylicMaterial)(object)Material.ToImmutable();
		}
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		if (change.Property == MaterialProperty)
		{
			UpdateMaterialSubscription();
		}
		if (change.Property == CornerRadiusProperty)
		{
			SyncMaterial(base.CompositionVisual);
		}
		base.OnPropertyChanged(change);
	}

	protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
	{
		base.OnDetachedFromVisualTree(e);
		UpdateMaterialSubscription();
		_subscription?.Dispose();
	}

	private protected override CompositionDrawListVisual CreateCompositionVisual(Compositor compositor)
	{
		CompositionExperimentalAcrylicVisual compositionExperimentalAcrylicVisual = new CompositionExperimentalAcrylicVisual(compositor, this);
		SyncMaterial(compositionExperimentalAcrylicVisual);
		return compositionExperimentalAcrylicVisual;
	}

	protected override Size MeasureOverride(Size availableSize)
	{
		return LayoutHelper.MeasureChild(base.Child, availableSize, base.Padding);
	}

	protected override Size ArrangeOverride(Size finalSize)
	{
		return LayoutHelper.ArrangeChild(base.Child, finalSize, base.Padding);
	}
}
