using System;

namespace Avalonia.Data;

public class RelativeSource
{
	private int _ancestorLevel = 1;

	public int AncestorLevel
	{
		get
		{
			return _ancestorLevel;
		}
		set
		{
			if (value <= 0)
			{
				throw new ArgumentOutOfRangeException("value", "AncestorLevel may not be set to less than 1.");
			}
			_ancestorLevel = value;
		}
	}

	public Type? AncestorType { get; set; }

	public RelativeSourceMode Mode { get; set; }

	public TreeType Tree { get; set; }

	public RelativeSource()
	{
		Mode = RelativeSourceMode.FindAncestor;
	}

	public RelativeSource(RelativeSourceMode mode)
	{
		Mode = mode;
	}
}
