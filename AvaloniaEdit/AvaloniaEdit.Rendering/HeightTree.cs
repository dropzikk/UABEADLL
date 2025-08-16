using System;
using System.Collections.Generic;
using System.Linq;
using AvaloniaEdit.Document;
using AvaloniaEdit.Utils;

namespace AvaloniaEdit.Rendering;

internal sealed class HeightTree : ILineTracker, IDisposable
{
	private enum UpdateAfterChildrenChangeRecursionMode
	{
		None,
		IfRequired,
		WholeBranch
	}

	private readonly TextDocument _document;

	private HeightTreeNode _root;

	private WeakLineTracker _weakLineTracker;

	private double _defaultLineHeight;

	private bool _inRemoval;

	private List<HeightTreeNode> _nodesToCheckForMerging;

	private const bool Red = true;

	private const bool Black = false;

	public bool IsDisposed => _root == null;

	public double DefaultLineHeight
	{
		get
		{
			return _defaultLineHeight;
		}
		set
		{
			double defaultLineHeight = _defaultLineHeight;
			if (defaultLineHeight == value)
			{
				return;
			}
			_defaultLineHeight = value;
			foreach (HeightTreeNode allNode in AllNodes)
			{
				if (allNode.LineNode.Height == defaultLineHeight)
				{
					allNode.LineNode.Height = value;
					UpdateAugmentedData(allNode, UpdateAfterChildrenChangeRecursionMode.IfRequired);
				}
			}
		}
	}

	public int LineCount => _root.TotalCount;

	public double TotalHeight => _root.TotalHeight;

	private IEnumerable<HeightTreeNode> AllNodes
	{
		get
		{
			if (_root != null)
			{
				for (HeightTreeNode node = _root.LeftMost; node != null; node = node.Successor)
				{
					yield return node;
				}
			}
		}
	}

	public HeightTree(TextDocument document, double defaultLineHeight)
	{
		_document = document;
		_weakLineTracker = WeakLineTracker.Register(document, this);
		DefaultLineHeight = defaultLineHeight;
		RebuildDocument();
	}

	public void Dispose()
	{
		if (_weakLineTracker != null)
		{
			_weakLineTracker.Deregister();
		}
		_root = null;
		_weakLineTracker = null;
	}

	private HeightTreeNode GetNode(DocumentLine ls)
	{
		return GetNodeByIndex(ls.LineNumber - 1);
	}

	void ILineTracker.ChangeComplete(DocumentChangeEventArgs e)
	{
	}

	void ILineTracker.SetLineLength(DocumentLine ls, int newTotalLength)
	{
	}

	public void RebuildDocument()
	{
		foreach (CollapsedLineSection allCollapsedSection in GetAllCollapsedSections())
		{
			allCollapsedSection.Start = null;
			allCollapsedSection.End = null;
		}
		HeightTreeNode[] array = new HeightTreeNode[_document.LineCount];
		int num = 0;
		foreach (DocumentLine line in _document.Lines)
		{
			array[num++] = new HeightTreeNode(line, _defaultLineHeight);
		}
		int treeHeight = DocumentLineTree.GetTreeHeight(array.Length);
		_root = BuildTree(array, 0, array.Length, treeHeight);
		_root.Color = false;
	}

	private HeightTreeNode BuildTree(HeightTreeNode[] nodes, int start, int end, int subtreeHeight)
	{
		if (start == end)
		{
			return null;
		}
		int num = (start + end) / 2;
		HeightTreeNode heightTreeNode = nodes[num];
		heightTreeNode.Left = BuildTree(nodes, start, num, subtreeHeight - 1);
		heightTreeNode.Right = BuildTree(nodes, num + 1, end, subtreeHeight - 1);
		if (heightTreeNode.Left != null)
		{
			heightTreeNode.Left.Parent = heightTreeNode;
		}
		if (heightTreeNode.Right != null)
		{
			heightTreeNode.Right.Parent = heightTreeNode;
		}
		if (subtreeHeight == 1)
		{
			heightTreeNode.Color = true;
		}
		UpdateAugmentedData(heightTreeNode, UpdateAfterChildrenChangeRecursionMode.None);
		return heightTreeNode;
	}

