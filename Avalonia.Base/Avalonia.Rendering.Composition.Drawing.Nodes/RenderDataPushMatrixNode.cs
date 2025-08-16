namespace Avalonia.Rendering.Composition.Drawing.Nodes;

internal class RenderDataPushMatrixNode : RenderDataPushNode
{
	public Matrix Matrix { get; set; }

	public override Rect? Bounds => base.Bounds?.TransformToAABB(Matrix);

	public override void Push(ref RenderDataNodeRenderContext context)
	{
		Matrix transform = context.Context.Transform;
		context.MatrixStack.Push(transform);
		context.Context.Transform = Matrix * transform;
	}

	public override void Pop(ref RenderDataNodeRenderContext context)
	{
		context.Context.Transform = context.MatrixStack.Pop();
	}

	public override bool HitTest(Point p)
	{
		if (Matrix.TryInvert(out var inverted))
		{
			return base.HitTest(p.Transform(inverted));
		}
		return false;
	}
}
