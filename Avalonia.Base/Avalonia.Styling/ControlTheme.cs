using System;
using Avalonia.PropertyStore;

namespace Avalonia.Styling;

public class ControlTheme : StyleBase
{
	public Type? TargetType { get; set; }

	public ControlTheme? BasedOn { get; set; }

	public ControlTheme()
	{
	}

	public ControlTheme(Type targetType)
	{
		TargetType = targetType;
	}

	public override string ToString()
	{
		return TargetType?.Name ?? "ControlTheme";
	}

	internal override void SetParent(StyleBase? parent)
	{
		throw new InvalidOperationException("ControlThemes cannot be added as a nested style.");
	}

	internal SelectorMatchResult TryAttach(StyledElement target, FrameType type)
	{
		if (target == null)
		{
			throw new ArgumentNullException("target");
		}
		if ((object)TargetType == null)
		{
			throw new InvalidOperationException("ControlTheme has no TargetType.");
		}
		if (base.HasSettersOrAnimations && TargetType.IsAssignableFrom(StyledElement.GetStyleKey(target)))
		{
			Attach(target, null, type);
			return SelectorMatchResult.AlwaysThisType;
		}
		return SelectorMatchResult.NeverThisType;
	}
}