	void ILineTracker.BeforeRemoveLine(DocumentLine line)
	{
		HeightTreeNode node = GetNode(line);
		if (node.LineNode.CollapsedSections != null)
		{
			CollapsedLineSection[] array = node.LineNode.CollapsedSections.ToArray();
			foreach (CollapsedLineSection collapsedLineSection in array)
			{
				if (collapsedLineSection.Start == line && collapsedLineSection.End == line)
				{
					collapsedLineSection.Start = null;
					collapsedLineSection.End = null;
				}
				else if (collapsedLineSection.Start == line)
				{
					Uncollapse(collapsedLineSection);
					collapsedLineSection.Start = line.NextLine;
					AddCollapsedSection(collapsedLineSection, collapsedLineSection.End.LineNumber - collapsedLineSection.Start.LineNumber + 1);
				}
				else if (collapsedLineSection.End == line)
				{
					Uncollapse(collapsedLineSection);
					collapsedLineSection.End = line.PreviousLine;
					AddCollapsedSection(collapsedLineSection, collapsedLineSection.End.LineNumber - collapsedLineSection.Start.LineNumber + 1);
				}
			}
		}
		BeginRemoval();
		RemoveNode(node);
		node.LineNode.CollapsedSections = null;
		EndRemoval();
	}

	void ILineTracker.LineInserted(DocumentLine insertionPos, DocumentLine newLine)
	{
		InsertAfter(GetNode(insertionPos), newLine);
	}

	private HeightTreeNode InsertAfter(HeightTreeNode node, DocumentLine newLine)
	{
		HeightTreeNode heightTreeNode = new HeightTreeNode(newLine, _defaultLineHeight);
		if (node.Right == null)
		{
			if (node.LineNode.CollapsedSections != null)
			{
				foreach (CollapsedLineSection collapsedSection in node.LineNode.CollapsedSections)
				{
					if (collapsedSection.End != node.DocumentLine)
					{
						heightTreeNode.AddDirectlyCollapsed(collapsedSection);
					}
				}
			}
			InsertAsRight(node, heightTreeNode);
		}
		else
		{
			node = node.Right.LeftMost;
			if (node.LineNode.CollapsedSections != null)
			{
				foreach (CollapsedLineSection collapsedSection2 in node.LineNode.CollapsedSections)
				{
					if (collapsedSection2.Start != node.DocumentLine)
					{
						heightTreeNode.AddDirectlyCollapsed(collapsedSection2);
					}
				}
			}
			InsertAsLeft(node, heightTreeNode);
		}
		return heightTreeNode;
	}

	private static void UpdateAfterChildrenChange(HeightTreeNode node)
	{
		UpdateAugmentedData(node, UpdateAfterChildrenChangeRecursionMode.IfRequired);
	}

	private static void UpdateAugmentedData(HeightTreeNode node, UpdateAfterChildrenChangeRecursionMode mode)
	{
		int num = 1;
		double num2 = node.LineNode.TotalHeight;
		if (node.Left != null)
		{
			num += node.Left.TotalCount;
			num2 += node.Left.TotalHeight;
		}
		if (node.Right != null)
		{
			num += node.Right.TotalCount;
			num2 += node.Right.TotalHeight;
		}
		if (node.IsDirectlyCollapsed)
		{
			num2 = 0.0;
		}
		if (num != node.TotalCount || !num2.IsClose(node.TotalHeight) || mode == UpdateAfterChildrenChangeRecursionMode.WholeBranch)
		{
			node.TotalCount = num;
			node.TotalHeight = num2;
			if (node.Parent != null && mode != 0)
			{
				UpdateAugmentedData(node.Parent, mode);
			}
		}
	}

