namespace Avalonia.OpenGL.Controls;

internal interface IGlTexture
{
	int TextureId { get; }

	int InternalFormat { get; }

	PixelSize Size { get; }
}
