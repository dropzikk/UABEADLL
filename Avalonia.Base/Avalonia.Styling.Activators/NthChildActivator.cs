using Avalonia.LogicalTree;

namespace Avalonia.Styling.Activators;

internal sealed class NthChildActivator : StyleActivatorBase
{
	private readonly ILogical _control;

	private readonly IChildIndexProvider _provider;

	private readonly int _step;

	private readonly int _offset;

	private readonly bool _reversed;

	private int? _index;

	public NthChildActivator(ILogical control, IChildIndexProvider provider, int step, int offset, bool reversed)
	{
		_control = control;
		_provider = provider;
		_step = step;
		_offset = offset;
		_reversed = reversed;
	}

	protected override bool EvaluateIsActive()
	{
		return NthChildSelector.Evaluate(_index ?? _provider.GetChildIndex(_control), _provider, _step, _offset, _reversed).IsMatch;
	}

	protected override void Initialize()
	{
		_provider.ChildIndexChanged += ChildIndexChanged;
	}

	protected override void Deinitialize()
	{
		_provider.ChildIndexChanged -= ChildIndexChanged;
	}

	private void ChildIndexChanged(object? sender, ChildIndexChangedEventArgs e)
	{
		switch (e.Action)
		{
		case ChildIndexChangedAction.ChildIndexChanged:
			if (e.Child == _control)
			{
				_index = e.Index;
				ReevaluateIsActive();
				_index = null;
			}
			break;
		case ChildIndexChangedAction.TotalCountChanged:
			if (!_reversed)
			{
				break;
			}
			goto case ChildIndexChangedAction.ChildIndexesReset;
		case ChildIndexChangedAction.ChildIndexesReset:
			_index = null;
			ReevaluateIsActive();
			break;
		}
	}
}
