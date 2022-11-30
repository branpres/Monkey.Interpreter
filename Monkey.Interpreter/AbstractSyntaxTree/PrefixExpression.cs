namespace Monkey.Interpreter.AbstractSyntaxTree;

public class PrefixExpression : Node, IExpression
{
    public string Operator { get; }

    public IExpression Right { get; }

    public PrefixExpression(Token token, string @operator, IExpression right) : base(token)
    {
        Operator = @operator;
        Right = right;
    }

    public override string ToString()
    {
        return $"({Operator}{Right.ToString()})";
    }
}