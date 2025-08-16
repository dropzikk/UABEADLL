namespace Avalonia.Media;

public readonly record struct TextHitTestResult(CharacterHit characterHit, int textPosition, bool isInside, bool isTrailing);
