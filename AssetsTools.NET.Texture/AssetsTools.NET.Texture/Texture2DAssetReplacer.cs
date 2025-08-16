using AssetsTools.NET.Extra;

namespace AssetsTools.NET.Texture;

public abstract class Texture2DAssetReplacer : SerializingAssetReplacer
{
	protected Texture2DAssetReplacer(AssetsManager manager, AssetsFileInstance assetsFile, AssetFileInfo asset)
		: base(manager, assetsFile, asset)
	{
	}

	public override ushort GetMonoScriptID()
	{
		return ushort.MaxValue;
	}

	protected override void Modify(AssetTypeValueField baseField)
	{
		TextureFile textureFile = TextureFile.ReadTextureFile(baseField);
		GetNewTextureData(out var bgra, out var width, out var height);
		SetTextureData(textureFile, bgra, width, height);
		textureFile.WriteTo(baseField);
	}

	protected abstract void GetNewTextureData(out byte[] bgra, out int width, out int height);

	protected virtual void SetTextureData(TextureFile texture, byte[] bgra, int width, int height)
	{
		texture.SetTextureData(bgra, width, height);
	}
}
