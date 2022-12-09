namespace Monkey.Interpreter.AbstractSyntaxTree.Expressions;

public class BooleanExpression : Node, IExpression
{
    public bool Value { get; }

    public BooleanExpression(Token token, bool value) : base(token)
    {
        Value = value;
    }

    public override string ToString() => Token.Literal;
}