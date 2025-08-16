using System.Collections.Generic;
using Avalonia.Controls;

namespace Avalonia.Styling.Activators;

internal sealed class StyleClassActivator : StyleActivatorBase, IClassesChangedListener
{
	private readonly IList<string> _match;

	private readonly Classes _classes;

	public StyleClassActivator(Classes classes, IList<string> match)
	{
		_classes = classes;
		_match = match;
	}

	public static bool AreClassesMatching(IReadOnlyList<string> classes, IList<string> toMatch)
	{
		int num = toMatch.Count;
		int count = classes.Count;
		if (count < num)
		{
			return false;
		}
		for (int i = 0; i < count; i++)
		{
			string item = classes[i];
			if (toMatch.Contains(item))
			{
				num--;
				if (num == 0)
				{
					break;
				}
			}
		}
		return num == 0;
	}

	void IClassesChangedListener.Changed()
	{
		ReevaluateIsActive();
	}

	protected override bool EvaluateIsActive()
	{
		return AreClassesMatching(_classes, _match);
	}

	protected override void Initialize()
	{
		_classes.AddListener(this);
	}

	protected override void Deinitialize()
	{
		_classes.RemoveListener(this);
	}
}
