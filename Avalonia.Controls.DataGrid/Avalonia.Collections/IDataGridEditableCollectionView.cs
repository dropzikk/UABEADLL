namespace Avalonia.Collections;

internal interface IDataGridEditableCollectionView
{
	bool CanAddNew { get; }

	bool IsAddingNew { get; }

	object CurrentAddItem { get; }

	bool CanRemove { get; }

	bool CanCancelEdit { get; }

	bool IsEditingItem { get; }

	object CurrentEditItem { get; }

	object AddNew();

	void CommitNew();

	void CancelNew();

	void RemoveAt(int index);

	void Remove(object item);

	void EditItem(object item);

	void CommitEdit();

	void CancelEdit();
}
