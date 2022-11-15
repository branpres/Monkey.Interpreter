namespace Monkey.Interpreter;

public static class Constants
{
    public const string ILLEGAL = "ILLEGAL";
    public const string EOF = "EOF";

    // identifiers, literals
    public const string IDENTIFIER = "IDENTIFIER";
    public const string INTEGER = "INTEGER";

    // operators
    public const string ASSIGNMENT = "=";
    public const string PLUS = "+";

    // delimiters
    public const string COMMA = ",";
    public const string SEMICOLON = ";";

    // keywords
    public const string FUNCTION = "FUNCTION";
    public const string LET = "LET";

    public const string LEFT_PARENTHESIS = "(";
    public const string RIGHT_PARENTHESIS = ")";
    public const string LEFT_BRACE = "{";
    public const string RIGHT_BRACE = "}";
}