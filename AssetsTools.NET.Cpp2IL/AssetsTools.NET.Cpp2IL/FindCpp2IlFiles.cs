using System.IO;

namespace AssetsTools.NET.Cpp2IL;

public static class FindCpp2IlFiles
{
	public static FindCpp2IlFilesResult Find(string fileDir)
	{
		string text = Path.Combine(fileDir, "il2cpp_data", "Metadata", "global-metadata.dat");
		string text2 = Path.Combine(fileDir, "..", "GameAssembly.dll");
		string text3 = Path.Combine(fileDir, "..", "GameAssembly.so");
		string text4 = Path.Combine(fileDir, "Managed", "Metadata", "global-metadata.dat");
		string path = Path.Combine(fileDir, "..", "..", "..", "lib");
		string text5 = Path.Combine(path, "x86", "libil2cpp.so");
		string text6 = Path.Combine(path, "x86_64", "libil2cpp.so");
		string text7 = Path.Combine(path, "arm64-v8a", "libil2cpp.so");
		string text8 = Path.Combine(path, "armeabi-v7a", "libil2cpp.so");
		if (File.Exists(text))
		{
			if (File.Exists(text2))
			{
				return new FindCpp2IlFilesResult(text, text2);
			}
			if (File.Exists(text3))
			{
				return new FindCpp2IlFilesResult(text, text3);
			}
		}
		else if (File.Exists(text4))
		{
			if (File.Exists(text5))
			{
				return new FindCpp2IlFilesResult(text4, text5);
			}
			if (File.Exists(text6))
			{
				return new FindCpp2IlFilesResult(text4, text6);
			}
			if (File.Exists(text7))
			{
				return new FindCpp2IlFilesResult(text4, text7);
			}
			if (File.Exists(text8))
			{
				return new FindCpp2IlFilesResult(text4, text8);
			}
		}
		return new FindCpp2IlFilesResult(success: false);
	}
}
