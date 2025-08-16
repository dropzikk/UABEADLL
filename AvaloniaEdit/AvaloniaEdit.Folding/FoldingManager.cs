using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Threading;
using AvaloniaEdit.Document;
using AvaloniaEdit.Editing;
using AvaloniaEdit.Rendering;
using AvaloniaEdit.Utils;

namespace AvaloniaEdit.Folding;

public class FoldingManager
{
	private sealed class FoldingManagerInstallation : FoldingManager
	{
		private TextArea _textArea;

		private FoldingMargin _margin;

		private FoldingElementGenerator _generator;

		public FoldingManagerInstallation(TextArea textArea)
			: base(textArea.Document)
		{
			_textArea = textArea;
			_margin = new FoldingMargin
			{
				FoldingManager = this
			};
			_generator = new FoldingElementGenerator
			{
				FoldingManager = this
			};
			textArea.LeftMargins.Add(_margin);
			textArea.TextView.Services.AddService(typeof(FoldingManager), this);
			textArea.TextView.ElementGenerators.Insert(0, _generator);
			textArea.Caret.PositionChanged += TextArea_Caret_PositionChanged;
		}

		public void Uninstall()
		{
			Clear();
			if (_textArea != null)
			{
				_textArea.Caret.PositionChanged -= TextArea_Caret_PositionChanged;
				_textArea.LeftMargins.Remove(_margin);
				_textArea.TextView.ElementGenerators.Remove(_generator);
				_textArea.TextView.Services.RemoveService(typeof(FoldingManager));
				_margin = null;
				_generator = null;
				_textArea = null;
			}
		}

		private void TextArea_Caret_PositionChanged(object sender, EventArgs e)
		{
			int offset = _textArea.Caret.Offset;
			foreach (FoldingSection item in GetFoldingsContaining(offset))
			{
				if (item.IsFolded && item.StartOffset < offset && offset < item.EndOffset)
				{
					item.IsFolded = false;
				}
			}
		}
	}

	private readonly TextSegmentCollection<FoldingSection> _foldings;

	private bool _isFirstUpdate = true;

	internal TextDocument Document { get; }

	internal List<TextView> TextViews { get; } = new List<TextView>();

	public IEnumerable<FoldingSection> AllFoldings => _foldings;

	public FoldingManager(TextDocument document)
	{
		Document = document ?? throw new ArgumentNullException("document");
		_foldings = new TextSegmentCollection<FoldingSection>();
		Dispatcher.UIThread.VerifyAccess();
		WeakEventManagerBase<TextDocumentWeakEventManager.Changed, TextDocument, EventHandler<DocumentChangeEventArgs>, DocumentChangeEventArgs>.AddHandler(document, OnDocumentChanged);
	}

	private void OnDocumentChanged(object sender, DocumentChangeEventArgs e)
	{
		_foldings.UpdateOffsets(e);
		int offset = e.Offset + e.InsertionLength;
		DocumentLine lineByOffset = Document.GetLineByOffset(offset);
		offset = lineByOffset.Offset + lineByOffset.TotalLength;
		foreach (FoldingSection item in _foldings.FindOverlappingSegments(e.Offset, offset - e.Offset))
		{
			if (item.Length == 0)
			{
				RemoveFolding(item);
			}
			else
			{
				item.ValidateCollapsedLineSections();
			}
		}
	}

	internal void AddToTextView(TextView textView)
	{
		if (textView == null || TextViews.Contains(textView))
		{
			throw new ArgumentException();
		}
		TextViews.Add(textView);
		foreach (FoldingSection folding in _foldings)
		{
			if (folding.CollapsedSections != null)
			{
				Array.Resize(ref folding.CollapsedSections, TextViews.Count);
				folding.ValidateCollapsedLineSections();
			}
		}
	}

	internal void RemoveFromTextView(TextView textView)
	{
		int num = TextViews.IndexOf(textView);
		if (num < 0)
		{
			throw new ArgumentException();
		}
		TextViews.RemoveAt(num);
		foreach (FoldingSection folding in _foldings)
		{
			if (folding.CollapsedSections != null)
			{
				CollapsedLineSection[] array = new CollapsedLineSection[TextViews.Count];
				Array.Copy(folding.CollapsedSections, 0, array, 0, num);
				folding.CollapsedSections[num].Uncollapse();
				Array.Copy(folding.CollapsedSections, num + 1, array, num, array.Length - num);
				folding.CollapsedSections = array;
			}
		}
	}

	internal void Redraw()
	{
		foreach (TextView textView in TextViews)
		{
			textView.Redraw();
		}
	}

