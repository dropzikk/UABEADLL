using System;
using System.Collections.Generic;
using Avalonia.Controls;

namespace Avalonia.Diagnostics;

internal class SingleViewTopLevelGroup : IDevToolsTopLevelGroup
{
	private readonly TopLevel _topLevel;

	public IReadOnlyList<TopLevel> Items { get; }

	public SingleViewTopLevelGroup(TopLevel topLevel)
	{
		_topLevel = topLevel;
		Items = new TopLevel[1] { topLevel ?? throw new ArgumentNullException("topLevel") };
	}

	public override int GetHashCode()
	{
		return _topLevel.GetHashCode();
	}

	public override bool Equals(object? obj)
	{
		if (obj is SingleViewTopLevelGroup singleViewTopLevelGroup)
		{
			return singleViewTopLevelGroup._topLevel == _topLevel;
		}
		return false;
	}
}
