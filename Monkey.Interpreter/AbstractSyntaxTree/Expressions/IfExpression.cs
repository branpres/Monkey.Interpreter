namespace Monkey.Interpreter.AbstractSyntaxTree.Expressions;

public class IfExpression : Node, IExpression
{
    public IExpression Condition { get; }

    public BlockStatement Consequence { get; }

    public BlockStatement? Alternative { get; }

    public IfExpression(Token token, IExpression condition, BlockStatement consequence, BlockStatement? alternative) : base(token)
    {
        Condition = condition;
        Consequence = consequence;
        Alternative = alternative;
    }

    public override string ToString()
    {
        var sb = new StringBuilder($"if{Condition} {Consequence}");

        if (Alternative != null)
        {
            sb.Append($"else {Alternative}");
        }

        return sb.ToString();
    }
}