namespace Monkey.Interpreter.AbstractSyntaxTree;

public class LetStatement : IStatement
{
    private readonly Token _token;

    private IdentifierExpression _identifier;

    private IExpression _value;

    public LetStatement(Token token, IdentifierExpression identifier, IExpression value)
    {
        _token = token;
        _identifier = identifier;
        _value = value;
    }

    public string GetTokenLiteral()
    {
        return _token.Literal;
    }
}