namespace Monkey.Interpreter.AbstractSyntaxTree;

public class BooleanExpression : Node, IExpression
{
    public bool Value { get; }

    public BooleanExpression(Token token, bool value) : base(token)
    {
        Value = value;
    }

    public override string ToString()
    {
        return Token.Literal;
    }
}