namespace Monkey.Interpreter.AbstractSyntaxTree.Expressions;

public class ArrayLiteralExpression : Node, IExpression
{
    public List<IExpression> Elements { get; }

    public ArrayLiteralExpression(Token token, List<IExpression> elements) : base(token)
    {
        Elements = elements;
    }

    public override string ToString()
    {
        return $"[{string.Join(", ", Elements.Select(x => x.ToString()))}]";
    }
}