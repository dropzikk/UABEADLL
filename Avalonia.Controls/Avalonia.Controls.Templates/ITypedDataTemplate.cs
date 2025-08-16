using System;
using Avalonia.Metadata;

namespace Avalonia.Controls.Templates;

public interface ITypedDataTemplate : IDataTemplate, ITemplate<object?, Control?>
{
	[DataType]
	Type? DataType { get; }
}
