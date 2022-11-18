namespace Monkey.Interpreter;

public static class Constants
{
    public static class Operator
    {
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
    }

    public static class Delimiter
    {
        public const string COMMA = ",";
        public const string SEMICOLON = ";";
    }

    public static class Keyword
    {
        public const string FUNCTION = "fn";
        public const string LET = "let";
        public const string TRUE = "true";
        public const string FALSE = "false";
        public const string IF = "if";
        public const string ELSE = "else";
        public const string RETURN = "return";
    }

    public static class Enclosure
    {
        public const string LEFT_PARENTHESIS = "(";
        public const string RIGHT_PARENTHESIS = ")";
        public const string LEFT_BRACE = "{";
        public const string RIGHT_BRACE = "}";
    }
}