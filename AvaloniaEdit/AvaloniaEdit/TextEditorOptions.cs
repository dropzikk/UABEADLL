using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace AvaloniaEdit;

public class TextEditorOptions : INotifyPropertyChanged
{
	private bool _showSpaces;

	private string _showSpacesGlyph = "·";

	private bool _showTabs;

	private string _showTabsGlyph = "→";

	private bool _showEndOfLine;

	private string _endOfLineCRLFGlyph = "¶";

	private string _endOfLineCRGlyph = "\\r";

	private string _endOfLineLFGlyph = "\\n";

	private bool _showBoxForControlCharacters = true;

	private bool _enableHyperlinks = true;

	private bool _enableEmailHyperlinks = true;

	private bool _requireControlModifierForHyperlinkClick = true;

	private int _indentationSize = 4;

	private bool _convertTabsToSpaces;

	private bool _cutCopyWholeLine = true;

	private bool _allowScrollBelowDocument = true;

	private double _wordWrapIndentation;

	private bool _inheritWordWrapIndentation = true;

	private bool _enableRectangularSelection = true;

	private bool _enableTextDragDrop = true;

	private bool _enableVirtualSpace;

	private bool _enableImeSupport = true;

	private bool _showColumnRulers;

	private IEnumerable<int> _columnRulerPositions = new List<int> { 80 };

	private bool _highlightCurrentLine;

	private bool _hideCursorWhileTyping = true;

	private bool _allowToggleOverstrikeMode;

	private bool _extendSelectionOnMouseUp = true;

	[DefaultValue(false)]
	public virtual bool ShowSpaces
	{
		get
		{
			return _showSpaces;
		}
		set
		{
			if (_showSpaces != value)
			{
				_showSpaces = value;
				OnPropertyChanged("ShowSpaces");
			}
		}
	}

	[DefaultValue("·")]
	public virtual string ShowSpacesGlyph
	{
		get
		{
			return _showSpacesGlyph;
		}
		set
		{
			if (_showSpacesGlyph != value)
			{
				_showSpacesGlyph = value;
				OnPropertyChanged("ShowSpacesGlyph");
			}
		}
	}

	[DefaultValue(false)]
	public virtual bool ShowTabs
	{
		get
		{
			return _showTabs;
		}
		set
		{
			if (_showTabs != value)
			{
				_showTabs = value;
				OnPropertyChanged("ShowTabs");
			}
		}
	}

	[DefaultValue("→")]
	public virtual string ShowTabsGlyph
	{
		get
		{
			return _showTabsGlyph;
		}
		set
		{
			if (_showTabsGlyph != value)
			{
				_showTabsGlyph = value;
				OnPropertyChanged("ShowTabsGlyph");
			}
		}
	}

	[DefaultValue(false)]
	public virtual bool ShowEndOfLine
	{
		get
		{
			return _showEndOfLine;
		}
		set
		{
			if (_showEndOfLine != value)
			{
				_showEndOfLine = value;
				OnPropertyChanged("ShowEndOfLine");
			}
		}
	}

	[DefaultValue("¶")]
	public virtual string EndOfLineCRLFGlyph
	{
		get
		{
			return _endOfLineCRLFGlyph;
		}
		set
		{
			if (_endOfLineCRLFGlyph != value)
			{
				_endOfLineCRLFGlyph = value;
				OnPropertyChanged("EndOfLineCRLFGlyph");
			}
		}
	}

	[DefaultValue("\\r")]
	public virtual string EndOfLineCRGlyph
	{
		get
		{
			return _endOfLineCRGlyph;
		}
		set
		{
			if (_endOfLineCRGlyph != value)
			{
				_endOfLineCRGlyph = value;
				OnPropertyChanged("EndOfLineCRGlyph");
			}
		}
	}

	[DefaultValue("\\n")]
	public virtual string EndOfLineLFGlyph
	{
		get
		{
			return _endOfLineLFGlyph;
		}
		set
		{
			if (_endOfLineLFGlyph != value)
			{
				_endOfLineLFGlyph = value;
				OnPropertyChanged("EndOfLineLFGlyph");
			}
		}
	}

