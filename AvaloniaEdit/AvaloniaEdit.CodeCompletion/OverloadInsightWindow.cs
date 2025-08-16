using Avalonia;
using Avalonia.Input;
using AvaloniaEdit.Editing;

namespace AvaloniaEdit.CodeCompletion;

public class OverloadInsightWindow : InsightWindow
{
	private readonly OverloadViewer _overloadViewer = new OverloadViewer();

	public IOverloadProvider Provider
	{
		get
		{
			return _overloadViewer.Provider;
		}
		set
		{
			_overloadViewer.Provider = value;
		}
	}

	public OverloadInsightWindow(TextArea textArea)
		: base(textArea)
	{
		_overloadViewer.Margin = new Thickness(2.0, 0.0, 0.0, 0.0);
		base.Child = _overloadViewer;
	}

	protected override void OnKeyDown(KeyEventArgs e)
	{
		base.OnKeyDown(e);
		if (!e.Handled && Provider != null && Provider.Count > 1)
		{
			switch (e.Key)
			{
			case Key.Up:
				e.Handled = true;
				_overloadViewer.ChangeIndex(-1);
				break;
			case Key.Down:
				e.Handled = true;
				_overloadViewer.ChangeIndex(1);
				break;
			}
			if (e.Handled)
			{
				UpdatePosition();
			}
		}
	}
}
