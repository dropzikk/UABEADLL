namespace Avalonia.Controls.Generators;

public class TreeItemContainerGenerator : ItemContainerGenerator
{
	public TreeContainerIndex Index { get; }

	internal TreeItemContainerGenerator(TreeView owner)
		: base(owner)
	{
		Index = new TreeContainerIndex(owner);
	}
}
