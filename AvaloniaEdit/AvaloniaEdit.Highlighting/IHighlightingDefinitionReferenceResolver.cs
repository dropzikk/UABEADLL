namespace AvaloniaEdit.Highlighting;

public interface IHighlightingDefinitionReferenceResolver
{
	IHighlightingDefinition GetDefinition(string name);
}
