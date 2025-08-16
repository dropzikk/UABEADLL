using System;

namespace Avalonia.Rendering.Composition.Drawing.Nodes;

internal class RenderDataEllipseNode : RenderDataBrushAndPenNode
{
	public Rect Rect { get; set; }

	public override Rect? Bounds => Rect.Inflate(base.ServerPen?.Thickness ?? 0.0);

	private bool Contains(double dx, double dy, double radiusX, double radiusY)
	{
		double num = radiusX * radiusX;
		double num2 = radiusY * radiusY;
		return num2 * dx * dx + num * dy * dy <= num * num2;
	}

	public override bool HitTest(Point p)
	{
		Point center = Rect.Center;
		double num = base.ClientPen?.Thickness ?? 0.0;
		double num2 = Rect.Width / 2.0 + num / 2.0;
		double num3 = Rect.Height / 2.0 + num / 2.0;
		double num4 = p.X - center.X;
		double num5 = p.Y - center.Y;
		if (Math.Abs(num4) > num2 || Math.Abs(num5) > num3)
		{
			return false;
		}
		if (base.ServerBrush != null)
		{
			return Contains(num4, num5, num2, num3);
		}
		if (num > 0.0)
		{
			bool num6 = Contains(num4, num5, num2, num3);
			num2 = Rect.Width / 2.0 - num / 2.0;
			num3 = Rect.Height / 2.0 - num / 2.0;
			bool flag = Contains(num4, num5, num2, num3);
			if (num6)
			{
				return !flag;
			}
			return false;
		}
		return false;
	}

	public override void Invoke(ref RenderDataNodeRenderContext context)
	{
		context.Context.DrawEllipse(base.ServerBrush, base.ServerPen, Rect);
	}
}
