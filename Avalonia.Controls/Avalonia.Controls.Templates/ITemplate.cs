namespace Avalonia.Controls.Templates;

public interface ITemplate<TParam, TControl>
{
	TControl Build(TParam param);
}
