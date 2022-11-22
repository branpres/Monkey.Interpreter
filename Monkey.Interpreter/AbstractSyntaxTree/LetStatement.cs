namespace Monkey.Interpreter.AbstractSyntaxTree;

public class LetStatement : IStatement
{
    public Token Token { get; }

    public IdentifierExpression Name { get; }

    public IExpression Value { get; }

    public LetStatement(Token token, IdentifierExpression name, IExpression value)
    {
        Token = token;
        Name = name;
        Value = value;
    }

    public string GetTokenLiteral()
    {
        return Token.Literal;
    }
}