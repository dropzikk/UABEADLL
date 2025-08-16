namespace AvaloniaEdit.Utils;

internal abstract class AbstractFreezable : IFreezable
{
	public bool IsFrozen { get; private set; }

	public void Freeze()
	{
		if (!IsFrozen)
		{
			FreezeInternal();
			IsFrozen = true;
		}
	}

	protected virtual void FreezeInternal()
	{
	}
}