	private void UpdateAfterRotateLeft(HeightTreeNode node)
	{
		List<CollapsedLineSection> collapsedSections = node.Parent.CollapsedSections;
		List<CollapsedLineSection> collapsedSections2 = node.CollapsedSections;
		node.Parent.CollapsedSections = collapsedSections2;
		node.CollapsedSections = null;
		if (collapsedSections != null)
		{
			foreach (CollapsedLineSection item in collapsedSections)
			{
				if (node.Parent.Right != null)
				{
					node.Parent.Right.AddDirectlyCollapsed(item);
				}
				node.Parent.LineNode.AddDirectlyCollapsed(item);
				if (node.Right != null)
				{
					node.Right.AddDirectlyCollapsed(item);
				}
			}
		}
		MergeCollapsedSectionsIfPossible(node);
		UpdateAfterChildrenChange(node);
	}

	private void UpdateAfterRotateRight(HeightTreeNode node)
	{
		List<CollapsedLineSection> collapsedSections = node.Parent.CollapsedSections;
		List<CollapsedLineSection> collapsedSections2 = node.CollapsedSections;
		node.Parent.CollapsedSections = collapsedSections2;
		node.CollapsedSections = null;
		if (collapsedSections != null)
		{
			foreach (CollapsedLineSection item in collapsedSections)
			{
				if (node.Parent.Left != null)
				{
					node.Parent.Left.AddDirectlyCollapsed(item);
				}
				node.Parent.LineNode.AddDirectlyCollapsed(item);
				if (node.Left != null)
				{
					node.Left.AddDirectlyCollapsed(item);
				}
			}
		}
		MergeCollapsedSectionsIfPossible(node);
		UpdateAfterChildrenChange(node);
	}

	private void BeforeNodeRemove(HeightTreeNode removedNode)
	{
		List<CollapsedLineSection> collapsedSections = removedNode.CollapsedSections;
		if (collapsedSections != null)
		{
			HeightTreeNode heightTreeNode = removedNode.Left ?? removedNode.Right;
			if (heightTreeNode != null)
			{
				foreach (CollapsedLineSection item in collapsedSections)
				{
					heightTreeNode.AddDirectlyCollapsed(item);
				}
			}
		}
		if (removedNode.Parent != null)
		{
			MergeCollapsedSectionsIfPossible(removedNode.Parent);
		}
	}

	private void BeforeNodeReplace(HeightTreeNode removedNode, HeightTreeNode newNode, HeightTreeNode newNodeOldParent)
	{
		while (newNodeOldParent != removedNode)
		{
			if (newNodeOldParent.CollapsedSections != null)
			{
				foreach (CollapsedLineSection collapsedSection in newNodeOldParent.CollapsedSections)
				{
					newNode.LineNode.AddDirectlyCollapsed(collapsedSection);
				}
			}
			newNodeOldParent = newNodeOldParent.Parent;
		}
		if (newNode.CollapsedSections != null)
		{
			foreach (CollapsedLineSection collapsedSection2 in newNode.CollapsedSections)
			{
				newNode.LineNode.AddDirectlyCollapsed(collapsedSection2);
			}
		}
		newNode.CollapsedSections = removedNode.CollapsedSections;
		MergeCollapsedSectionsIfPossible(newNode);
	}

	private void BeginRemoval()
	{
		if (_nodesToCheckForMerging == null)
		{
			_nodesToCheckForMerging = new List<HeightTreeNode>();
		}
		_inRemoval = true;
	}

	private void EndRemoval()
	{
		_inRemoval = false;
		foreach (HeightTreeNode item in _nodesToCheckForMerging)
		{
			MergeCollapsedSectionsIfPossible(item);
		}
		_nodesToCheckForMerging.Clear();
	}

