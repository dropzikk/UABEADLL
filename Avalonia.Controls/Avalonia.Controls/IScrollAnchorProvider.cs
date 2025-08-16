namespace Avalonia.Controls;

public interface IScrollAnchorProvider
{
	Control? CurrentAnchor { get; }

	void RegisterAnchorCandidate(Control element);

	void UnregisterAnchorCandidate(Control element);
}
