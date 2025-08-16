using Avalonia.Controls.Platform;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Rendering;

namespace Avalonia.Controls;

internal interface IMenu : IMenuElement, IInputElement, ILogical
{
	IMenuInteractionHandler InteractionHandler { get; }

	bool IsOpen { get; }

	IRenderRoot? VisualRoot { get; }
}
