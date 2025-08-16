using Avalonia.Metadata;
using Avalonia.Platform;

namespace Avalonia.Markup.Xaml.MarkupExtensions;

public abstract class OnFormFactorExtensionBase<TReturn, TOn> : IAddChild<TOn> where TOn : On<TReturn>
{
	[MarkupExtensionDefaultOption]
	public TReturn? Default { get; set; }

	[MarkupExtensionOption(FormFactorType.Desktop)]
	public TReturn? Desktop { get; set; }

	[MarkupExtensionOption(FormFactorType.Mobile)]
	public TReturn? Mobile { get; set; }

	public object ProvideValue()
	{
		return this;
	}

	void IAddChild<TOn>.AddChild(TOn child)
	{
	}
}
