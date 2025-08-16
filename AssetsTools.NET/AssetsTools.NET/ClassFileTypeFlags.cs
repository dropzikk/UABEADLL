using System;

namespace AssetsTools.NET;

[Flags]
public enum ClassFileTypeFlags : byte
{
	/// <summary>
	/// None of the flags apply to this class
	/// </summary>
	None = 0,
	/// <summary>
	/// Is the class abstract?
	/// </summary>
	IsAbstract = 1,
	/// <summary>
	/// Is the class sealed? Not necessarily accurate.
	/// </summary>
	IsSealed = 2,
	/// <summary>
	/// Does the class only appear in the editor?
	/// </summary>
	IsEditorOnly = 4,
	/// <summary>
	/// Does the class only appear in game files? Not currently used.
	/// </summary>
	IsReleaseOnly = 8,
	/// <summary>
	/// Is the class stripped?
	/// </summary>
	IsStripped = 0x10,
	/// <summary>
	/// Not currently used
	/// </summary>
	Reserved = 0x20,
	/// <summary>
	/// Does the class have an editor root node?
	/// </summary>
	HasEditorRootNode = 0x40,
	/// <summary>
	/// Does the class have a release root node?
	/// </summary>
	HasReleaseRootNode = 0x80
}
