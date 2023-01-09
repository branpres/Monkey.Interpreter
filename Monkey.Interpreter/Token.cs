namespace Monkey.Interpreter;

public enum TokenType
{
    ILLEGAL,
    EOF,

    // identifiers, literals
    IDENTIFIER,
    INTEGER,
    STRING,

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
    RIGHT_BRACE,
    LEFT_BRACKET,
    RIGHT_BRACKET
}

public record Token
{
    public Token(TokenType tokenType, string literal)
    {
        TokenType = tokenType;
        Literal = literal;
    }

    public TokenType TokenType { get; init; }
    
    public string Literal { get; init; }

    public override string ToString()
    {
        return $"{{TokenType:{TokenType} Literal:{Literal}}}";
    }
}