	[DefaultValue(true)]
	public virtual bool ShowBoxForControlCharacters
	{
		get
		{
			return _showBoxForControlCharacters;
		}
		set
		{
			if (_showBoxForControlCharacters != value)
			{
				_showBoxForControlCharacters = value;
				OnPropertyChanged("ShowBoxForControlCharacters");
			}
		}
	}

	[DefaultValue(true)]
	public virtual bool EnableHyperlinks
	{
		get
		{
			return _enableHyperlinks;
		}
		set
		{
			if (_enableHyperlinks != value)
			{
				_enableHyperlinks = value;
				OnPropertyChanged("EnableHyperlinks");
			}
		}
	}

	[DefaultValue(true)]
	public virtual bool EnableEmailHyperlinks
	{
		get
		{
			return _enableEmailHyperlinks;
		}
		set
		{
			if (_enableEmailHyperlinks != value)
			{
				_enableEmailHyperlinks = value;
				OnPropertyChanged("EnableEmailHyperlinks");
			}
		}
	}

	[DefaultValue(true)]
	public virtual bool RequireControlModifierForHyperlinkClick
	{
		get
		{
			return _requireControlModifierForHyperlinkClick;
		}
		set
		{
			if (_requireControlModifierForHyperlinkClick != value)
			{
				_requireControlModifierForHyperlinkClick = value;
				OnPropertyChanged("RequireControlModifierForHyperlinkClick");
			}
		}
	}

	[DefaultValue(4)]
	public virtual int IndentationSize
	{
		get
		{
			return _indentationSize;
		}
		set
		{
			if (value < 1)
			{
				throw new ArgumentOutOfRangeException("value", value, "value must be positive");
			}
			if (value > 1000)
			{
				throw new ArgumentOutOfRangeException("value", value, "indentation size is too large");
			}
			if (_indentationSize != value)
			{
				_indentationSize = value;
				OnPropertyChanged("IndentationSize");
				OnPropertyChanged("IndentationString");
			}
		}
	}

	[DefaultValue(false)]
	public virtual bool ConvertTabsToSpaces
	{
		get
		{
			return _convertTabsToSpaces;
		}
		set
		{
			if (_convertTabsToSpaces != value)
			{
				_convertTabsToSpaces = value;
				OnPropertyChanged("ConvertTabsToSpaces");
				OnPropertyChanged("IndentationString");
			}
		}
	}

	public string IndentationString => GetIndentationString(1);

	[DefaultValue(true)]
	public virtual bool CutCopyWholeLine
	{
		get
		{
			return _cutCopyWholeLine;
		}
		set
		{
			if (_cutCopyWholeLine != value)
			{
				_cutCopyWholeLine = value;
				OnPropertyChanged("CutCopyWholeLine");
			}
		}
	}

	[DefaultValue(true)]
	public virtual bool AllowScrollBelowDocument
	{
		get
		{
			return _allowScrollBelowDocument;
		}
		set
		{
			if (_allowScrollBelowDocument != value)
			{
				_allowScrollBelowDocument = value;
				OnPropertyChanged("AllowScrollBelowDocument");
			}
		}
	}

	[DefaultValue(0.0)]
	public virtual double WordWrapIndentation
	{
		get
		{
			return _wordWrapIndentation;
		}
		set
		{
			if (double.IsNaN(value) || double.IsInfinity(value))
			{
				throw new ArgumentOutOfRangeException("value", value, "value must not be NaN/infinity");
			}
			if (value != _wordWrapIndentation)
			{
				_wordWrapIndentation = value;
				OnPropertyChanged("WordWrapIndentation");
			}
		}
	}

	[DefaultValue(true)]
	public virtual bool InheritWordWrapIndentation
	{
		get
		{
			return _inheritWordWrapIndentation;
		}
		set
		{
			if (value != _inheritWordWrapIndentation)
			{
				_inheritWordWrapIndentation = value;
				OnPropertyChanged("InheritWordWrapIndentation");
			}
		}
	}

	[DefaultValue(true)]
	public bool EnableRectangularSelection
	{
		get
		{
			return _enableRectangularSelection;
		}
		set
		{
			if (_enableRectangularSelection != value)
			{
				_enableRectangularSelection = value;
				OnPropertyChanged("EnableRectangularSelection");
			}
		}
	}

