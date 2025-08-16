using System;

namespace AvaloniaEdit.Utils;

internal sealed class FunctionNode<T> : RopeNode<T>
{
	private Func<Rope<T>> _initializer;

	private RopeNode<T> _cachedResults;

	public FunctionNode(int length, Func<Rope<T>> initializer)
		: base(isShared: true)
	{
		Length = length;
		_initializer = initializer;
	}

	internal override RopeNode<T> GetContentNode()
	{
		lock (this)
		{
			if (_cachedResults == null)
			{
				if (_initializer == null)
				{
					throw new InvalidOperationException("Trying to load this node recursively; or: a previous call to a rope initializer failed.");
				}
				Func<Rope<T>> initializer = _initializer;
				_initializer = null;
				RopeNode<T> root = (initializer() ?? throw new InvalidOperationException("Rope initializer returned null.")).Root;
				root.Publish();
				if (root.Length != Length)
				{
					throw new InvalidOperationException("Rope initializer returned rope with incorrect length.");
				}
				if (root.Height == 0 && root.Contents == null)
				{
					_cachedResults = root.GetContentNode();
				}
				else
				{
					_cachedResults = root;
				}
			}
			return _cachedResults;
		}
	}
}
