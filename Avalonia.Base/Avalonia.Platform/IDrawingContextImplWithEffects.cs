using Avalonia.Media;

namespace Avalonia.Platform;

public interface IDrawingContextImplWithEffects
{
	void PushEffect(IEffect effect);

	void PopEffect();
}
