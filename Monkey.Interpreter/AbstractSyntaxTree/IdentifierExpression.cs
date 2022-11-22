namespace Monkey.Interpreter.AbstractSyntaxTree;

public class IdentifierExpression : IExpression
{
    public Token Token { get; }

    public string Value { get; }

    public IdentifierExpression(Token token, string value)
    {
        Token = token;
        Value = value;
    }

    public string GetTokenLiteral()
    {
        return Token.Literal;
    }
}