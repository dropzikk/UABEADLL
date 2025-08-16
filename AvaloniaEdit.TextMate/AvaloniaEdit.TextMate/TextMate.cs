using System;
using System.Linq;
using TextMateSharp.Grammars;
using TextMateSharp.Model;
using TextMateSharp.Registry;
using TextMateSharp.Themes;

namespace AvaloniaEdit.TextMate;

public static class TextMate
{
	public class Installation
	{
		private IRegistryOptions _textMateRegistryOptions;

		private Registry _textMateRegistry;

		private TextEditor _editor;

		private TextEditorModel _editorModel;

		private IGrammar _grammar;

		private TMModel _tmModel;

		private TextMateColoringTransformer _transformer;

		public IRegistryOptions RegistryOptions => _textMateRegistryOptions;

		public TextEditorModel EditorModel => _editorModel;

		public Installation(TextEditor editor, IRegistryOptions registryOptions, bool initCurrentDocument = true)
		{
			_textMateRegistryOptions = registryOptions;
			_textMateRegistry = new Registry(registryOptions);
			_editor = editor;
			SetTheme(registryOptions.GetDefaultTheme());
			editor.DocumentChanged += OnEditorOnDocumentChanged;
			if (initCurrentDocument)
			{
				OnEditorOnDocumentChanged(editor, EventArgs.Empty);
			}
		}

		public void SetGrammar(string scopeName)
		{
			_grammar = _textMateRegistry.LoadGrammar(scopeName);
			GetOrCreateTransformer().SetGrammar(_grammar);
			_editor.TextArea.TextView.Redraw();
		}

		public void SetTheme(IRawTheme theme)
		{
			_textMateRegistry.SetTheme(theme);
			GetOrCreateTransformer().SetTheme(_textMateRegistry.GetTheme());
			_tmModel?.InvalidateLine(0);
			_editorModel?.InvalidateViewPortLines();
		}

		public void Dispose()
		{
			_editor.DocumentChanged -= OnEditorOnDocumentChanged;
			DisposeEditorModel(_editorModel);
			DisposeTMModel(_tmModel, _transformer);
			DisposeTransformer(_transformer);
		}

		private void OnEditorOnDocumentChanged(object sender, EventArgs args)
		{
			try
			{
				DisposeEditorModel(_editorModel);
				DisposeTMModel(_tmModel, _transformer);
				_editorModel = new TextEditorModel(_editor.TextArea.TextView, _editor.Document, _exceptionHandler);
				_tmModel = new TMModel(_editorModel);
				_tmModel.SetGrammar(_grammar);
				_transformer = GetOrCreateTransformer();
				_transformer.SetModel(_editor.Document, _tmModel);
				_tmModel.AddModelTokensChangedListener(_transformer);
			}
			catch (Exception obj)
			{
				_exceptionHandler?.Invoke(obj);
			}
		}

		private TextMateColoringTransformer GetOrCreateTransformer()
		{
			TextMateColoringTransformer textMateColoringTransformer = _editor.TextArea.TextView.LineTransformers.OfType<TextMateColoringTransformer>().FirstOrDefault();
			if (textMateColoringTransformer == null)
			{
				textMateColoringTransformer = new TextMateColoringTransformer(_editor.TextArea.TextView, _exceptionHandler);
				_editor.TextArea.TextView.LineTransformers.Add(textMateColoringTransformer);
			}
			return textMateColoringTransformer;
		}

		private static void DisposeTransformer(TextMateColoringTransformer transformer)
		{
			transformer?.Dispose();
		}

		private static void DisposeTMModel(TMModel tmModel, TextMateColoringTransformer transformer)
		{
			if (tmModel != null)
			{
				if (transformer != null)
				{
					tmModel.RemoveModelTokensChangedListener(transformer);
				}
				tmModel.Dispose();
			}
		}

		private static void DisposeEditorModel(TextEditorModel editorModel)
		{
			editorModel?.Dispose();
		}
	}

	private static Action<Exception> _exceptionHandler;

	public static void RegisterExceptionHandler(Action<Exception> handler)
	{
		_exceptionHandler = handler;
	}

	public static Installation InstallTextMate(this TextEditor editor, IRegistryOptions registryOptions, bool initCurrentDocument = true)
	{
		return new Installation(editor, registryOptions, initCurrentDocument);
	}
}
