using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Avalonia.Metadata;

namespace Avalonia.Controls.Platform;

[Obsolete("Use Window.StorageProvider API or TopLevel.StorageProvider API")]
[EditorBrowsable(EditorBrowsableState.Never)]
[Unstable]
public interface ISystemDialogImpl
{
	Task<string[]?> ShowFileDialogAsync(FileDialog dialog, Window parent);

	Task<string?> ShowFolderDialogAsync(OpenFolderDialog dialog, Window parent);
}
