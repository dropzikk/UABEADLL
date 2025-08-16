namespace Avalonia.Controls.Templates;

public interface IDataTemplate : ITemplate<object?, Control?>
{
	bool Match(object? data);
}
