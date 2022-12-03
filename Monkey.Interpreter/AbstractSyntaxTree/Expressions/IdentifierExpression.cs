namespace Monkey.Interpreter.AbstractSyntaxTree.Expressions;

public class IdentifierExpression : Node, IExpression
{
    public string Value { get; }

    public IdentifierExpression(Token token, string value) : base(token)
    {
        Value = value;
    }

    public override string ToString()
    {
        return Value;
    }
}