	internal void Redraw(FoldingSection fs)
	{
		foreach (TextView textView in TextViews)
		{
			textView.Redraw(fs);
		}
	}

	public FoldingSection CreateFolding(int startOffset, int endOffset)
	{
		if (startOffset >= endOffset)
		{
			throw new ArgumentException("startOffset must be less than endOffset");
		}
		if (startOffset < 0 || endOffset > Document.TextLength)
		{
			throw new ArgumentException("Folding must be within document boundary");
		}
		FoldingSection foldingSection = new FoldingSection(this, startOffset, endOffset);
		_foldings.Add(foldingSection);
		Redraw(foldingSection);
		return foldingSection;
	}

	public void RemoveFolding(FoldingSection fs)
	{
		if (fs == null)
		{
			throw new ArgumentNullException("fs");
		}
		fs.IsFolded = false;
		_foldings.Remove(fs);
		Redraw(fs);
	}

	public void Clear()
	{
		Dispatcher.UIThread.VerifyAccess();
		foreach (FoldingSection folding in _foldings)
		{
			folding.IsFolded = false;
		}
		_foldings.Clear();
		Redraw();
	}

	public int GetNextFoldedFoldingStart(int startOffset)
	{
		FoldingSection foldingSection = _foldings.FindFirstSegmentWithStartAfter(startOffset);
		while (foldingSection != null && !foldingSection.IsFolded)
		{
			foldingSection = _foldings.GetNextSegment(foldingSection);
		}
		return foldingSection?.StartOffset ?? (-1);
	}

	public FoldingSection GetNextFolding(int startOffset)
	{
		return _foldings.FindFirstSegmentWithStartAfter(startOffset);
	}

	public ReadOnlyCollection<FoldingSection> GetFoldingsAt(int startOffset)
	{
		List<FoldingSection> list = new List<FoldingSection>();
		FoldingSection foldingSection = _foldings.FindFirstSegmentWithStartAfter(startOffset);
		while (foldingSection != null && foldingSection.StartOffset == startOffset)
		{
			list.Add(foldingSection);
			foldingSection = _foldings.GetNextSegment(foldingSection);
		}
		return new ReadOnlyCollection<FoldingSection>(list);
	}

	public ReadOnlyCollection<FoldingSection> GetFoldingsContaining(int offset)
	{
		return _foldings.FindSegmentsContaining(offset);
	}

	public void UpdateFoldings(IEnumerable<NewFolding> newFoldings, int firstErrorOffset)
	{
		if (newFoldings == null)
		{
			throw new ArgumentNullException("newFoldings");
		}
		if (firstErrorOffset < 0)
		{
			firstErrorOffset = int.MaxValue;
		}
		FoldingSection[] array = AllFoldings.ToArray();
		int num = 0;
		int num2 = 0;
		foreach (NewFolding newFolding in newFoldings)
		{
			if (newFolding.StartOffset < num2)
			{
				throw new ArgumentException("newFoldings must be sorted by start offset");
			}
			num2 = newFolding.StartOffset;
			if (newFolding.StartOffset == newFolding.EndOffset)
			{
				continue;
			}
			while (num < array.Length && newFolding.StartOffset > array[num].StartOffset)
			{
				RemoveFolding(array[num++]);
			}
			FoldingSection foldingSection;
			if (num < array.Length && newFolding.StartOffset == array[num].StartOffset)
			{
				foldingSection = array[num++];
				foldingSection.Length = newFolding.EndOffset - newFolding.StartOffset;
			}
			else
			{
				foldingSection = CreateFolding(newFolding.StartOffset, newFolding.EndOffset);
				if (_isFirstUpdate)
				{
					foldingSection.IsFolded = newFolding.DefaultClosed;
				}
				foldingSection.Tag = newFolding;
			}
			foldingSection.Title = newFolding.Name;
		}
		_isFirstUpdate = false;
		while (num < array.Length)
		{
			FoldingSection foldingSection2 = array[num++];
			if (foldingSection2.StartOffset < firstErrorOffset)
			{
				RemoveFolding(foldingSection2);
				continue;
			}
			break;
		}
	}

	public static FoldingManager Install(TextArea textArea)
	{
		if (textArea == null)
		{
			throw new ArgumentNullException("textArea");
		}
		return new FoldingManagerInstallation(textArea);
	}

	public static void Uninstall(FoldingManager manager)
	{
		if (manager == null)
		{
			throw new ArgumentNullException("manager");
		}
		if (manager is FoldingManagerInstallation foldingManagerInstallation)
		{
			foldingManagerInstallation.Uninstall();
			return;
		}
		throw new ArgumentException("FoldingManager was not created using FoldingManager.Install");
	}
}
