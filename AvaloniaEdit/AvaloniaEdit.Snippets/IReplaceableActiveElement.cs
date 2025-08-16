using System;

namespace AvaloniaEdit.Snippets;

public interface IReplaceableActiveElement : IActiveElement
{
	string Text { get; }

	event EventHandler TextChanged;
}
