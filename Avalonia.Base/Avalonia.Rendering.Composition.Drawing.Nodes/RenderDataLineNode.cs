using System;
using Avalonia.Media;
using Avalonia.Rendering.SceneGraph;

namespace Avalonia.Rendering.Composition.Drawing.Nodes;

internal class RenderDataLineNode : IRenderDataItemWithServerResources, IRenderDataItem
{
	public IPen? ServerPen { get; set; }

	public IPen? ClientPen { get; set; }

	public Point P1 { get; set; }

	public Point P2 { get; set; }

	public Rect? Bounds => LineBoundsHelper.CalculateBounds(P1, P2, ServerPen);

	public bool HitTest(Point p)
	{
		if (ClientPen == null)
		{
			return false;
		}
		double num = ClientPen.Thickness / 2.0;
		double num2 = Math.Min(P1.X, P2.X) - num;
		double num3 = Math.Max(P1.X, P2.X) + num;
		double num4 = Math.Min(P1.Y, P2.Y) - num;
		double num5 = Math.Max(P1.Y, P2.Y) + num;
		if (p.X < num2 || p.X > num3 || p.Y < num4 || p.Y > num5)
		{
			return false;
		}
		Point p2 = P1;
		Point p3 = P2;
		Vector b = p - p2;
		if (Vector.Dot(p3 - p2, b) < 0.0)
		{
			return b.Length <= ClientPen.Thickness / 2.0;
		}
		Vector b2 = p - p3;
		if (Vector.Dot(p2 - p3, b2) < 0.0)
		{
			return b2.Length <= num;
		}
		double num6 = p3.X - p2.X;
		double num7 = p3.Y - p2.Y;
		return Math.Abs((num6 * (p.Y - p2.Y) - num7 * (p.X - p2.X)) / Math.Sqrt(num6 * num6 + num7 * num7)) <= num;
	}

	public void Invoke(ref RenderDataNodeRenderContext context)
	{
		context.Context.DrawLine(ServerPen, P1, P2);
	}

	public void Collect(IRenderDataServerResourcesCollector collector)
	{
		collector.AddRenderDataServerResource(ServerPen);
	}
}
