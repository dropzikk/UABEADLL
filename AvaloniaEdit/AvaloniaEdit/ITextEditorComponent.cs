using System;
using System.ComponentModel;
using AvaloniaEdit.Document;

namespace AvaloniaEdit;

public interface ITextEditorComponent : IServiceProvider
{
	TextDocument Document { get; }

	TextEditorOptions Options { get; }

	event EventHandler<DocumentChangedEventArgs> DocumentChanged;

	event PropertyChangedEventHandler OptionChanged;
}
