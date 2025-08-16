namespace AssetsTools.NET;

public class AssetsFileExternal
{
	/// <summary>
	/// Unknown.
	/// </summary>
	public string VirtualAssetPathName { get; set; }

	/// <summary>
	/// GUID for dependencies used in editor. Otherwise this is 0.
	/// </summary>
	public GUID128 Guid { get; set; }

	/// <summary>
	/// Dependency type.
	/// </summary>
	public AssetsFileExternalType Type { get; set; }

	/// <summary>
	/// Real path name to the other file.
	/// </summary>
	public string PathName { get; set; }

	/// <summary>
	/// Original path name listed in the assets file (if it was changed).
	/// You shouldn't modify this.
	/// </summary>
	public string OriginalPathName { get; set; }

	/// <summary>
	/// Read the <see cref="T:AssetsTools.NET.AssetsFileExternal" /> with the provided reader.
	/// </summary>
	/// <param name="reader">The reader to use.</param>
	public void Read(AssetsFileReader reader)
	{
		VirtualAssetPathName = reader.ReadNullTerminated();
		Guid = new GUID128(reader);
		Type = (AssetsFileExternalType)reader.ReadInt32();
		PathName = reader.ReadNullTerminated();
		OriginalPathName = PathName;
		if (PathName == "resources/unity_builtin_extra")
		{
			PathName = "Resources/unity_builtin_extra";
		}
		else if (PathName == "library/unity default resources" || PathName == "Library/unity default resources")
		{
			PathName = "Resources/unity default resources";
		}
		else if (PathName == "library/unity editor resources" || PathName == "Library/unity editor resources")
		{
			PathName = "Resources/unity editor resources";
		}
	}

	/// <summary>
	/// Write the <see cref="T:AssetsTools.NET.AssetsFileExternal" /> with the provided writer.
	/// </summary>
	/// <param name="writer">The writer to use.</param>
	public void Write(AssetsFileWriter writer)
	{
		writer.WriteNullTerminated(VirtualAssetPathName);
		Guid.Write(writer);
		writer.Write((int)Type);
		string text = PathName;
		if ((PathName == "Resources/unity_builtin_extra" || PathName == "Resources/unity default resources" || PathName == "Resources/unity editor resources") && OriginalPathName != string.Empty)
		{
			text = OriginalPathName;
		}
		writer.WriteNullTerminated(text);
	}
}
