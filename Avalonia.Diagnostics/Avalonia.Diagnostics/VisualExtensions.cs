using System;
using System.IO;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.VisualTree;

namespace Avalonia.Diagnostics;

internal static class VisualExtensions
{
	public static void RenderTo(this Control source, Stream destination, double dpi = 96.0)
	{
		Matrix? matrix = source.CompositionVisual?.TryGetServerGlobalTransform();
		if (!matrix.HasValue)
		{
			return;
		}
		Rect rect = new Rect(source.Bounds.Size).TransformToAABB(matrix.Value);
		Point topLeft = rect.TopLeft;
		PixelSize pixelSize = new PixelSize((int)rect.Width, (int)rect.Height);
		Vector dpi2 = new Vector(dpi, dpi);
		Control control = ((source.VisualRoot ?? source.GetVisualRoot()) as Control) ?? source;
		IDisposable disposable = null;
		IDisposable disposable2 = null;
		IDisposable disposable3 = null;
		IDisposable disposable4 = null;
		try
		{
			RectangleGeometry value = new RectangleGeometry(rect);
			disposable2 = control.SetValue(Visual.ClipToBoundsProperty, value: true, BindingPriority.Animation);
			disposable = control.SetValue(Visual.ClipProperty, value, BindingPriority.Animation);
			disposable3 = control.SetValue(Visual.RenderTransformOriginProperty, new RelativePoint(topLeft, RelativeUnit.Absolute), BindingPriority.Animation);
			disposable4 = control.SetValue(Visual.RenderTransformProperty, new TranslateTransform(0.0 - topLeft.X, 0.0 - topLeft.Y), BindingPriority.Animation);
			using RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(pixelSize, dpi2);
			renderTargetBitmap.Render(control);
			renderTargetBitmap.Save(destination, null);
		}
		finally
		{
			disposable4?.Dispose();
			disposable3?.Dispose();
			disposable?.Dispose();
			disposable2?.Dispose();
			source?.InvalidateVisual();
		}
	}
}
