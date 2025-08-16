using System.Collections.Generic;

namespace AvaloniaEdit.Rendering;

internal struct HeightTreeLineNode
{
	internal double Height;

	internal List<CollapsedLineSection> CollapsedSections;

	internal bool IsDirectlyCollapsed => CollapsedSections != null;

	internal double TotalHeight
	{
		get
		{
			if (!IsDirectlyCollapsed)
			{
				return Height;
			}
			return 0.0;
		}
	}

	internal HeightTreeLineNode(double height)
	{
		CollapsedSections = null;
		Height = height;
	}

	internal void AddDirectlyCollapsed(CollapsedLineSection section)
	{
		if (CollapsedSections == null)
		{
			CollapsedSections = new List<CollapsedLineSection>();
		}
		CollapsedSections.Add(section);
	}

	internal void RemoveDirectlyCollapsed(CollapsedLineSection section)
	{
		CollapsedSections.Remove(section);
		if (CollapsedSections.Count == 0)
		{
			CollapsedSections = null;
		}
	}
}