	private void MergeCollapsedSectionsIfPossible(HeightTreeNode node)
	{
		if (_inRemoval)
		{
			_nodesToCheckForMerging.Add(node);
			return;
		}
		bool flag = false;
		List<CollapsedLineSection> collapsedSections = node.LineNode.CollapsedSections;
		if (collapsedSections != null)
		{
			for (int num = collapsedSections.Count - 1; num >= 0; num--)
			{
				CollapsedLineSection collapsedLineSection = collapsedSections[num];
				if (collapsedLineSection.Start != node.DocumentLine && collapsedLineSection.End != node.DocumentLine && (node.Left == null || (node.Left.CollapsedSections != null && node.Left.CollapsedSections.Contains(collapsedLineSection))) && (node.Right == null || (node.Right.CollapsedSections != null && node.Right.CollapsedSections.Contains(collapsedLineSection))))
				{
					if (node.Left != null)
					{
						node.Left.RemoveDirectlyCollapsed(collapsedLineSection);
					}
					if (node.Right != null)
					{
						node.Right.RemoveDirectlyCollapsed(collapsedLineSection);
					}
					collapsedSections.RemoveAt(num);
					node.AddDirectlyCollapsed(collapsedLineSection);
					flag = true;
				}
			}
			if (collapsedSections.Count == 0)
			{
				node.LineNode.CollapsedSections = null;
			}
		}
		if (flag && node.Parent != null)
		{
			MergeCollapsedSectionsIfPossible(node.Parent);
		}
	}

	private HeightTreeNode GetNodeByIndex(int index)
	{
		HeightTreeNode heightTreeNode = _root;
		while (true)
		{
			if (heightTreeNode.Left != null && index < heightTreeNode.Left.TotalCount)
			{
				heightTreeNode = heightTreeNode.Left;
				continue;
			}
			if (heightTreeNode.Left != null)
			{
				index -= heightTreeNode.Left.TotalCount;
			}
			if (index == 0)
			{
				break;
			}
			index--;
			heightTreeNode = heightTreeNode.Right;
		}
		return heightTreeNode;
	}

	private HeightTreeNode GetNodeByVisualPosition(double position)
	{
		HeightTreeNode heightTreeNode = _root;
		while (true)
		{
			double num = position;
			if (heightTreeNode.Left != null)
			{
				num -= heightTreeNode.Left.TotalHeight;
				if (num < 0.0)
				{
					heightTreeNode = heightTreeNode.Left;
					continue;
				}
			}
			double num2 = num - heightTreeNode.LineNode.TotalHeight;
			if (num2 < 0.0)
			{
				return heightTreeNode;
			}
			if (heightTreeNode.Right == null || heightTreeNode.Right.TotalHeight == 0.0)
			{
				if (heightTreeNode.LineNode.TotalHeight > 0.0 || heightTreeNode.Left == null)
				{
					break;
				}
				heightTreeNode = heightTreeNode.Left;
			}
			else
			{
				position = num2;
				heightTreeNode = heightTreeNode.Right;
			}
		}
		return heightTreeNode;
	}

	private static double GetVisualPositionFromNode(HeightTreeNode node)
	{
		double num = ((node.Left != null) ? node.Left.TotalHeight : 0.0);
		while (node.Parent != null)
		{
			if (node.IsDirectlyCollapsed)
			{
				num = 0.0;
			}
			if (node == node.Parent.Right)
			{
				if (node.Parent.Left != null)
				{
					num += node.Parent.Left.TotalHeight;
				}
				num += node.Parent.LineNode.TotalHeight;
			}
			node = node.Parent;
		}
		return num;
	}

	public DocumentLine GetLineByNumber(int number)
	{
		return GetNodeByIndex(number - 1).DocumentLine;
	}

	public DocumentLine GetLineByVisualPosition(double position)
	{
		return GetNodeByVisualPosition(position).DocumentLine;
	}

	public double GetVisualPosition(DocumentLine line)
	{
		return GetVisualPositionFromNode(GetNode(line));
	}

	public double GetHeight(DocumentLine line)
	{
		return GetNode(line).LineNode.Height;
	}

