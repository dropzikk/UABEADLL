using System;
using System.Collections.Generic;
using System.Text;
using Avalonia.Styling.Activators;
using Avalonia.Utilities;

namespace Avalonia.Styling;

internal sealed class TypeNameAndClassSelector : Selector
{
	private readonly Selector? _previous;

	private List<string>? _classes;

	private Type? _targetType;

	private string? _selectorString;

	internal override bool InTemplate => _previous?.InTemplate ?? false;

	public string? Name { get; set; }

	internal override Type? TargetType
	{
		get
		{
			Type? targetType = _targetType;
			if ((object)targetType == null)
			{
				Selector? previous = _previous;
				if (previous == null)
				{
					return null;
				}
				targetType = previous.TargetType;
			}
			return targetType;
		}
	}

	internal override bool IsCombinator => false;

	public bool IsConcreteType { get; private set; }

	public IList<string> Classes => _classes ?? (_classes = new List<string>());

	public static TypeNameAndClassSelector OfType(Selector? previous, Type targetType)
	{
		return new TypeNameAndClassSelector(previous)
		{
			_targetType = targetType,
			IsConcreteType = true
		};
	}

	public static TypeNameAndClassSelector Is(Selector? previous, Type targetType)
	{
		return new TypeNameAndClassSelector(previous)
		{
			_targetType = targetType,
			IsConcreteType = false
		};
	}

	public static TypeNameAndClassSelector ForName(Selector? previous, string name)
	{
		return new TypeNameAndClassSelector(previous)
		{
			Name = name
		};
	}

	public static TypeNameAndClassSelector ForClass(Selector? previous, string className)
	{
		return new TypeNameAndClassSelector(previous)
		{
			Classes = { className }
		};
	}

	private TypeNameAndClassSelector(Selector? previous)
	{
		_previous = previous;
	}

	public override string ToString(Style? owner)
	{
		return _selectorString ?? (_selectorString = BuildSelectorString(owner));
	}

	private protected override SelectorMatch Evaluate(StyledElement control, IStyle? parent, bool subscribe)
	{
		if (TargetType != null)
		{
			Type type = StyledElement.GetStyleKey(control) ?? control.GetType();
			if (IsConcreteType)
			{
				if (type != TargetType)
				{
					return SelectorMatch.NeverThisType;
				}
			}
			else if (!TargetType.IsAssignableFrom(type))
			{
				return SelectorMatch.NeverThisType;
			}
		}
		if (Name != null && control.Name != Name)
		{
			return SelectorMatch.NeverThisInstance;
		}
		List<string> classes = _classes;
		if (classes != null && classes.Count > 0)
		{
			if (subscribe)
			{
				return new SelectorMatch(new StyleClassActivator(control.Classes, _classes));
			}
			if (!StyleClassActivator.AreClassesMatching(control.Classes, _classes))
			{
				return SelectorMatch.NeverThisInstance;
			}
		}
		if (Name != null)
		{
			return SelectorMatch.AlwaysThisInstance;
		}
		return SelectorMatch.AlwaysThisType;
	}

	private protected override Selector? MovePrevious()
	{
		return _previous;
	}

	private protected override Selector? MovePreviousOrParent()
	{
		return _previous;
	}

	private string BuildSelectorString(Style? owner)
	{
		StringBuilder stringBuilder = StringBuilderCache.Acquire();
		if (_previous != null)
		{
			stringBuilder.Append(_previous.ToString(owner));
		}
		if (TargetType != null)
		{
			if (IsConcreteType)
			{
				stringBuilder.Append(TargetType.Name);
			}
			else
			{
				stringBuilder.Append(":is(");
				stringBuilder.Append(TargetType.Name);
				stringBuilder.Append(")");
			}
		}
		if (Name != null)
		{
			stringBuilder.Append('#');
			stringBuilder.Append(Name);
		}
		List<string> classes = _classes;
		if (classes != null && classes.Count > 0)
		{
			foreach (string @class in _classes)
			{
				if (!@class.StartsWith(":"))
				{
					stringBuilder.Append('.');
				}
				stringBuilder.Append(@class);
			}
		}
		return StringBuilderCache.GetStringAndRelease(stringBuilder);
	}
}
