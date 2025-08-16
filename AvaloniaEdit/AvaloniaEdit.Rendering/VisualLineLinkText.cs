using System;
using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;

namespace AvaloniaEdit.Rendering;

public class VisualLineLinkText : VisualLineText
{
	public static RoutedEvent<OpenUriRoutedEventArgs> OpenUriEvent { get; }

	public Uri NavigateUri { get; set; }

	public string TargetName { get; set; }

	public bool RequireControlModifierForClick { get; set; }

	static VisualLineLinkText()
	{
		OpenUriEvent = RoutedEvent.Register<VisualLineText, OpenUriRoutedEventArgs>("OpenUriEvent", RoutingStrategies.Bubble);
		OpenUriEvent.AddClassHandler<Window>(ExecuteOpenUriEventHandler);
	}

	public VisualLineLinkText(VisualLine parentVisualLine, int length)
		: base(parentVisualLine, length)
	{
		RequireControlModifierForClick = true;
	}

	public override TextRun CreateTextRun(int startVisualColumn, ITextRunConstructionContext context)
	{
		base.TextRunProperties.SetForegroundBrush(context.TextView.LinkTextForegroundBrush);
		base.TextRunProperties.SetBackgroundBrush(context.TextView.LinkTextBackgroundBrush);
		if (context.TextView.LinkTextUnderline)
		{
			base.TextRunProperties.SetTextDecorations(TextDecorations.Underline);
		}
		return base.CreateTextRun(startVisualColumn, context);
	}

	protected virtual bool LinkIsClickable(KeyModifiers modifiers)
	{
		if (NavigateUri == null)
		{
			return false;
		}
		if (RequireControlModifierForClick)
		{
			return modifiers.HasFlag(KeyModifiers.Control);
		}
		return true;
	}

	protected internal override void OnQueryCursor(PointerEventArgs e)
	{
		if (LinkIsClickable(e.KeyModifiers))
		{
			if (e.Source is InputElement inputElement)
			{
				inputElement.Cursor = new Cursor(StandardCursorType.Hand);
			}
			e.Handled = true;
		}
	}

	protected internal override void OnPointerPressed(PointerPressedEventArgs e)
	{
		if (!e.Handled && LinkIsClickable(e.KeyModifiers))
		{
			OpenUriRoutedEventArgs e2 = new OpenUriRoutedEventArgs(NavigateUri)
			{
				RoutedEvent = OpenUriEvent
			};
			if (e.Source is Interactive interactive)
			{
				interactive.RaiseEvent(e2);
			}
			e.Handled = true;
		}
	}

	protected override VisualLineText CreateInstance(int length)
	{
		return new VisualLineLinkText(base.ParentVisualLine, length)
		{
			NavigateUri = NavigateUri,
			TargetName = TargetName,
			RequireControlModifierForClick = RequireControlModifierForClick
		};
	}

	private static void ExecuteOpenUriEventHandler(Window window, OpenUriRoutedEventArgs arg)
	{
		string fileName = arg.Uri.ToString();
		try
		{
			Process.Start(new ProcessStartInfo
			{
				FileName = fileName,
				UseShellExecute = true
			});
		}
		catch (Exception)
		{
		}
	}
}
