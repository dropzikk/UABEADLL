namespace AvaloniaEdit;

public static class EditingCommands
{
	public static RoutedCommand Delete { get; } = new RoutedCommand("Delete");

	public static RoutedCommand DeleteNextWord { get; } = new RoutedCommand("DeleteNextWord");

	public static RoutedCommand Backspace { get; } = new RoutedCommand("Backspace");

	public static RoutedCommand DeletePreviousWord { get; } = new RoutedCommand("DeletePreviousWord");

	public static RoutedCommand EnterParagraphBreak { get; } = new RoutedCommand("EnterParagraphBreak");

	public static RoutedCommand EnterLineBreak { get; } = new RoutedCommand("EnterLineBreak");

	public static RoutedCommand TabForward { get; } = new RoutedCommand("TabForward");

	public static RoutedCommand TabBackward { get; } = new RoutedCommand("TabBackward");

	public static RoutedCommand MoveLeftByCharacter { get; } = new RoutedCommand("MoveLeftByCharacter");

	public static RoutedCommand SelectLeftByCharacter { get; } = new RoutedCommand("SelectLeftByCharacter");

	public static RoutedCommand MoveRightByCharacter { get; } = new RoutedCommand("MoveRightByCharacter");

	public static RoutedCommand SelectRightByCharacter { get; } = new RoutedCommand("SelectRightByCharacter");

	public static RoutedCommand MoveLeftByWord { get; } = new RoutedCommand("MoveLeftByWord");

	public static RoutedCommand SelectLeftByWord { get; } = new RoutedCommand("SelectLeftByWord");

	public static RoutedCommand MoveRightByWord { get; } = new RoutedCommand("MoveRightByWord");

	public static RoutedCommand SelectRightByWord { get; } = new RoutedCommand("SelectRightByWord");

	public static RoutedCommand MoveUpByLine { get; } = new RoutedCommand("MoveUpByLine");

	public static RoutedCommand SelectUpByLine { get; } = new RoutedCommand("SelectUpByLine");

	public static RoutedCommand MoveDownByLine { get; } = new RoutedCommand("MoveDownByLine");

	public static RoutedCommand SelectDownByLine { get; } = new RoutedCommand("SelectDownByLine");

	public static RoutedCommand MoveDownByPage { get; } = new RoutedCommand("MoveDownByPage");

	public static RoutedCommand SelectDownByPage { get; } = new RoutedCommand("SelectDownByPage");

	public static RoutedCommand MoveUpByPage { get; } = new RoutedCommand("MoveUpByPage");

	public static RoutedCommand SelectUpByPage { get; } = new RoutedCommand("SelectUpByPage");

	public static RoutedCommand MoveToLineStart { get; } = new RoutedCommand("MoveToLineStart");

	public static RoutedCommand SelectToLineStart { get; } = new RoutedCommand("SelectToLineStart");

	public static RoutedCommand MoveToLineEnd { get; } = new RoutedCommand("MoveToLineEnd");

	public static RoutedCommand SelectToLineEnd { get; } = new RoutedCommand("SelectToLineEnd");

	public static RoutedCommand MoveToDocumentStart { get; } = new RoutedCommand("MoveToDocumentStart");

	public static RoutedCommand SelectToDocumentStart { get; } = new RoutedCommand("SelectToDocumentStart");

	public static RoutedCommand MoveToDocumentEnd { get; } = new RoutedCommand("MoveToDocumentEnd");

	public static RoutedCommand SelectToDocumentEnd { get; } = new RoutedCommand("SelectToDocumentEnd");
}
