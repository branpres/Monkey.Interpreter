namespace Monkey.Interpreter;

public enum TokenType
{
    ILLEGAL,
    EOF,

    // identifiers, literals
    IDENTIFIER,
    INTEGER,

    // operators
    ASSIGNMENT,
    PLUS,
    MINUS,
    BANG,
    ASTERISK,
    SLASH,
    LESS_THAN,
    GREATER_THAN,
    EQUAL,
    NOT_EQUAL,

    // delimiters
    COMMA,
    SEMICOLON,

    // keywords
    FUNCTION,
    LET,
    TRUE,
    FALSE,
    IF,
    ELSE,
    RETURN,

    // enclosures
    LEFT_PARENTHESIS,
    RIGHT_PARENTHESIS,
    LEFT_BRACE,
    RIGHT_BRACE
}