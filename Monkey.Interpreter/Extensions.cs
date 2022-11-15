namespace Monkey.Interpreter;

internal static class Extensions
{
    internal static bool IsLetter(this char ch)
    {
        return 'a' <= ch && ch <= 'z' || 'A' <= ch && ch <= 'Z' || ch == '_';
    }

    internal static string GetIdentifier(this string identifier)
    {
        if (identifier == Constants.FUNCTION_KEYWORD)
        {
            return Constants.FUNCTION;
        }

        if (identifier == Constants.LET_KEYWORD)
        {
            return Constants.LET;
        }

        return Constants.IDENTIFIER;
    }

    internal static bool IsWhitespace(this char ch)
    {
        return ch == ' ' || ch == '\t' || ch == '\n' || ch == '\r';
    }
}