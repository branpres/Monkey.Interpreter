namespace Monkey.Interpreter;

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
        return $"TokenType:{TokenType.Value} Literal:{Literal}";
    }
}