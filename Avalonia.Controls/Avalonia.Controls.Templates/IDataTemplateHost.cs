using Avalonia.Metadata;

namespace Avalonia.Controls.Templates;

[NotClientImplementable]
public interface IDataTemplateHost
{
	DataTemplates DataTemplates { get; }

	bool IsDataTemplatesInitialized { get; }
}
