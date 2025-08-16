using System.Collections.Generic;
using Avalonia.Automation.Peers;

namespace Avalonia.Automation.Provider;

public interface ISelectionProvider
{
	bool CanSelectMultiple { get; }

	bool IsSelectionRequired { get; }

	IReadOnlyList<AutomationPeer> GetSelection();
}