	public void SetHeight(DocumentLine line, double val)
	{
		HeightTreeNode node = GetNode(line);
		node.LineNode.Height = val;
		UpdateAfterChildrenChange(node);
	}

	public bool GetIsCollapsed(int lineNumber)
	{
		HeightTreeNode nodeByIndex = GetNodeByIndex(lineNumber - 1);
		if (!nodeByIndex.LineNode.IsDirectlyCollapsed)
		{
			return GetIsCollapedFromNode(nodeByIndex);
		}
		return true;
	}

	public CollapsedLineSection CollapseText(DocumentLine start, DocumentLine end)
	{
		if (!_document.Lines.Contains(start))
		{
			throw new ArgumentException("Line is not part of this document", "start");
		}
		if (!_document.Lines.Contains(end))
		{
			throw new ArgumentException("Line is not part of this document", "end");
		}
		int num = end.LineNumber - start.LineNumber + 1;
		if (num < 0)
		{
			throw new ArgumentException("start must be a line before end");
		}
		CollapsedLineSection collapsedLineSection = new CollapsedLineSection(this, start, end);
		AddCollapsedSection(collapsedLineSection, num);
		return collapsedLineSection;
	}

	internal IEnumerable<CollapsedLineSection> GetAllCollapsedSections()
	{
		List<CollapsedLineSection> emptyCsList = new List<CollapsedLineSection>();
		return AllNodes.SelectMany((HeightTreeNode node) => (node.LineNode.CollapsedSections ?? emptyCsList).Concat(node.CollapsedSections ?? emptyCsList)).Distinct();
	}

	private void InsertAsLeft(HeightTreeNode parentNode, HeightTreeNode newNode)
	{
		parentNode.Left = newNode;
		newNode.Parent = parentNode;
		newNode.Color = true;
		UpdateAfterChildrenChange(parentNode);
		FixTreeOnInsert(newNode);
	}

	private void InsertAsRight(HeightTreeNode parentNode, HeightTreeNode newNode)
	{
		parentNode.Right = newNode;
		newNode.Parent = parentNode;
		newNode.Color = true;
		UpdateAfterChildrenChange(parentNode);
		FixTreeOnInsert(newNode);
	}

	private void FixTreeOnInsert(HeightTreeNode node)
	{
		HeightTreeNode parent = node.Parent;
		if (parent == null)
		{
			node.Color = false;
		}
		else
		{
			if (!parent.Color)
			{
				return;
			}
			HeightTreeNode parent2 = parent.Parent;
			HeightTreeNode heightTreeNode = Sibling(parent);
			if (heightTreeNode != null && heightTreeNode.Color)
			{
				parent.Color = false;
				heightTreeNode.Color = false;
				parent2.Color = true;
				FixTreeOnInsert(parent2);
				return;
			}
			if (node == parent.Right && parent == parent2.Left)
			{
				RotateLeft(parent);
				node = node.Left;
			}
			else if (node == parent.Left && parent == parent2.Right)
			{
				RotateRight(parent);
				node = node.Right;
			}
			parent = node.Parent;
			parent2 = parent.Parent;
			parent.Color = false;
			parent2.Color = true;
			if (node == parent.Left && parent == parent2.Left)
			{
				RotateRight(parent2);
			}
			else
			{
				RotateLeft(parent2);
			}
		}
	}

