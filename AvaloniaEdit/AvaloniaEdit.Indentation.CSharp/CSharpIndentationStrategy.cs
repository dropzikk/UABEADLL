using System;
using AvaloniaEdit.Document;

namespace AvaloniaEdit.Indentation.CSharp;

public class CSharpIndentationStrategy : DefaultIndentationStrategy
{
	private string _indentationString = "\t";

	public string IndentationString
	{
		get
		{
			return _indentationString;
		}
		set
		{
			if (string.IsNullOrEmpty(value))
			{
				throw new ArgumentException("Indentation string must not be null or empty");
			}
			_indentationString = value;
		}
	}

	public CSharpIndentationStrategy()
	{
	}

	public CSharpIndentationStrategy(TextEditorOptions options)
	{
		IndentationString = options.IndentationString;
	}

	public void Indent(IDocumentAccessor document, bool keepEmptyLines)
	{
		if (document == null)
		{
			throw new ArgumentNullException("document");
		}
		IndentationSettings set = new IndentationSettings
		{
			IndentString = IndentationString,
			LeaveEmptyLines = keepEmptyLines
		};
		new IndentationReformatter().Reformat(document, set);
	}

	public override void IndentLine(TextDocument document, DocumentLine line)
	{
		int lineNumber = line.LineNumber;
		TextDocumentAccessor textDocumentAccessor = new TextDocumentAccessor(document, lineNumber, lineNumber);
		Indent(textDocumentAccessor, keepEmptyLines: false);
		if (textDocumentAccessor.Text.Length == 0)
		{
			base.IndentLine(document, line);
		}
	}

	public override void IndentLines(TextDocument document, int beginLine, int endLine)
	{
		Indent(new TextDocumentAccessor(document, beginLine, endLine), keepEmptyLines: true);
	}
}
