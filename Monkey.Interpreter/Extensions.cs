namespace Monkey.Interpreter;

internal static class Extensions
{
    internal static bool IsLetter(this char ch)
    {
        return 'a' <= ch && ch <= 'z' || 'A' <= ch && ch <= 'Z' || ch == '_';
    }

    internal static bool IsDigit(this char ch)
    {
        return '0' <= ch && ch <= '9';
    }

    internal static string GetIdentifier(this string identifier)
    {
        var keywords = new Dictionary<string, string>
        {
            { Constants.FUNCTION_KEYWORD, Constants.FUNCTION },
            { Constants.LET_KEYWORD, Constants.LET },
            { Constants.TRUE_KEYWORD, Constants.TRUE },
            { Constants.FALSE_KEYWORD, Constants.FALSE },
            { Constants.IF_KEYWORD, Constants.IF },
            { Constants.ELSE_KEYWORD, Constants.ELSE },
            { Constants.RETURN_KEYWORD, Constants.RETURN }
        };

        if (keywords.TryGetValue(identifier, out var value))
        {
            return value;
        }

        return Constants.IDENTIFIER;
    }

    internal static bool IsWhitespace(this char ch)
    {
        return ch == ' ' || ch == '\t' || ch == '\n' || ch == '\r';
    }
}