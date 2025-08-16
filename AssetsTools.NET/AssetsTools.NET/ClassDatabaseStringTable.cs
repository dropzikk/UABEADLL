using System.Collections.Generic;

namespace AssetsTools.NET;

public class ClassDatabaseStringTable
{
	public List<string> Strings { get; set; }

	public void Read(AssetsFileReader reader)
	{
		int num = reader.ReadInt32();
		Strings = new List<string>(num);
		for (int i = 0; i < num; i++)
		{
			Strings.Add(reader.ReadString());
		}
	}

	public void Write(AssetsFileWriter writer)
	{
		writer.Write(Strings.Count);
		for (int i = 0; i < Strings.Count; i++)
		{
			writer.Write(Strings[i]);
		}
	}

	public ushort AddString(string str)
	{
		int num = Strings.IndexOf(str);
		if (num == -1)
		{
			num = Strings.Count;
			Strings.Add(str);
		}
		return (ushort)num;
	}

	/// <summary>
	/// Get a string from the string table.
	/// </summary>
	/// <param name="index">The index of the string in the table.</param>
	/// <returns>The string at that index.</returns>
	public string GetString(ushort index)
	{
		return Strings[index];
	}
}
