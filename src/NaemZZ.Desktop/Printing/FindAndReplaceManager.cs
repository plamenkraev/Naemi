using System.Windows.Documents;

namespace NaemZZ.Desktop.Printing;

/// <summary>
/// RTF find/replace helper ported unchanged from the legacy NaemZZ application
/// (original: FindAndReplaceManager.cs). Pure WPF (System.Windows.Documents), no
/// changes needed for .NET 10.
/// </summary>
public sealed class FindAndReplaceManager
{
    private readonly FlowDocument inputDocument;

    private TextPointer currentPosition;

    public TextPointer CurrentPosition
    {
        get => currentPosition;
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            if (value.CompareTo(inputDocument.ContentStart) < 0 || value.CompareTo(inputDocument.ContentEnd) > 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
            currentPosition = value;
        }
    }

    public FindAndReplaceManager(FlowDocument inputDocument)
    {
        ArgumentNullException.ThrowIfNull(inputDocument);
        this.inputDocument = inputDocument;
        currentPosition = inputDocument.ContentStart;
    }

    public TextRange? FindNext(string input, FindOptions findOptions) =>
        GetTextRangeFromPosition(ref currentPosition, input, findOptions);

    public TextRange? Replace(string input, string replacement, FindOptions findOptions)
    {
        var textRange = FindNext(input, findOptions);
        if (textRange != null)
        {
            textRange.Text = replacement;
        }
        return textRange;
    }

    public int ReplaceAll(string input, string replacement, FindOptions findOptions, Action<TextRange>? action)
    {
        var count = 0;
        currentPosition = inputDocument.ContentStart;
        while (currentPosition.CompareTo(inputDocument.ContentEnd) < 0)
        {
            var textRange = Replace(input, replacement, findOptions);
            if (textRange != null)
            {
                count++;
                action?.Invoke(textRange);
            }
            else
            {
                break;
            }
        }
        return count;
    }

    public TextRange? GetTextRangeFromPosition(ref TextPointer position, string input, FindOptions findOptions)
    {
        var matchCase = (findOptions & FindOptions.MatchCase) == FindOptions.MatchCase;
        var matchWholeWord = (findOptions & FindOptions.MatchWholeWord) == FindOptions.MatchWholeWord;
        TextRange? textRange = null;

        while (position != null && position.CompareTo(inputDocument.ContentEnd) != 0)
        {
            if (position.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
            {
                var textRun = position.GetTextInRun(LogicalDirection.Forward);
                var stringComparison = matchCase ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase;
                var indexInRun = textRun.IndexOf(input, stringComparison);
                if (indexInRun >= 0)
                {
                    position = position.GetPositionAtOffset(indexInRun)!;
                    var nextPointer = position.GetPositionAtOffset(input.Length)!;
                    textRange = new TextRange(position, nextPointer);
                    if (matchWholeWord)
                    {
                        if (IsWholeWord(textRange))
                        {
                            position = position.GetPositionAtOffset(input.Length)!;
                            break;
                        }
                        position = position.GetPositionAtOffset(input.Length)!;
                        return GetTextRangeFromPosition(ref position, input, findOptions);
                    }
                    position = position.GetPositionAtOffset(input.Length)!;
                    break;
                }
                position = position.GetPositionAtOffset(textRun.Length)!;
            }
            else
            {
                position = position.GetNextContextPosition(LogicalDirection.Forward)!;
            }
        }
        return textRange;
    }

    private static bool IsWordChar(char character) => char.IsLetterOrDigit(character) || character == '_';

    private bool IsWholeWord(TextRange textRange)
    {
        var chars = new char[1];
        if (textRange.Start.CompareTo(inputDocument.ContentStart) == 0 || textRange.Start.IsAtLineStartPosition)
        {
            textRange.End.GetTextInRun(LogicalDirection.Forward, chars, 0, 1);
            return !IsWordChar(chars[0]);
        }
        if (textRange.End.CompareTo(inputDocument.ContentEnd) == 0)
        {
            textRange.Start.GetTextInRun(LogicalDirection.Backward, chars, 0, 1);
            return !IsWordChar(chars[0]);
        }
        textRange.End.GetTextInRun(LogicalDirection.Forward, chars, 0, 1);
        if (!IsWordChar(chars[0]))
        {
            textRange.Start.GetTextInRun(LogicalDirection.Backward, chars, 0, 1);
            return !IsWordChar(chars[0]);
        }
        return false;
    }
}
