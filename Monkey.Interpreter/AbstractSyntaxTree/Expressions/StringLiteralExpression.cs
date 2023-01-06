namespace Monkey.Interpreter.AbstractSyntaxTree.Expressions;

public class StringLiteralExpression : Node, IExpression
{
    public string Value { get; }

    public StringLiteralExpression(Token token, string value) : base(token)
    {
        Value = value;
    }

    public override string ToString() => Token.Literal;
}