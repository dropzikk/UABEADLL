namespace Avalonia.Controls;

public class NativeMenuItemBase : AvaloniaObject
{
	private NativeMenu? _parent;

	public static readonly DirectProperty<NativeMenuItemBase, NativeMenu?> ParentProperty = AvaloniaProperty.RegisterDirect("Parent", (NativeMenuItemBase o) => o.Parent);

	public NativeMenu? Parent
	{
		get
		{
			return _parent;
		}
		internal set
		{
			SetAndRaise(ParentProperty, ref _parent, value);
		}
	}

	internal NativeMenuItemBase()
	{
	}
}
