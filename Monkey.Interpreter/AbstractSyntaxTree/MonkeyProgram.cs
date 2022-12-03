namespace Monkey.Interpreter.AbstractSyntaxTree;

public class MonkeyProgram : INode
{
    public List<IStatement> Statements { get; } = new();

    public void AddStatement(IStatement statement) => Statements.Add(statement);

    public string GetTokenLiteral()
    {
        if (Statements.Count > 0)
        {
            return Statements[0].GetTokenLiteral();
        }

        return string.Empty;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        foreach (var statement in Statements)
        {
            sb.Append(statement.ToString());
        }

        return sb.ToString();
    }
}