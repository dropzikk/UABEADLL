namespace AvaloniaEdit.Utils;

internal interface IFreezable
{
	bool IsFrozen { get; }

	void Freeze();
}
