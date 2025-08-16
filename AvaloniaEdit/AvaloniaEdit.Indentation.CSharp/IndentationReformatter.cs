using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace AvaloniaEdit.Indentation.CSharp;

internal sealed class IndentationReformatter
{
	private struct Block
	{
		public string OuterIndent;

		public string InnerIndent;

		public string LastWord;

		public char Bracket;

		public bool Continuation;

		public int OneLineBlock;

		public int PreviousOneLineBlock;

		public int StartLine;

		public void ResetOneLineBlock()
		{
			PreviousOneLineBlock = OneLineBlock;
			OneLineBlock = 0;
		}

		public void Indent(IndentationSettings set)
		{
			Indent(set.IndentString);
		}

		public void Indent(string indentationString)
		{
			OuterIndent = InnerIndent;
			InnerIndent += indentationString;
			Continuation = false;
			ResetOneLineBlock();
			LastWord = "";
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "[Block StartLine={0}, LastWord='{1}', Continuation={2}, OneLineBlock={3}, PreviousOneLineBlock={4}]", StartLine, LastWord, Continuation, OneLineBlock, PreviousOneLineBlock);
		}
	}

	private StringBuilder _wordBuilder;

	private Stack<Block> _blocks;

	private Block _block;

	private bool _inString;

	private bool _inChar;

	private bool _verbatim;

	private bool _escape;

	private bool _lineComment;

	private bool _blockComment;

	private char _lastRealChar;

	public void Reformat(IDocumentAccessor doc, IndentationSettings set)
	{
		Init();
		while (doc.MoveNext())
		{
			Step(doc, set);
		}
	}

	public void Init()
	{
		_wordBuilder = new StringBuilder();
		_blocks = new Stack<Block>();
		_block = new Block
		{
			InnerIndent = "",
			OuterIndent = "",
			Bracket = '{',
			Continuation = false,
			LastWord = "",
			OneLineBlock = 0,
			PreviousOneLineBlock = 0,
			StartLine = 0
		};
		_inString = false;
		_inChar = false;
		_verbatim = false;
		_escape = false;
		_lineComment = false;
		_blockComment = false;
		_lastRealChar = ' ';
	}

	public void Step(IDocumentAccessor doc, IndentationSettings set)
	{
		string text = doc.Text;
		if (set.LeaveEmptyLines && text.Length == 0)
		{
			return;
		}
		text = text.TrimStart(Array.Empty<char>());
		StringBuilder stringBuilder = new StringBuilder();
		if (text.Length == 0)
		{
			if (!_blockComment && (!_inString || !_verbatim))
			{
				stringBuilder.Append(_block.InnerIndent);
				stringBuilder.Append(Repeat(set.IndentString, _block.OneLineBlock));
				if (doc.Text != stringBuilder.ToString())
				{
					doc.Text = stringBuilder.ToString();
				}
			}
			return;
		}
		if (TrimEnd(doc))
		{
			text = doc.Text.TrimStart(Array.Empty<char>());
		}
		Block block = _block;
		bool blockComment = _blockComment;
		bool flag = _inString && _verbatim;
		_lineComment = false;
		_inChar = false;
		_escape = false;
		if (!_verbatim)
		{
			_inString = false;
		}
		_lastRealChar = '\n';
		char c = ' ';
		char c2 = text[0];
		for (int i = 0; i < text.Length; i++)
		{
			if (_lineComment)
			{
				break;
			}
			char c3 = c;
			c = c2;
			c2 = ((i + 1 < text.Length) ? text[i + 1] : '\n');
			if (_escape)
			{
				_escape = false;
				continue;
			}
			switch (c)
			{
			case '/':
				if (_blockComment && c3 == '*')
				{
					_blockComment = false;
				}
				if (!_inString && !_inChar)
				{
					if (!_blockComment && c2 == '/')
					{
						_lineComment = true;
					}
					if (!_lineComment && c2 == '*')
					{
						_blockComment = true;
					}
				}
				break;
			case '#':
				if (!_inChar && !_blockComment && !_inString)
				{
					_lineComment = true;
				}
				break;
			case '"':
				if (_inChar || _lineComment || _blockComment)
				{
					break;
				}
				_inString = !_inString;
				if (!_inString && _verbatim)
				{
					if (c2 == '"')
					{
						_escape = true;
						_inString = true;
					}
					else
					{
						_verbatim = false;
					}
				}
				else if (_inString && c3 == '@')
				{
					_verbatim = true;
				}
				break;
			case '\'':
				if (!_inString && !_lineComment && !_blockComment)
				{
					_inChar = !_inChar;
				}
				break;
			case '\\':
				if ((_inString && !_verbatim) || _inChar)
				{
					_escape = true;
				}
				break;
			}
			if (_lineComment || _blockComment || _inString || _inChar)
			{
				if (_wordBuilder.Length > 0)
				{
					_block.LastWord = _wordBuilder.ToString();
				}
				_wordBuilder.Length = 0;
				continue;
			}
			if (!char.IsWhiteSpace(c) && c != '[' && c != '/' && _block.Bracket == '{')
			{
				_block.Continuation = true;
			}
			if (char.IsLetterOrDigit(c))
			{
				_wordBuilder.Append(c);
			}
			else
			{
				if (_wordBuilder.Length > 0)
				{
					_block.LastWord = _wordBuilder.ToString();
				}
				_wordBuilder.Length = 0;
			}
			switch (c)
			{
			case '{':
				_block.ResetOneLineBlock();
				_blocks.Push(_block);
				_block.StartLine = doc.LineNumber;
				if (_block.LastWord == "switch")
				{
					_block.Indent(set.IndentString + set.IndentString);
				}
				else
				{
					_block.Indent(set);
				}
				_block.Bracket = '{';
				break;
			case '}':
				while (_block.Bracket != '{' && _blocks.Count != 0)
				{
					_block = _blocks.Pop();
				}
				if (_blocks.Count != 0)
				{
					_block = _blocks.Pop();
					_block.Continuation = false;
					_block.ResetOneLineBlock();
				}
				break;
			case '(':
			case '[':
				_blocks.Push(_block);
				if (_block.StartLine == doc.LineNumber)
				{
					_block.InnerIndent = _block.OuterIndent;
				}
				else
				{
					_block.StartLine = doc.LineNumber;
				}
				_block.Indent(Repeat(set.IndentString, block.OneLineBlock) + (block.Continuation ? set.IndentString : "") + ((i == text.Length - 1) ? set.IndentString : new string(' ', i + 1)));
				_block.Bracket = c;
				break;
			case ')':
				if (_blocks.Count != 0 && _block.Bracket == '(')
				{
					_block = _blocks.Pop();
					if (IsSingleStatementKeyword(_block.LastWord))
					{
						_block.Continuation = false;
					}
				}
				break;
			case ']':
				if (_blocks.Count != 0 && _block.Bracket == '[')
				{
					_block = _blocks.Pop();
				}
				break;
			case ',':
			case ';':
				_block.Continuation = false;
				_block.ResetOneLineBlock();
				break;
			case ':':
				if (_block.LastWord == "case" || text.StartsWith("case ", StringComparison.Ordinal) || text.StartsWith(_block.LastWord + ":", StringComparison.Ordinal))
				{
					_block.Continuation = false;
					_block.ResetOneLineBlock();
				}
				break;
			}
			if (!char.IsWhiteSpace(c))
			{
				_lastRealChar = c;
			}
		}
		if (_wordBuilder.Length > 0)
		{
			_block.LastWord = _wordBuilder.ToString();
		}
		_wordBuilder.Length = 0;
		if (flag || (blockComment && text[0] != '*') || doc.Text.StartsWith("//\t", StringComparison.Ordinal) || doc.Text == "//")
		{
			return;
		}
		if (text[0] == '}')
		{
			stringBuilder.Append(block.OuterIndent);
			block.ResetOneLineBlock();
			block.Continuation = false;
		}
		else
		{
			stringBuilder.Append(block.InnerIndent);
		}
		if (stringBuilder.Length > 0 && block.Bracket == '(' && text[0] == ')')
		{
			stringBuilder.Remove(stringBuilder.Length - 1, 1);
		}
		else if (stringBuilder.Length > 0 && block.Bracket == '[' && text[0] == ']')
		{
			stringBuilder.Remove(stringBuilder.Length - 1, 1);
		}
		if (text[0] == ':')
		{
			block.Continuation = true;
		}
		else if (_lastRealChar == ':' && stringBuilder.Length >= set.IndentString.Length)
		{
			if (_block.LastWord == "case" || text.StartsWith("case ", StringComparison.Ordinal) || text.StartsWith(_block.LastWord + ":", StringComparison.Ordinal))
			{
				stringBuilder.Remove(stringBuilder.Length - set.IndentString.Length, set.IndentString.Length);
			}
		}
		else if (_lastRealChar == ')')
		{
			if (IsSingleStatementKeyword(_block.LastWord))
			{
				_block.OneLineBlock++;
			}
		}
		else if (_lastRealChar == 'e' && _block.LastWord == "else")
		{
			_block.OneLineBlock = Math.Max(1, _block.PreviousOneLineBlock);
			_block.Continuation = false;
			block.OneLineBlock = _block.OneLineBlock - 1;
		}
		if (doc.IsReadOnly)
		{
			if (block.Continuation || block.OneLineBlock != 0 || block.StartLine != _block.StartLine || _block.StartLine >= doc.LineNumber || _lastRealChar == ':')
			{
				return;
			}
			stringBuilder.Length = 0;
			text = doc.Text;
			string text2 = text;
			foreach (char c4 in text2)
			{
				if (!char.IsWhiteSpace(c4))
				{
					break;
				}
				stringBuilder.Append(c4);
			}
			if (blockComment && stringBuilder.Length > 0 && stringBuilder[stringBuilder.Length - 1] == ' ')
			{
				stringBuilder.Length--;
			}
			_block.InnerIndent = stringBuilder.ToString();
			return;
		}
		if (text[0] != '{')
		{
			if (text[0] != ')' && block.Continuation && block.Bracket == '{')
			{
				stringBuilder.Append(set.IndentString);
			}
			stringBuilder.Append(Repeat(set.IndentString, block.OneLineBlock));
		}
		if (blockComment)
		{
			stringBuilder.Append(' ');
		}
		if (stringBuilder.Length != doc.Text.Length - text.Length || !doc.Text.StartsWith(stringBuilder.ToString(), StringComparison.Ordinal) || char.IsWhiteSpace(doc.Text[stringBuilder.Length]))
		{
			doc.Text = stringBuilder?.ToString() + text;
		}
	}

	private static string Repeat(string text, int count)
	{
		switch (count)
		{
		case 0:
			return string.Empty;
		case 1:
			return text;
		default:
		{
			StringBuilder stringBuilder = new StringBuilder(text.Length * count);
			for (int i = 0; i < count; i++)
			{
				stringBuilder.Append(text);
			}
			return stringBuilder.ToString();
		}
		}
	}

	private static bool IsSingleStatementKeyword(string keyword)
	{
		switch (keyword)
		{
		case "do":
		case "if":
		case "using":
		case "while":
		case "for":
		case "foreach":
		case "lock":
			return true;
		default:
			return false;
		}
	}

	private static bool TrimEnd(IDocumentAccessor doc)
	{
		string text = doc.Text;
		if (!char.IsWhiteSpace(text[text.Length - 1]))
		{
			return false;
		}
		if (text.EndsWith("// ", StringComparison.Ordinal) || text.EndsWith("* ", StringComparison.Ordinal))
		{
			return false;
		}
		doc.Text = text.TrimEnd(Array.Empty<char>());
		return true;
	}
}