	private void RemoveNode(HeightTreeNode removedNode)
	{
		if (removedNode.Left != null && removedNode.Right != null)
		{
			HeightTreeNode leftMost = removedNode.Right.LeftMost;
			HeightTreeNode parent = leftMost.Parent;
			RemoveNode(leftMost);
			BeforeNodeReplace(removedNode, leftMost, parent);
			ReplaceNode(removedNode, leftMost);
			leftMost.Left = removedNode.Left;
			if (leftMost.Left != null)
			{
				leftMost.Left.Parent = leftMost;
			}
			leftMost.Right = removedNode.Right;
			if (leftMost.Right != null)
			{
				leftMost.Right.Parent = leftMost;
			}
			leftMost.Color = removedNode.Color;
			UpdateAfterChildrenChange(leftMost);
			if (leftMost.Parent != null)
			{
				UpdateAfterChildrenChange(leftMost.Parent);
			}
			return;
		}
		HeightTreeNode parent2 = removedNode.Parent;
		HeightTreeNode heightTreeNode = removedNode.Left ?? removedNode.Right;
		BeforeNodeRemove(removedNode);
		ReplaceNode(removedNode, heightTreeNode);
		if (parent2 != null)
		{
			UpdateAfterChildrenChange(parent2);
		}
		if (!removedNode.Color)
		{
			if (heightTreeNode != null && heightTreeNode.Color)
			{
				heightTreeNode.Color = false;
			}
			else
			{
				FixTreeOnDelete(heightTreeNode, parent2);
			}
		}
	}

	private void FixTreeOnDelete(HeightTreeNode node, HeightTreeNode parentNode)
	{
		if (parentNode == null)
		{
			return;
		}
		HeightTreeNode heightTreeNode = Sibling(node, parentNode);
		if (heightTreeNode.Color)
		{
			parentNode.Color = true;
			heightTreeNode.Color = false;
			if (node == parentNode.Left)
			{
				RotateLeft(parentNode);
			}
			else
			{
				RotateRight(parentNode);
			}
			heightTreeNode = Sibling(node, parentNode);
		}
		if (!parentNode.Color && !heightTreeNode.Color && !GetColor(heightTreeNode.Left) && !GetColor(heightTreeNode.Right))
		{
			heightTreeNode.Color = true;
			FixTreeOnDelete(parentNode, parentNode.Parent);
			return;
		}
		if (parentNode.Color && !heightTreeNode.Color && !GetColor(heightTreeNode.Left) && !GetColor(heightTreeNode.Right))
		{
			heightTreeNode.Color = true;
			parentNode.Color = false;
			return;
		}
		if (node == parentNode.Left && !heightTreeNode.Color && GetColor(heightTreeNode.Left) && !GetColor(heightTreeNode.Right))
		{
			heightTreeNode.Color = true;
			heightTreeNode.Left.Color = false;
			RotateRight(heightTreeNode);
		}
		else if (node == parentNode.Right && !heightTreeNode.Color && GetColor(heightTreeNode.Right) && !GetColor(heightTreeNode.Left))
		{
			heightTreeNode.Color = true;
			heightTreeNode.Right.Color = false;
			RotateLeft(heightTreeNode);
		}
		heightTreeNode = Sibling(node, parentNode);
		heightTreeNode.Color = parentNode.Color;
		parentNode.Color = false;
		if (node == parentNode.Left)
		{
			if (heightTreeNode.Right != null)
			{
				heightTreeNode.Right.Color = false;
			}
			RotateLeft(parentNode);
		}
		else
		{
			if (heightTreeNode.Left != null)
			{
				heightTreeNode.Left.Color = false;
			}
			RotateRight(parentNode);
		}
	}

	private void ReplaceNode(HeightTreeNode replacedNode, HeightTreeNode newNode)
	{
		if (replacedNode.Parent == null)
		{
			_root = newNode;
		}
		else if (replacedNode.Parent.Left == replacedNode)
		{
			replacedNode.Parent.Left = newNode;
		}
		else
		{
			replacedNode.Parent.Right = newNode;
		}
		if (newNode != null)
		{
			newNode.Parent = replacedNode.Parent;
		}
		replacedNode.Parent = null;
	}

	private void RotateLeft(HeightTreeNode p)
	{
		HeightTreeNode right = p.Right;
		ReplaceNode(p, right);
		p.Right = right.Left;
		if (p.Right != null)
		{
			p.Right.Parent = p;
		}
		right.Left = p;
		p.Parent = right;
		UpdateAfterRotateLeft(p);
	}

