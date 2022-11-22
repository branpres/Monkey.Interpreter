namespace Monkey.Interpreter.AbstractSyntaxTree;

public class IdentifierExpression : IExpression
{
    private readonly Token _token;

    private readonly string _value;

    public IdentifierExpression(Token token, string value)
    {
        _token = token;
        _value = value;
    }

    public string GetTokenLiteral()
    {
        return _token.Literal;
    }
}