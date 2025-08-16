using Avalonia.Collections;
using Avalonia.LogicalTree;

namespace Avalonia.Controls.Presenters;

internal interface IContentPresenterHost
{
	IAvaloniaList<ILogical> LogicalChildren { get; }

	bool RegisterContentPresenter(ContentPresenter presenter);
}
