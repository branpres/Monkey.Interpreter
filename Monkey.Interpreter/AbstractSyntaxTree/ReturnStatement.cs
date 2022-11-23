namespace Monkey.Interpreter.AbstractSyntaxTree;

public class ReturnStatement : IStatement
{
    public Token Token { get; }

    public IExpression ReturnValue { get; }

    public ReturnStatement(Token token, IExpression returnValue)
    {
        Token = token;
        ReturnValue = returnValue;
    }

    public string GetTokenLiteral()
    {
        return Token.Literal;
    }
}
