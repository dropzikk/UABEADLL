namespace Avalonia.Controls.Templates;

public interface IRecyclingDataTemplate : IDataTemplate, ITemplate<object?, Control?>
{
	Control? Build(object? data, Control? existing);
}
