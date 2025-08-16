using System;

namespace AvaloniaEdit.Rendering;

public abstract class VisualLineElementGenerator
{
	internal int CachedInterest;

	protected ITextRunConstructionContext CurrentContext { get; private set; }

	public virtual void StartGeneration(ITextRunConstructionContext context)
	{
		CurrentContext = context ?? throw new ArgumentNullException("context");
	}

	public virtual void FinishGeneration()
	{
		CurrentContext = null;
	}

	public abstract int GetFirstInterestedOffset(int startOffset);

	public abstract VisualLineElement ConstructElement(int offset);
}
