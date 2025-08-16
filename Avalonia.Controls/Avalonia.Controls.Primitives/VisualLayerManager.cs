using System.Collections.Generic;
using Avalonia.LogicalTree;

namespace Avalonia.Controls.Primitives;

public class VisualLayerManager : Decorator
{
	private const int AdornerZIndex = 2147483547;

	private const int ChromeZIndex = 2147483548;

	private const int LightDismissOverlayZIndex = 2147483549;

	private const int OverlayZIndex = 2147483550;

	private ILogicalRoot? _logicalRoot;

	private readonly List<Control> _layers = new List<Control>();

	public static readonly StyledProperty<ChromeOverlayLayer?> ChromeOverlayLayerProperty = AvaloniaProperty.Register<VisualLayerManager, ChromeOverlayLayer>("ChromeOverlayLayer");

	public bool IsPopup { get; set; }

	public AdornerLayer AdornerLayer
	{
		get
		{
			AdornerLayer adornerLayer = FindLayer<AdornerLayer>();
			if (adornerLayer == null)
			{
				AddLayer(adornerLayer = new AdornerLayer(), 2147483547);
			}
			return adornerLayer;
		}
	}

	public ChromeOverlayLayer ChromeOverlayLayer
	{
		get
		{
			ChromeOverlayLayer chromeOverlayLayer = GetValue(ChromeOverlayLayerProperty);
			if (chromeOverlayLayer == null)
			{
				ChromeOverlayLayer chromeOverlayLayer2 = new ChromeOverlayLayer();
				AddLayer(chromeOverlayLayer2, 2147483548);
				SetValue(ChromeOverlayLayerProperty, chromeOverlayLayer2);
				chromeOverlayLayer = chromeOverlayLayer2;
			}
			return chromeOverlayLayer;
		}
	}

	public OverlayLayer? OverlayLayer
	{
		get
		{
			if (IsPopup)
			{
				return null;
			}
			OverlayLayer overlayLayer = FindLayer<OverlayLayer>();
			if (overlayLayer == null)
			{
				AddLayer(overlayLayer = new OverlayLayer(), 2147483550);
			}
			return overlayLayer;
		}
	}

	public LightDismissOverlayLayer LightDismissOverlayLayer
	{
		get
		{
			LightDismissOverlayLayer lightDismissOverlayLayer = FindLayer<LightDismissOverlayLayer>();
			if (lightDismissOverlayLayer == null)
			{
				lightDismissOverlayLayer = new LightDismissOverlayLayer
				{
					IsVisible = false
				};
				AddLayer(lightDismissOverlayLayer, 2147483549);
			}
			return lightDismissOverlayLayer;
		}
	}

	private T? FindLayer<T>() where T : class
	{
		foreach (Control layer in _layers)
		{
			if (layer is T result)
			{
				return result;
			}
		}
		return null;
	}

	private void AddLayer(Control layer, int zindex)
	{
		_layers.Add(layer);
		((ISetLogicalParent)layer).SetParent((ILogical?)this);
		layer.ZIndex = zindex;
		base.VisualChildren.Add(layer);
		if (((ILogical)this).IsAttachedToLogicalTree)
		{
			((ILogical)layer).NotifyAttachedToLogicalTree(new LogicalTreeAttachmentEventArgs(_logicalRoot, layer, this));
		}
		InvalidateArrange();
	}

	internal override void NotifyChildResourcesChanged(ResourcesChangedEventArgs e)
	{
		foreach (Control layer in _layers)
		{
			((ILogical)layer).NotifyResourcesChanged(e);
		}
		base.NotifyChildResourcesChanged(e);
	}

	protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
	{
		base.OnAttachedToLogicalTree(e);
		_logicalRoot = e.Root;
		foreach (Control layer in _layers)
		{
			((ILogical)layer).NotifyAttachedToLogicalTree(e);
		}
	}

	protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
	{
		_logicalRoot = null;
		base.OnDetachedFromLogicalTree(e);
		foreach (Control layer in _layers)
		{
			((ILogical)layer).NotifyDetachedFromLogicalTree(e);
		}
	}

	protected override Size MeasureOverride(Size availableSize)
	{
		foreach (Control layer in _layers)
		{
			layer.Measure(availableSize);
		}
		return base.MeasureOverride(availableSize);
	}

	protected override Size ArrangeOverride(Size finalSize)
	{
		foreach (Control layer in _layers)
		{
			layer.Arrange(new Rect(finalSize));
		}
		return base.ArrangeOverride(finalSize);
	}
}
