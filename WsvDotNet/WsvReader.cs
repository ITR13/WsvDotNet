using System.Text;

namespace WsvNet;

public static class WsvReader
{
    private static readonly HashSet<char> WsChar =
    [
        '\u0009', '\u000B', '\u000C', '\u000D', '\u0020', '\u0085', '\u00A0', '\u1680',
        '\u2000', '\u2001', '\u2002', '\u2003', '\u2004', '\u2005', '\u2006', '\u2007',
        '\u2008', '\u2009', '\u200A', '\u2028', '\u2029', '\u202F', '\u205F', '\u3000'
    ];

    private const char LineBreakChar = '\u000A';
    private const char HashChar = '#';
    private const char DoubleQuoteChar = '\u0022';
    private const char MinusChar = '-';
    private const char SlashChar = '/';


    /// <summary>
    /// Reads a wsvText from a file.
    /// </summary>
    /// <param name="filePath">The path to the file</param>
    /// <param name="expectedColumnCount">The amount of columns expected on each line</param>
    /// <returns></returns>
    public static string?[][] ReadFromFile(string filePath, int expectedColumnCount = 0)
    {
        var wsvText = File.ReadAllText(filePath);
        return Read(wsvText.AsSpan(), 0, expectedColumnCount);
    }

    /// <summary>
    /// Reads a wsvText from a stream.
    /// </summary>
    /// <param name="stream">The stream to read from</param>
    /// <param name="expectedColumnCount">The amount of columns expected on each line</param>
    /// <returns></returns>
    public static string?[][] ReadFromStream(Stream stream, int expectedColumnCount = 0)
    {
        using var reader = new StreamReader(stream);
        var wsvText = reader.ReadToEnd();
        return Read(wsvText.AsSpan(), 0, expectedColumnCount);
    }

    /// <summary>
    /// Reads a wsvText from a char span.
    /// </summary>
    /// <param name="wsvText">The text to read</param>
    /// <param name="index">The first character in the span to read from</param>
    /// <param name="expectedColumnCount">The amount of columns expected on each line</param>
    /// <returns></returns>
    public static string?[][] Read(ReadOnlySpan<char> wsvText, int index = 0, int expectedColumnCount = 0)
    {
        var lines = new List<string?[]>();
        var emptyLineAtEnd = false;
        while (index < wsvText.Length)
        {
            emptyLineAtEnd = ReadLine(wsvText, lines.Count, ref index, expectedColumnCount, out var line);
            lines.Add(line.ToArray());

            var columns = line.Count;
            if (columns > expectedColumnCount)
            {
                expectedColumnCount = columns;
            }
        }

        if (emptyLineAtEnd)
        {
            lines.Add(Array.Empty<string?>());
        }

        return lines.ToArray();
    }

    /// <summary>
    /// Reads a WsvLine and a LineBreak
    /// </summary>
    /// <param name="wsvText">The text we read from</param>
    /// <param name="lineNumber">What line we are currently reading</param>
    /// <param name="index">The current index we're evaluating</param>
    /// <param name="maxColumnsThusFar">The expected amount of columns (used for smarter allocation)</param>
    /// <param name="line">The line that was read</param>
    /// <returns>True if a linebreak was consumed at the end, false otherwise</returns>
    private static bool ReadLine(
        ReadOnlySpan<char> wsvText,
        int lineNumber,
        ref int index,
        int maxColumnsThusFar,
        out List<string?> line
    )
    {
        var lineStart = index;
        line = new List<string?>(maxColumnsThusFar);
        while (
            index < wsvText.Length &&
            ReadLineValue(wsvText, lineNumber, lineStart, ref index, out var lineValue)
        )
        {
            line.Add(lineValue);
        }

        // NB: index is increased after reading linebreak
        return index < wsvText.Length && wsvText[index++] == LineBreakChar;
    }

