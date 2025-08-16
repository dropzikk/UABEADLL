using System;
using System.Diagnostics;

namespace Avalonia.Media.TextFormatting;

[DebuggerTypeProxy(typeof(TextRunDebuggerProxy))]
public abstract class TextRun
{
	private class TextRunDebuggerProxy
	{
		private readonly TextRun _textRun;

		public string Text => _textRun.Text.ToString();

		public TextRunProperties? Properties => _textRun.Properties;

		public TextRunDebuggerProxy(TextRun textRun)
		{
			_textRun = textRun;
		}
	}

	public const int DefaultTextSourceLength = 1;

	public virtual int Length => 1;

	public virtual ReadOnlyMemory<char> Text => default(ReadOnlyMemory<char>);

	public virtual TextRunProperties? Properties => null;
}
