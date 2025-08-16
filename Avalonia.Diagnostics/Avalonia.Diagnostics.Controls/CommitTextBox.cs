using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Diagnostics.Controls;

internal sealed class CommitTextBox : TextBox
{
	public static readonly DirectProperty<CommitTextBox, string?> CommittedTextProperty = AvaloniaProperty.RegisterDirect("CommittedText", (CommitTextBox o) => o.CommittedText, delegate(CommitTextBox o, string? v)
	{
		o.CommittedText = v;
	});

	private string? _committedText;

	protected override Type StyleKeyOverride => typeof(TextBox);

	public string? CommittedText
	{
		get
		{
			return _committedText;
		}
		set
		{
			SetAndRaise(CommittedTextProperty, ref _committedText, value);
		}
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (change.Property == CommittedTextProperty)
		{
			base.Text = CommittedText;
		}
	}

	protected override void OnKeyUp(KeyEventArgs e)
	{
		base.OnKeyUp(e);
		switch (e.Key)
		{
		case Key.Return:
			TryCommit();
			e.Handled = true;
			break;
		case Key.Escape:
			Cancel();
			e.Handled = true;
			break;
		}
	}

	protected override void OnLostFocus(RoutedEventArgs e)
	{
		base.OnLostFocus(e);
		TryCommit();
	}

	private void Cancel()
	{
		base.Text = CommittedText;
		DataValidationErrors.ClearErrors(this);
	}

	private void TryCommit()
	{
		if (!DataValidationErrors.GetHasErrors(this))
		{
			CommittedText = base.Text;
			return;
		}
		base.Text = CommittedText;
		DataValidationErrors.ClearErrors(this);
	}
}