    /// <summary>
    /// Reads a line value, either a string, value, or null
    /// </summary>
    /// <param name="wsvText">The text we read from</param>
    /// <param name="lineNumber">What line we are currently reading</param>
    /// <param name="lineStart">What index the line started at</param>
    /// <param name="index">The current index we're evaluating.</param>
    /// <param name="lineValue"></param>
    /// <returns></returns>
    private static bool ReadLineValue(
        ReadOnlySpan<char> wsvText,
        int lineNumber,
        int lineStart,
        ref int index,
        out string? lineValue
    )
    {
        lineValue = null;
        while (WsChar.Contains(wsvText[index]))
        {
            index++;
            if (index >= wsvText.Length)
            {
                return false;
            }
        }

        switch (wsvText[index])
        {
            case MinusChar:
                index++;
                return true;
            case DoubleQuoteChar:
                index++;
                lineValue = ReadStringAfterQuote(wsvText, lineNumber, lineStart, ref index);
                return true;
            case LineBreakChar:
                return false;
            case HashChar:
                index++;
                break;
            default:
                lineValue = ReadValue(wsvText, lineNumber, lineStart, ref index);
                return true;
        }

        while (index < wsvText.Length && wsvText[index] != LineBreakChar)
        {
            index++;
        }

        return false;
    }

    private static string ReadStringAfterQuote(ReadOnlySpan<char> wsvText, int lineNumber, int lineStart, ref int index)
    {
        var trueStart = index;
        var startIndex = index;
        var sb = new StringBuilder(wsvText.Length - index);
        while (true)
        {
            if (index >= wsvText.Length || wsvText[index] == LineBreakChar)
            {
                throw new StringNotClosedException(lineNumber, trueStart - lineStart, index - lineStart, index);
            }

            if (wsvText[index] != DoubleQuoteChar)
            {
                index++;
                continue;
            }

            index++;
            if (index >= wsvText.Length)
            {
                break;
            }

            if (wsvText[index] == DoubleQuoteChar)
            {
                index++;
                // -2 at end because we read the two characters ""
                sb.Append(wsvText.Slice(startIndex, index - startIndex - 2));
                sb.Append('"');
                startIndex = index;
                continue;
            }

            if (WsChar.Contains(wsvText[index]))
            {
                break;
            }

            if (wsvText[index] != SlashChar)
            {
                throw new InvalidCharacterAfterStringException(
                    lineNumber,
                    trueStart - lineStart,
                    index - lineStart,
                    index
                );
            }

            index++;
            if (index >= wsvText.Length || wsvText[index] != DoubleQuoteChar)
            {
                throw new InvalidStringLineBreakException(lineNumber, trueStart - lineStart, index - lineStart, index);
            }

            index++;
            // -3 at end because we read the three characters "/"
            sb.Append(wsvText.Slice(startIndex, index - startIndex - 3));
            sb.Append('\n');
            startIndex = index;
        }

        // -1 at end because we read a double quote
        sb.Append(wsvText.Slice(startIndex, index - startIndex - 1));
        return sb.ToString();
    }

    private static string ReadValue(ReadOnlySpan<char> wsvText, int lineNumber, int lineStart, ref int index)
    {
        var trueStart = index;
        var startIndex = index;
        while (index < wsvText.Length)
        {
            if (wsvText[index] == DoubleQuoteChar)
            {
                throw new InvalidDoubleQuoteAfterValueException(
                    lineNumber,
                    trueStart - lineStart,
                    index - lineStart,
                    index
                );
            }

            if (wsvText[index] == LineBreakChar || WsChar.Contains(wsvText[index]))
            {
                break;
            }

            index++;
        }

        return new string(wsvText.Slice(startIndex, index - startIndex));
    }
}

public abstract class ParserException(int line, int start, int end, int endSpanIndex) : Exception
{
    public override string ToString() =>
        $"Failed to read Wsv file at span index {endSpanIndex} (line {line}, char {start} -> {end}): {base.ToString()}";
}

public class InvalidCharacterAfterStringException(int line, int start, int end, int endSpanIndex) : ParserException(
    line,
    start,
    end,
    endSpanIndex
);

public class InvalidDoubleQuoteAfterValueException(int line, int start, int end, int endSpanIndex) : ParserException(
    line,
    start,
    end,
    endSpanIndex
);

public class InvalidStringLineBreakException(int line, int start, int end, int endSpanIndex) : ParserException(
    line,
    start,
    end,
    endSpanIndex
);

public class StringNotClosedException(int line, int start, int end, int endSpanIndex) : ParserException(
    line,
    start,
    end,
    endSpanIndex
);