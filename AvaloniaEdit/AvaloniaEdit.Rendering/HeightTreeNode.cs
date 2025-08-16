using System.Collections.Generic;
using AvaloniaEdit.Document;

namespace AvaloniaEdit.Rendering;

internal sealed class HeightTreeNode
{
	internal readonly DocumentLine DocumentLine;

	internal HeightTreeLineNode LineNode;

	internal HeightTreeNode Left;

	internal HeightTreeNode Right;

	internal HeightTreeNode Parent;

	internal bool Color;

	internal int TotalCount;

	internal double TotalHeight;

	internal List<CollapsedLineSection> CollapsedSections;

	internal HeightTreeNode LeftMost
	{
		get
		{
			HeightTreeNode heightTreeNode = this;
			while (heightTreeNode.Left != null)
			{
				heightTreeNode = heightTreeNode.Left;
			}
			return heightTreeNode;
		}
	}

	internal HeightTreeNode RightMost
	{
		get
		{
			HeightTreeNode heightTreeNode = this;
			while (heightTreeNode.Right != null)
			{
				heightTreeNode = heightTreeNode.Right;
			}
			return heightTreeNode;
		}
	}

	internal HeightTreeNode Successor
	{
		get
		{
			if (Right != null)
			{
				return Right.LeftMost;
			}
			HeightTreeNode heightTreeNode = this;
			HeightTreeNode heightTreeNode2;
			do
			{
				heightTreeNode2 = heightTreeNode;
				heightTreeNode = heightTreeNode.Parent;
			}
			while (heightTreeNode != null && heightTreeNode.Right == heightTreeNode2);
			return heightTreeNode;
		}
	}

	internal bool IsDirectlyCollapsed => CollapsedSections != null;

	internal HeightTreeNode()
	{
	}

	internal HeightTreeNode(DocumentLine documentLine, double height)
	{
		DocumentLine = documentLine;
		TotalCount = 1;
		LineNode = new HeightTreeLineNode(height);
		TotalHeight = height;
	}

	internal void AddDirectlyCollapsed(CollapsedLineSection section)
	{
		if (CollapsedSections == null)
		{
			CollapsedSections = new List<CollapsedLineSection>();
			TotalHeight = 0.0;
		}
		CollapsedSections.Add(section);
	}

	internal void RemoveDirectlyCollapsed(CollapsedLineSection section)
	{
		CollapsedSections.Remove(section);
		if (CollapsedSections.Count == 0)
		{
			CollapsedSections = null;
			TotalHeight = LineNode.TotalHeight;
			if (Left != null)
			{
				TotalHeight += Left.TotalHeight;
			}
			if (Right != null)
			{
				TotalHeight += Right.TotalHeight;
			}
		}
	}
}
