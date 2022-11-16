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
    public const string MINUS = "-";
    public const string BANG = "!";
    public const string ASTERISK = "*";
    public const string SLASH = "/";
    public const string LESS_THAN = "<";
    public const string GREATER_THAN = ">";
    public const string EQUAL = "==";
    public const string NOT_EQUAL = "!=";

    // delimiters
    public const string COMMA = ",";
    public const string SEMICOLON = ";";

    // keywords
    public const string FUNCTION = "FUNCTION";
    public const string FUNCTION_KEYWORD = "fn";
    public const string LET = "LET";
    public const string LET_KEYWORD = "let";
    public const string TRUE = "TRUE";
    public const string TRUE_KEYWORD = "true";
    public const string FALSE = "FALSE";
    public const string FALSE_KEYWORD = "false";
    public const string IF = "IF";
    public const string IF_KEYWORD = "if";
    public const string ELSE = "ELSE";
    public const string ELSE_KEYWORD = "else";
    public const string RETURN = "RETURN";
    public const string RETURN_KEYWORD = "return";

    public const string LEFT_PARENTHESIS = "(";
    public const string RIGHT_PARENTHESIS = ")";
    public const string LEFT_BRACE = "{";
    public const string RIGHT_BRACE = "}";
}