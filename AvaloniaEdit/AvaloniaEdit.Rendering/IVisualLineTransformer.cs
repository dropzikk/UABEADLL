using System.Collections.Generic;

namespace AvaloniaEdit.Rendering;

public interface IVisualLineTransformer
{
	void Transform(ITextRunConstructionContext context, IList<VisualLineElement> elements);
}
