namespace AssetsTools.NET.Extra;

public interface IMonoBehaviourTemplateGenerator
{
	AssetTypeTemplateField GetTemplateField(AssetTypeTemplateField baseField, string assemblyName, string nameSpace, string className, UnityVersion unityVersion);

	void Dispose();
}
