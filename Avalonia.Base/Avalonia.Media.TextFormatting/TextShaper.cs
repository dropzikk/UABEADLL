using System;
using Avalonia.Platform;

namespace Avalonia.Media.TextFormatting;

public class TextShaper
{
	private readonly ITextShaperImpl _platformImpl;

	public static TextShaper Current
	{
		get
		{
			TextShaper service = AvaloniaLocator.Current.GetService<TextShaper>();
			if (service != null)
			{
				return service;
			}
			service = new TextShaper(AvaloniaLocator.Current.GetRequiredService<ITextShaperImpl>());
			AvaloniaLocator.CurrentMutable.Bind<TextShaper>().ToConstant(service);
			return service;
		}
	}

	public TextShaper(ITextShaperImpl platformImpl)
	{
		_platformImpl = platformImpl;
	}

	public ShapedBuffer ShapeText(ReadOnlyMemory<char> text, TextShaperOptions options = default(TextShaperOptions))
	{
		return _platformImpl.ShapeText(text, options);
	}

	public ShapedBuffer ShapeText(string text, TextShaperOptions options = default(TextShaperOptions))
	{
		return ShapeText(text.AsMemory(), options);
	}
}
