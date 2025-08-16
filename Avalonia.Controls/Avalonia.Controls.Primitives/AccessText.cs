using Avalonia.Automation.Peers;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;
using Avalonia.Reactive;

namespace Avalonia.Controls.Primitives;

public class AccessText : TextBlock
{
	public static readonly AttachedProperty<bool> ShowAccessKeyProperty;

	private IAccessKeyHandler? _accessKeys;

	public char AccessKey { get; private set; }

	public bool ShowAccessKey
	{
		get
		{
			return GetValue(ShowAccessKeyProperty);
		}
		set
		{
			SetValue(ShowAccessKeyProperty, value);
		}
	}

	static AccessText()
	{
		ShowAccessKeyProperty = AvaloniaProperty.RegisterAttached<AccessText, Control, bool>("ShowAccessKey", defaultValue: false, inherits: true);
		Visual.AffectsRender<AccessText>(new AvaloniaProperty[1] { ShowAccessKeyProperty });
	}

	public AccessText()
	{
		this.GetObservable(TextBlock.TextProperty).Subscribe(TextChanged);
	}

	private protected override void RenderCore(DrawingContext context)
	{
		base.RenderCore(context);
		int num = base.Text?.IndexOf('_') ?? (-1);
		if (num != -1 && ShowAccessKey)
		{
			Rect rect = base.TextLayout.HitTestTextPosition(num);
			Vector vector = new Vector(0.0, -1.5);
			context.DrawLine(new Pen(base.Foreground), rect.BottomLeft + vector, rect.BottomRight + vector);
		}
	}

	protected override TextLayout CreateTextLayout(string? text)
	{
		return base.CreateTextLayout(RemoveAccessKeyMarker(text));
	}

	protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
	{
		base.OnAttachedToVisualTree(e);
		_accessKeys = (e.Root as TopLevel)?.AccessKeyHandler;
		if (_accessKeys != null && AccessKey != 0)
		{
			_accessKeys.Register(AccessKey, this);
		}
	}

	protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
	{
		base.OnDetachedFromVisualTree(e);
		if (_accessKeys != null && AccessKey != 0)
		{
			_accessKeys.Unregister(this);
			_accessKeys = null;
		}
	}

	protected override AutomationPeer OnCreateAutomationPeer()
	{
		return new NoneAutomationPeer(this);
	}

	internal static string? RemoveAccessKeyMarker(string? text)
	{
		if (!string.IsNullOrEmpty(text))
		{
			string text2 = "_";
			string oldValue = text2 + text2;
			int num = FindAccessKeyMarker(text);
			if (num >= 0 && num < text.Length - 1)
			{
				text = text.Remove(num, 1);
			}
			text = text.Replace(oldValue, text2);
		}
		return text;
	}

	private static int FindAccessKeyMarker(string text)
	{
		int length = text.Length;
		int num = 0;
		while (num < length)
		{
			int num2 = text.IndexOf('_', num);
			if (num2 == -1)
			{
				return -1;
			}
			if (num2 + 1 < length && text[num2 + 1] != '_')
			{
				return num2;
			}
			num = num2 + 2;
		}
		return -1;
	}

	private void TextChanged(string? text)
	{
		char accessKey = '\0';
		if (text != null)
		{
			int num = text.IndexOf('_');
			if (num != -1 && num < text.Length - 1)
			{
				accessKey = text[num + 1];
			}
		}
		AccessKey = accessKey;
		if (_accessKeys != null && AccessKey != 0)
		{
			_accessKeys.Register(AccessKey, this);
		}
	}
}
