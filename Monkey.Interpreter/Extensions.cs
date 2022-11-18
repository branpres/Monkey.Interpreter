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

    internal static TokenType GetIdentifier(this string identifier)
    {
        var keywords = new Dictionary<string, TokenType>
        {
            { Constants.Keyword.FUNCTION, TokenType.FUNCTION },
            { Constants.Keyword.LET, TokenType.LET },
            { Constants.Keyword.TRUE, TokenType.TRUE },
            { Constants.Keyword.FALSE, TokenType.FALSE },
            { Constants.Keyword.IF, TokenType.IF },
            { Constants.Keyword.ELSE, TokenType.ELSE },
            { Constants.Keyword.RETURN, TokenType.RETURN }
        };

        if (keywords.TryGetValue(identifier, out var value))
        {
            return value;
        }

        return TokenType.IDENTIFIER;
    }

    internal static bool IsWhitespace(this char ch)
    {
        return ch == ' ' || ch == '\t' || ch == '\n' || ch == '\r';
    }
}