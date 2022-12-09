namespace Monkey.Interpreter.AbstractSyntaxTree.Statements;

public class BlockStatement : Node, IStatement
{
    public List<IStatement> Statements { get; } = new();

    public void AddStatement(IStatement statement) => Statements.Add(statement);

    public BlockStatement(Token token) : base(token) { }

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