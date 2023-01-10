namespace Monkey.Interpreter.AbstractSyntaxTree.Expressions;

public class IndexExpression : Node, IExpression
{
    public IExpression Left { get; }

    public IExpression Index { get; }

    public IndexExpression(Token token, IExpression left, IExpression index) : base(token)
    {
        Left = left;
        Index = index;
    }

    public override string ToString()
    {
        return $"({Left.ToString()}[{Index.ToString()}])";
    }
}