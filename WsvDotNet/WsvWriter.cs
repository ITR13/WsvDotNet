using System.Text;

namespace WsvNet;

public static class WsvWriter
{
    private const char WsChar = ' ';
    private const char LineBreakChar = '\u000A';
    private const char HashChar = '#';
    private const char DoubleQuoteChar = '\u0022';
    private const char MinusChar = '-';
    private const char SlashChar = '/';

    public static string WriteTsv(string?[][] values)
    {
        ArgumentNullException.ThrowIfNull(values);

        var sb = new StringBuilder();
        for (var i = 0; i < values.Length; i++)
        {
            ArgumentNullException.ThrowIfNull(values[i]);

            var formattedValues = FormatLine(values[i]);

            sb.AppendJoin(WsChar, formattedValues);
            sb.Append(LineBreakChar);
        }

        if (values.Length > 0)
        {
            sb.Length -= 1;
        }

        return sb.ToString();
    }

    private static string[] FormatLine(string?[] values)
    {
        var formattedValues = new string[values.Length];
        for (var i = 0; i < values.Length; i++)
        {
            if (values[i] == null)
            {
                formattedValues[i] = "-";
                continue;
            }

            // Rider is incorrectly marking this as possibly null
            if (values[i]!.Length == 0)
            {
                formattedValues[i] = "\"\"";
                continue;
            }

            var span = values[i].AsSpan();
            formattedValues[i] = SanitizeSpan(span);
        }

        return formattedValues;
    }

    private static string SanitizeSpan(ReadOnlySpan<char> span)
    {
        var sb = new StringBuilder();
        var startIndex = 0;
        var quoted = false;
        for (var i = 0; i < span.Length; i++)
        {
            switch (span[i])
            {
                case DoubleQuoteChar:
                    quoted = true;
                    sb.Append(span.Slice(startIndex, i - startIndex));
                    startIndex = i + 1;
                    sb.Append(DoubleQuoteChar);
                    sb.Append(DoubleQuoteChar);
                    break;
                case LineBreakChar:
                    quoted = true;
                    sb.Append(span.Slice(startIndex, i - startIndex));
                    startIndex = i + 1;
                    sb.Append(DoubleQuoteChar);
                    sb.Append(SlashChar);
                    sb.Append(DoubleQuoteChar);
                    break;
                case HashChar:
                case WsChar:
                case MinusChar:
                    quoted = true;
                    break;
            }
        }

        sb.Append(span[startIndex..]);

        if (!quoted) return sb.ToString();

        sb.Insert(0, DoubleQuoteChar);
        sb.Append(DoubleQuoteChar);

        return sb.ToString();
    }
}