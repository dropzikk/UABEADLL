namespace AssetsTools.NET.Extra;

public class UnityVersion
{
	public int major;

	public int minor;

	public int patch;

	public string type;

	public int typeNum;

	public UnityVersion()
	{
	}

	public UnityVersion(string version)
	{
		string[] array = version.Split(new char[1] { '.' });
		major = int.Parse(array[0]);
		minor = int.Parse(array[1]);
		int num = array[2].IndexOfAny(new char[6] { 'f', 'p', 'a', 'b', 'c', 'x' });
		if (num != -1)
		{
			type = array[2][num].ToString();
			patch = int.Parse(array[2].Substring(0, num));
			string text = array[2].Substring(num + 1);
			if (!int.TryParse(text, out typeNum))
			{
				string text2 = "";
				for (int i = 0; i < text.Length && text[i] >= '0' && text[i] <= '9'; i++)
				{
					text2 += text[i];
				}
				if (text2.Length > 0)
				{
					typeNum = int.Parse(text2);
				}
			}
		}
		else
		{
			patch = int.Parse(array[2]);
			type = "";
			typeNum = 0;
		}
	}

	public override string ToString()
	{
		if (type == string.Empty)
		{
			return $"{major}.{minor}.{patch}";
		}
		return $"{major}.{minor}.{patch}{type}{typeNum}";
	}

	public ulong ToUInt64()
	{
		string text = type;
		if (1 == 0)
		{
		}
		byte b = text switch
		{
			"a" => 0, 
			"b" => 1, 
			"c" => 2, 
			"f" => 3, 
			"p" => 4, 
			"x" => 5, 
			_ => byte.MaxValue, 
		};
		if (1 == 0)
		{
		}
		byte b2 = b;
		return (ulong)(((long)major << 48) | ((long)minor << 32) | ((long)patch << 16)) | ((ulong)b2 << 8) | (uint)typeNum;
	}

	public static UnityVersion FromUInt64(ulong data)
	{
		UnityVersion unityVersion = new UnityVersion();
		unityVersion.major = (int)((data >> 48) & 0xFFFF);
		unityVersion.minor = (int)((data >> 32) & 0xFFFF);
		unityVersion.patch = (int)((data >> 16) & 0xFFFF);
		UnityVersion unityVersion2 = unityVersion;
		ulong num = (data >> 8) & 0xFF;
		if (1 == 0)
		{
		}
		if (num > 5)
		{
			goto IL_00ac;
		}
		switch (num)
		{
		case 0uL:
			break;
		case 1uL:
			goto IL_0084;
		case 2uL:
			goto IL_008c;
		case 3uL:
			goto IL_0094;
		case 4uL:
			goto IL_009c;
		case 5uL:
			goto IL_00a4;
		default:
			goto IL_00ac;
		}
		string text = "a";
		goto IL_00b4;
		IL_00a4:
		text = "x";
		goto IL_00b4;
		IL_00ac:
		text = "?";
		goto IL_00b4;
		IL_00b4:
		if (1 == 0)
		{
		}
		unityVersion2.type = text;
		unityVersion.typeNum = (int)(data & 0xFF);
		return unityVersion;
		IL_0084:
		text = "b";
		goto IL_00b4;
		IL_008c:
		text = "c";
		goto IL_00b4;
		IL_0094:
		text = "f";
		goto IL_00b4;
		IL_009c:
		text = "p";
		goto IL_00b4;
	}
}
