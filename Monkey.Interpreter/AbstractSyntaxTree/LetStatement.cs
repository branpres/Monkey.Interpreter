namespace Monkey.Interpreter.AbstractSyntaxTree;

public class LetStatement : Node, IStatement
{
    public IdentifierExpression Name { get; }

    public IExpression Value { get; }

    public LetStatement(Token token, IdentifierExpression name, IExpression value) : base(token)
    {
        Name = name;
        Value = value;
    }

    public override string ToString()
    {
        var sb = new StringBuilder($"{GetTokenLiteral()} {Name} = ");

        if (Value != null)
        {
            sb.Append(Value.ToString());
        }

        sb.Append(';');

        return sb.ToString();
    }
}