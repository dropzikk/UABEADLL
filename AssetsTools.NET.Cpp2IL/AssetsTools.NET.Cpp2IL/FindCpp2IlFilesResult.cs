namespace AssetsTools.NET.Cpp2IL;

public struct FindCpp2IlFilesResult
{
	public string metaPath;

	public string asmPath;

	public bool success;

	public FindCpp2IlFilesResult(bool success)
	{
		metaPath = null;
		asmPath = null;
		this.success = success;
	}

	public FindCpp2IlFilesResult(string metaPath, string asmPath)
	{
		this.metaPath = metaPath;
		this.asmPath = asmPath;
		success = true;
	}
}