	[DefaultValue(true)]
	public bool EnableTextDragDrop
	{
		get
		{
			return _enableTextDragDrop;
		}
		set
		{
			if (_enableTextDragDrop != value)
			{
				_enableTextDragDrop = value;
				OnPropertyChanged("EnableTextDragDrop");
			}
		}
	}

	[DefaultValue(false)]
	public virtual bool EnableVirtualSpace
	{
		get
		{
			return _enableVirtualSpace;
		}
		set
		{
			if (_enableVirtualSpace != value)
			{
				_enableVirtualSpace = value;
				OnPropertyChanged("EnableVirtualSpace");
			}
		}
	}

	[DefaultValue(true)]
	public virtual bool EnableImeSupport
	{
		get
		{
			return _enableImeSupport;
		}
		set
		{
			if (_enableImeSupport != value)
			{
				_enableImeSupport = value;
				OnPropertyChanged("EnableImeSupport");
			}
		}
	}

	[DefaultValue(false)]
	public virtual bool ShowColumnRulers
	{
		get
		{
			return _showColumnRulers;
		}
		set
		{
			if (_showColumnRulers != value)
			{
				_showColumnRulers = value;
				OnPropertyChanged("ShowColumnRulers");
			}
		}
	}

	public virtual IEnumerable<int> ColumnRulerPositions
	{
		get
		{
			return _columnRulerPositions;
		}
		set
		{
			if (_columnRulerPositions != value)
			{
				_columnRulerPositions = value;
				OnPropertyChanged("ColumnRulerPositions");
			}
		}
	}

	[DefaultValue(false)]
	public virtual bool HighlightCurrentLine
	{
		get
		{
			return _highlightCurrentLine;
		}
		set
		{
			if (_highlightCurrentLine != value)
			{
				_highlightCurrentLine = value;
				OnPropertyChanged("HighlightCurrentLine");
			}
		}
	}

	[DefaultValue(true)]
	public bool HideCursorWhileTyping
	{
		get
		{
			return _hideCursorWhileTyping;
		}
		set
		{
			if (_hideCursorWhileTyping != value)
			{
				_hideCursorWhileTyping = value;
				OnPropertyChanged("HideCursorWhileTyping");
			}
		}
	}

	[DefaultValue(false)]
	public bool AllowToggleOverstrikeMode
	{
		get
		{
			return _allowToggleOverstrikeMode;
		}
		set
		{
			if (_allowToggleOverstrikeMode != value)
			{
				_allowToggleOverstrikeMode = value;
				OnPropertyChanged("AllowToggleOverstrikeMode");
			}
		}
	}

	[DefaultValue(true)]
	public bool ExtendSelectionOnMouseUp
	{
		get
		{
			return _extendSelectionOnMouseUp;
		}
		set
		{
			if (_extendSelectionOnMouseUp != value)
			{
				_extendSelectionOnMouseUp = value;
				OnPropertyChanged("ExtendSelectionOnMouseUp");
			}
		}
	}

	public event PropertyChangedEventHandler PropertyChanged;

	public TextEditorOptions()
	{
	}

	public TextEditorOptions(TextEditorOptions options)
	{
		foreach (FieldInfo runtimeField in typeof(TextEditorOptions).GetRuntimeFields())
		{
			if (!runtimeField.IsStatic)
			{
				runtimeField.SetValue(this, runtimeField.GetValue(options));
			}
		}
	}

	protected void OnPropertyChanged(string propertyName)
	{
		PropertyChangedEventArgs e = new PropertyChangedEventArgs(propertyName);
		OnPropertyChanged(e);
	}

	protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
	{
		this.PropertyChanged?.Invoke(this, e);
	}

	public virtual string GetIndentationString(int column)
	{
		if (column < 1)
		{
			throw new ArgumentOutOfRangeException("column", column, "Value must be at least 1.");
		}
		int indentationSize = IndentationSize;
		if (ConvertTabsToSpaces)
		{
			return new string(' ', indentationSize - (column - 1) % indentationSize);
		}
		return "\t";
	}
}
