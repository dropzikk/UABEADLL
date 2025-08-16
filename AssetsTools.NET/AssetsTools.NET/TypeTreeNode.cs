using System.Text;

namespace AssetsTools.NET;

public class TypeTreeNode
{
	/// <summary>
	/// Version of the node.
	/// </summary>
	public ushort Version { get; set; }

	/// <summary>
	/// Level of the node (0 for root, 1 for child, etc.)
	/// </summary>
	public byte Level { get; set; }

	/// <summary>
	/// Information about whether the node is an array, registry, etc.
	/// </summary>
	public TypeTreeNodeFlags TypeFlags { get; set; }

	/// <summary>
	/// Offset of the type string in the string table.
	/// </summary>
	public uint TypeStrOffset { get; set; }

	/// <summary>
	/// Offset of the name string in the string table.
	/// </summary>
	public uint NameStrOffset { get; set; }

	/// <summary>
	/// Byte size of the field's type (for example, int is 4).
	/// If the field isn't a value type, then this value is a sum of all children sizes.
	/// If the size is variable, this is set to -1.
	/// </summary>
	public int ByteSize { get; set; }

	/// <summary>
	/// Index in the type tree. This should always be the same as the index in the array.
	/// </summary>
	public uint Index { get; set; }

	/// <summary>
	/// 0x4000 if aligned.
	/// </summary>
	public uint MetaFlags { get; set; }

	/// <summary>
	/// Unknown.
	/// </summary>
	public ulong RefTypeHash { get; set; }

	/// <summary>
	/// Read the <see cref="T:AssetsTools.NET.TypeTreeNode" /> with the provided reader and format version.
	/// </summary>
	/// <param name="reader">The reader to use.</param>
	/// <param name="version">The version of the file.</param>
	public void Read(AssetsFileReader reader, uint version)
	{
		Version = reader.ReadUInt16();
		Level = reader.ReadByte();
		TypeFlags = (TypeTreeNodeFlags)reader.ReadByte();
		TypeStrOffset = reader.ReadUInt32();
		NameStrOffset = reader.ReadUInt32();
		ByteSize = reader.ReadInt32();
		Index = reader.ReadUInt32();
		MetaFlags = reader.ReadUInt32();
		if (version >= 18)
		{
			RefTypeHash = reader.ReadUInt64();
		}
	}

	/// <summary>
	/// Write the <see cref="T:AssetsTools.NET.TypeTreeNode" /> with the provided writer and format version.
	/// </summary>
	/// <param name="writer">The writer to use.</param>
	/// <param name="version">The version of the file.</param>
	public void Write(AssetsFileWriter writer, uint version)
	{
		writer.Write(Version);
		writer.Write(Level);
		writer.Write((byte)TypeFlags);
		writer.Write(TypeStrOffset);
		writer.Write(NameStrOffset);
		writer.Write(ByteSize);
		writer.Write(Index);
		writer.Write(MetaFlags);
		if (version >= 18)
		{
			writer.Write(RefTypeHash);
		}
	}

	/// <summary>
	/// Get the type name from the string table (from <see cref="P:AssetsTools.NET.TypeTreeType.StringBuffer" />).
	/// </summary>
	/// <param name="stringTable">The string table to use.</param>
	/// <param name="commonStringTable">
	/// The common string table to use, if the builtin one is outdated.
	/// See <see cref="P:AssetsTools.NET.ClassDatabaseFile.CommonStringBufferIndices" />.
	/// </param>
	/// <returns>The node type name.</returns>
	public string GetTypeString(string stringTable, string commonStringTable = null)
	{
		return ReadStringTableString(stringTable, commonStringTable ?? "AABB\0AnimationClip\0AnimationCurve\0AnimationState\0Array\0Base\0BitField\0bitset\0bool\0char\0ColorRGBA\0Component\0data\0deque\0double\0dynamic_array\0FastPropertyName\0first\0float\0Font\0GameObject\0Generic Mono\0GradientNEW\0GUID\0GUIStyle\0int\0list\0long long\0map\0Matrix4x4f\0MdFour\0MonoBehaviour\0MonoScript\0m_ByteSize\0m_Curve\0m_EditorClassIdentifier\0m_EditorHideFlags\0m_Enabled\0m_ExtensionPtr\0m_GameObject\0m_Index\0m_IsArray\0m_IsStatic\0m_MetaFlag\0m_Name\0m_ObjectHideFlags\0m_PrefabInternal\0m_PrefabParentObject\0m_Script\0m_StaticEditorFlags\0m_Type\0m_Version\0Object\0pair\0PPtr<Component>\0PPtr<GameObject>\0PPtr<Material>\0PPtr<MonoBehaviour>\0PPtr<MonoScript>\0PPtr<Object>\0PPtr<Prefab>\0PPtr<Sprite>\0PPtr<TextAsset>\0PPtr<Texture>\0PPtr<Texture2D>\0PPtr<Transform>\0Prefab\0Quaternionf\0Rectf\0RectInt\0RectOffset\0second\0set\0short\0size\0SInt16\0SInt32\0SInt64\0SInt8\0staticvector\0string\0TextAsset\0TextMesh\0Texture\0Texture2D\0Transform\0TypelessData\0UInt16\0UInt32\0UInt64\0UInt8\0unsigned int\0unsigned long long\0unsigned short\0vector\0Vector2f\0Vector3f\0Vector4f\0m_ScriptingClassIdentifier\0Gradient\0Type*\0int2_storage\0int3_storage\0BoundsInt\0m_CorrespondingSourceObject\0m_PrefabInstance\0m_PrefabAsset\0FileSize\0Hash128\0", TypeStrOffset);
	}

