using Avalonia.Styling;

namespace Avalonia.Controls;

public interface ITemplate<TControl> : ITemplate where TControl : Control?
{
	new TControl Build();
}
