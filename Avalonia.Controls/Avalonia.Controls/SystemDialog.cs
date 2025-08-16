using System;
using System.ComponentModel;
using Avalonia.Controls.Platform;

namespace Avalonia.Controls;

[Obsolete("Use Window.StorageProvider API or TopLevel.StorageProvider API")]
[EditorBrowsable(EditorBrowsableState.Never)]
public abstract class SystemDialog
{
	public string? Title { get; set; }

	static SystemDialog()
	{
		if (AvaloniaLocator.Current.GetService<ISystemDialogImpl>() == null)
		{
			AvaloniaLocator.CurrentMutable.Bind<ISystemDialogImpl>().ToSingleton<SystemDialogImpl>();
		}
	}
}