	/// <summary>
	/// Get the name name from the string table (from <see cref="P:AssetsTools.NET.TypeTreeType.StringBuffer" />).
	/// </summary>
	/// <param name="stringTable">The string table to use.</param>
	/// <param name="commonStringTable">
	/// The common string table to use, if the builtin one is outdated.
	/// See <see cref="P:AssetsTools.NET.ClassDatabaseFile.CommonStringBufferIndices" />.
	/// </param>
	/// <returns>The node name.</returns>
	public string GetNameString(string stringTable, string commonStringTable = null)
	{
		return ReadStringTableString(stringTable, commonStringTable ?? "AABB\0AnimationClip\0AnimationCurve\0AnimationState\0Array\0Base\0BitField\0bitset\0bool\0char\0ColorRGBA\0Component\0data\0deque\0double\0dynamic_array\0FastPropertyName\0first\0float\0Font\0GameObject\0Generic Mono\0GradientNEW\0GUID\0GUIStyle\0int\0list\0long long\0map\0Matrix4x4f\0MdFour\0MonoBehaviour\0MonoScript\0m_ByteSize\0m_Curve\0m_EditorClassIdentifier\0m_EditorHideFlags\0m_Enabled\0m_ExtensionPtr\0m_GameObject\0m_Index\0m_IsArray\0m_IsStatic\0m_MetaFlag\0m_Name\0m_ObjectHideFlags\0m_PrefabInternal\0m_PrefabParentObject\0m_Script\0m_StaticEditorFlags\0m_Type\0m_Version\0Object\0pair\0PPtr<Component>\0PPtr<GameObject>\0PPtr<Material>\0PPtr<MonoBehaviour>\0PPtr<MonoScript>\0PPtr<Object>\0PPtr<Prefab>\0PPtr<Sprite>\0PPtr<TextAsset>\0PPtr<Texture>\0PPtr<Texture2D>\0PPtr<Transform>\0Prefab\0Quaternionf\0Rectf\0RectInt\0RectOffset\0second\0set\0short\0size\0SInt16\0SInt32\0SInt64\0SInt8\0staticvector\0string\0TextAsset\0TextMesh\0Texture\0Texture2D\0Transform\0TypelessData\0UInt16\0UInt32\0UInt64\0UInt8\0unsigned int\0unsigned long long\0unsigned short\0vector\0Vector2f\0Vector3f\0Vector4f\0m_ScriptingClassIdentifier\0Gradient\0Type*\0int2_storage\0int3_storage\0BoundsInt\0m_CorrespondingSourceObject\0m_PrefabInstance\0m_PrefabAsset\0FileSize\0Hash128\0", NameStrOffset);
	}

	private string ReadStringTableString(string stringTable, string commonStringTable, uint offset)
	{
		if (offset >= 2147483648u)
		{
			offset &= 0x7FFFFFFF;
			stringTable = commonStringTable;
		}
		StringBuilder stringBuilder = new StringBuilder();
		int num = (int)offset;
		char value;
		while ((value = stringTable[num]) != 0)
		{
			stringBuilder.Append(value);
			num++;
		}
		return stringBuilder.ToString();
	}
}