	private void RotateRight(HeightTreeNode p)
	{
		HeightTreeNode left = p.Left;
		ReplaceNode(p, left);
		p.Left = left.Right;
		if (p.Left != null)
		{
			p.Left.Parent = p;
		}
		left.Right = p;
		p.Parent = left;
		UpdateAfterRotateRight(p);
	}

	private static HeightTreeNode Sibling(HeightTreeNode node)
	{
		if (node == node.Parent.Left)
		{
			return node.Parent.Right;
		}
		return node.Parent.Left;
	}

	private static HeightTreeNode Sibling(HeightTreeNode node, HeightTreeNode parentNode)
	{
		if (node == parentNode.Left)
		{
			return parentNode.Right;
		}
		return parentNode.Left;
	}

	private static bool GetColor(HeightTreeNode node)
	{
		return node?.Color ?? false;
	}

	private static bool GetIsCollapedFromNode(HeightTreeNode node)
	{
		while (node != null)
		{
			if (node.IsDirectlyCollapsed)
			{
				return true;
			}
			node = node.Parent;
		}
		return false;
	}

	internal void AddCollapsedSection(CollapsedLineSection section, int sectionLength)
	{
		AddRemoveCollapsedSection(section, sectionLength, add: true);
	}

	private void AddRemoveCollapsedSection(CollapsedLineSection section, int sectionLength, bool add)
	{
		HeightTreeNode heightTreeNode = GetNode(section.Start);
		while (true)
		{
			if (add)
			{
				heightTreeNode.LineNode.AddDirectlyCollapsed(section);
			}
			else
			{
				heightTreeNode.LineNode.RemoveDirectlyCollapsed(section);
			}
			sectionLength--;
			if (sectionLength == 0)
			{
				break;
			}
			if (heightTreeNode.Right != null)
			{
				if (heightTreeNode.Right.TotalCount >= sectionLength)
				{
					AddRemoveCollapsedSectionDown(section, heightTreeNode.Right, sectionLength, add);
					break;
				}
				if (add)
				{
					heightTreeNode.Right.AddDirectlyCollapsed(section);
				}
				else
				{
					heightTreeNode.Right.RemoveDirectlyCollapsed(section);
				}
				sectionLength -= heightTreeNode.Right.TotalCount;
			}
			HeightTreeNode parent = heightTreeNode.Parent;
			while (parent.Right == heightTreeNode)
			{
				heightTreeNode = parent;
				parent = heightTreeNode.Parent;
			}
			heightTreeNode = parent;
		}
		UpdateAugmentedData(GetNode(section.Start), UpdateAfterChildrenChangeRecursionMode.WholeBranch);
		UpdateAugmentedData(GetNode(section.End), UpdateAfterChildrenChangeRecursionMode.WholeBranch);
	}

	private static void AddRemoveCollapsedSectionDown(CollapsedLineSection section, HeightTreeNode node, int sectionLength, bool add)
	{
		while (true)
		{
			if (node.Left != null)
			{
				if (node.Left.TotalCount >= sectionLength)
				{
					node = node.Left;
					continue;
				}
				if (add)
				{
					node.Left.AddDirectlyCollapsed(section);
				}
				else
				{
					node.Left.RemoveDirectlyCollapsed(section);
				}
				sectionLength -= node.Left.TotalCount;
			}
			if (add)
			{
				node.LineNode.AddDirectlyCollapsed(section);
			}
			else
			{
				node.LineNode.RemoveDirectlyCollapsed(section);
			}
			sectionLength--;
			if (sectionLength != 0)
			{
				node = node.Right;
				continue;
			}
			break;
		}
	}

	public void Uncollapse(CollapsedLineSection section)
	{
		int sectionLength = section.End.LineNumber - section.Start.LineNumber + 1;
		AddRemoveCollapsedSection(section, sectionLength, add: false);
	}
}
