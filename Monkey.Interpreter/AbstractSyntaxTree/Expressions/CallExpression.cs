namespace Monkey.Interpreter.AbstractSyntaxTree.Expressions;

public class CallExpression : Node, IExpression
{
    public IExpression Function { get; }

    public List<IExpression> Arguments { get; }

    public CallExpression(Token token, IExpression function, List<IExpression> arguments) : base(token)
    {
        Function = function;
        Arguments = arguments;
    }

    public override string ToString()
    {
        return $"{Function}({string.Join(", ", Arguments.Select(x => x.ToString()))})";
    }
}