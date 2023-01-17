namespace Monkey.Interpreter.AbstractSyntaxTree.Expressions;

public class HashLiteralExpression : Node, IExpression
{
    public Dictionary<IExpression, IExpression> Pairs { get; }

    public HashLiteralExpression(Token token, Dictionary<IExpression, IExpression> pairs) : base(token)
    {
        Pairs = pairs;
    }

    public override string ToString()
    {
        var pairs = new List<string>();
        foreach (var pair in Pairs)
        {
            pairs.Add($"{pair.Key}:{pair.Value}");
        }

        return $"{{{string.Join(", ", pairs)}}}";
    }
}