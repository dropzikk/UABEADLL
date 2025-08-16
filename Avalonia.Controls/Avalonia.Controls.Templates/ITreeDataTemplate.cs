using Avalonia.Data;

namespace Avalonia.Controls.Templates;

public interface ITreeDataTemplate : IDataTemplate, ITemplate<object?, Control?>
{
	InstancedBinding? ItemsSelector(object item);
}
