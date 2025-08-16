using Avalonia.Controls;
using Avalonia.Media;

namespace AvaloniaEdit.Rendering;

internal class Layer : Control
{
	protected readonly TextView TextView;

	protected readonly KnownLayer KnownLayer;

	public Layer(TextView textView, KnownLayer knownLayer)
	{
		TextView = textView;
		KnownLayer = knownLayer;
		base.Focusable = false;
		base.IsHitTestVisible = false;
	}

	public override void Render(DrawingContext context)
	{
		base.Render(context);
		TextView.RenderBackground(context, KnownLayer);
	}
}
