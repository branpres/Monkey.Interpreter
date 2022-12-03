namespace Monkey.Interpreter.AbstractSyntaxTree.Statements;

public class ReturnStatement : Node, IStatement
{
    public IExpression ReturnValue { get; }

    public ReturnStatement(Token token, IExpression returnValue) : base(token)
    {
        ReturnValue = returnValue;
    }

    public override string ToString()
    {
        var sb = new StringBuilder($"{GetTokenLiteral()} ");

        if (ReturnValue != null)
        {
            sb.Append(ReturnValue.ToString());
        }

        sb.Append(';');

        return sb.ToString();
    }
}