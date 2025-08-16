namespace Avalonia.Styling;

public abstract class SetterBase
{
	internal abstract ISetterInstance Instance(IStyleInstance styleInstance, StyledElement target);
}
