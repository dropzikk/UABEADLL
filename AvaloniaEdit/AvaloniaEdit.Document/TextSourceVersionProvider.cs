using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AvaloniaEdit.Utils;

namespace AvaloniaEdit.Document;

public class TextSourceVersionProvider
{
	[DebuggerDisplay("Version #{_id}")]
	private sealed class Version : ITextSourceVersion
	{
		private readonly TextSourceVersionProvider _provider;

		private readonly int _id;

		internal TextChangeEventArgs Change;

		internal Version Next;

		internal Version(TextSourceVersionProvider provider)
		{
			_provider = provider;
		}

		internal Version(Version prev)
		{
			_provider = prev._provider;
			_id = prev._id + 1;
		}

		public bool BelongsToSameDocumentAs(ITextSourceVersion other)
		{
			if (other is Version version)
			{
				return _provider == version._provider;
			}
			return false;
		}

		public int CompareAge(ITextSourceVersion other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			if (!(other is Version version) || _provider != version._provider)
			{
				throw new ArgumentException("Versions do not belong to the same document.");
			}
			return Math.Sign(_id - version._id);
		}

		public IEnumerable<TextChangeEventArgs> GetChangesTo(ITextSourceVersion other)
		{
			int num = CompareAge(other);
			Version version = (Version)other;
			if (num < 0)
			{
				return GetForwardChanges(version);
			}
			if (num > 0)
			{
				return from c in version.GetForwardChanges(this).Reverse()
					select c.Invert();
			}
			return Empty<TextChangeEventArgs>.Array;
		}

		private IEnumerable<TextChangeEventArgs> GetForwardChanges(Version other)
		{
			for (Version node = this; node != other; node = node.Next)
			{
				yield return node.Change;
			}
		}

		public int MoveOffsetTo(ITextSourceVersion other, int oldOffset, AnchorMovementType movement)
		{
			return GetChangesTo(other).Aggregate(oldOffset, (int current, TextChangeEventArgs e) => e.GetNewOffset(current, movement));
		}
	}

	private Version _currentVersion;

	public ITextSourceVersion CurrentVersion => _currentVersion;

	public TextSourceVersionProvider()
	{
		_currentVersion = new Version(this);
	}

	public void AppendChange(TextChangeEventArgs change)
	{
		_currentVersion.Change = change ?? throw new ArgumentNullException("change");
		_currentVersion.Next = new Version(_currentVersion);
		_currentVersion = _currentVersion.Next;
	}
}
