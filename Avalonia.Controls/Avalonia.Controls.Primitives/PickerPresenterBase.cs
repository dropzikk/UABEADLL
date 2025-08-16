using System;

namespace Avalonia.Controls.Primitives;

public abstract class PickerPresenterBase : TemplatedControl
{
	public event EventHandler? Confirmed;

	public event EventHandler? Dismissed;

	protected virtual void OnConfirmed()
	{
		this.Confirmed?.Invoke(this, EventArgs.Empty);
	}

	protected virtual void OnDismiss()
	{
		this.Dismissed?.Invoke(this, EventArgs.Empty);
	}
}
