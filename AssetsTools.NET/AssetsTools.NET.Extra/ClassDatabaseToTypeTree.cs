using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssetsTools.NET.Extra;

public class ClassDatabaseToTypeTree
{
	private ClassDatabaseFile cldbFile;

	private uint stringTablePos;

	private Dictionary<string, uint> stringTableLookup;

	private Dictionary<string, uint> commonStringTableLookup;

	private List<TypeTreeNode> typeTreeNodes;

	public static TypeTreeType Convert(ClassDatabaseFile classes, string name, bool preferEditor = false)
	{
		ClassDatabaseType type = classes.FindAssetClassByName(name);
		return Convert(classes, type, preferEditor);
	}

	public static TypeTreeType Convert(ClassDatabaseFile classes, int id, bool preferEditor = false)
	{
		ClassDatabaseType type = classes.FindAssetClassByID(id);
		return Convert(classes, type, preferEditor);
	}

	public static TypeTreeType Convert(ClassDatabaseFile classes, ClassDatabaseType type, bool preferEditor = false)
	{
		ClassDatabaseToTypeTree classDatabaseToTypeTree = new ClassDatabaseToTypeTree();
		return classDatabaseToTypeTree.ConvertInternal(classes, type, preferEditor);
	}

	private TypeTreeType ConvertInternal(ClassDatabaseFile classes, ClassDatabaseType type, bool preferEditor = false)
	{
		TypeTreeType typeTreeType = new TypeTreeType();
		typeTreeType.TypeId = type.ClassId;
		typeTreeType.ScriptTypeIndex = ushort.MaxValue;
		typeTreeType.IsStrippedType = false;
		typeTreeType.ScriptIdHash = Hash128.NewBlankHash();
		typeTreeType.TypeHash = Hash128.NewBlankHash();
		typeTreeType.TypeDependencies = new int[0];
		TypeTreeType typeTreeType2 = typeTreeType;
		cldbFile = classes;
		stringTablePos = 0u;
		stringTableLookup = new Dictionary<string, uint>();
		commonStringTableLookup = new Dictionary<string, uint>();
		typeTreeNodes = new List<TypeTreeNode>();
		InitializeDefaultStringTableIndices();
		ClassDatabaseTypeNode preferredNode = type.GetPreferredNode(preferEditor);
		ConvertFields(preferredNode, 0);
		StringBuilder stringBuilder = new StringBuilder();
		List<KeyValuePair<string, uint>> list = stringTableLookup.OrderBy((KeyValuePair<string, uint> n) => n.Value).ToList();
		foreach (KeyValuePair<string, uint> item in list)
		{
			stringBuilder.Append(item.Key + "\0");
		}
		typeTreeType2.StringBuffer = stringBuilder.ToString();
		typeTreeType2.Nodes = typeTreeNodes;
		return typeTreeType2;
	}

	private void InitializeDefaultStringTableIndices()
	{
		int num = 0;
		List<ushort> commonStringBufferIndices = cldbFile.CommonStringBufferIndices;
		foreach (ushort item in commonStringBufferIndices)
		{
			string @string = cldbFile.StringTable.GetString(item);
			if (@string != string.Empty)
			{
				commonStringTableLookup.Add(@string, (uint)num);
				num += @string.Length + 1;
			}
		}
	}

	private void ConvertFields(ClassDatabaseTypeNode node, int depth)
	{
		string @string = cldbFile.GetString(node.FieldName);
		string string2 = cldbFile.GetString(node.TypeName);
		uint nameStrOffset;
		if (stringTableLookup.ContainsKey(@string))
		{
			nameStrOffset = stringTableLookup[@string];
		}
		else if (commonStringTableLookup.ContainsKey(@string))
		{
			nameStrOffset = commonStringTableLookup[@string] + 2147483648u;
		}
		else
		{
			nameStrOffset = stringTablePos;
			stringTableLookup.Add(@string, stringTablePos);
			stringTablePos += (uint)(@string.Length + 1);
		}
		uint typeStrOffset;
		if (stringTableLookup.ContainsKey(string2))
		{
			typeStrOffset = stringTableLookup[string2];
		}
		else if (commonStringTableLookup.ContainsKey(string2))
		{
			typeStrOffset = commonStringTableLookup[string2] + 2147483648u;
		}
		else
		{
			typeStrOffset = stringTablePos;
			stringTableLookup.Add(string2, stringTablePos);
			stringTablePos += (uint)(string2.Length + 1);
		}
		typeTreeNodes.Add(new TypeTreeNode
		{
			Level = (byte)depth,
			MetaFlags = node.MetaFlag,
			Index = (uint)typeTreeNodes.Count,
			TypeFlags = (TypeTreeNodeFlags)node.TypeFlags,
			NameStrOffset = nameStrOffset,
			ByteSize = node.ByteSize,
			TypeStrOffset = typeStrOffset,
			Version = node.Version
		});
		foreach (ClassDatabaseTypeNode child in node.Children)
		{
			ConvertFields(child, depth + 1);
		}
	}
}
