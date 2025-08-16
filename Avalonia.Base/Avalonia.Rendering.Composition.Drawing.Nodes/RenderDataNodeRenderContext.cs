using System;
using System.Collections.Generic;
using Avalonia.Platform;
using Avalonia.Threading;

namespace Avalonia.Rendering.Composition.Drawing.Nodes;

internal struct RenderDataNodeRenderContext : IDisposable
{
	private Stack<Matrix>? _stack;

	private static readonly ThreadSafeObjectPool<Stack<Matrix>> s_matrixStackPool = new ThreadSafeObjectPool<Stack<Matrix>>();

	public IDrawingContextImpl Context { get; }

	public Stack<Matrix> MatrixStack => _stack ?? (_stack = s_matrixStackPool.Get());

	public RenderDataNodeRenderContext(IDrawingContextImpl context)
	{
		_stack = null;
		Context = context;
	}

	public void Dispose()
	{
		if (_stack != null)
		{
			_stack.Clear();
			s_matrixStackPool.ReturnAndSetNull(ref _stack);
		}
	}
}
