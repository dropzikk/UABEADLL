using Avalonia.Metadata;

namespace Avalonia.Controls.Platform;

[Unstable]
public interface IMenuInteractionHandler
{
	void Attach(MenuBase menu);

	void Detach(MenuBase menu);
}
