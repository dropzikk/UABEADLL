using System;

namespace Avalonia.Controls;

public interface IResourceProvider : IResourceNode
{
	IResourceHost? Owner { get; }

	event EventHandler? OwnerChanged;

	void AddOwner(IResourceHost owner);

	void RemoveOwner(IResourceHost owner);
}
