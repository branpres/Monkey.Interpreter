namespace Monkey.Interpreter.AbstractSyntaxTree.Expressions;

public class FunctionLiteralExpression : Node, IExpression
{
    public List<IdentifierExpression> Parameters { get; } = new();

    public BlockStatement Body { get; }

    public FunctionLiteralExpression(Token token, BlockStatement body) : base(token)
    {
        Body = body;
    }

    public override string ToString()
    {
        return $"{GetTokenLiteral()}({string.Join(", ", Parameters.Select(x => x.ToString()))}){Body}";
    }
}