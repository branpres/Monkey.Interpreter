namespace Monkey.Interpreter.AbstractSyntaxTree;

public class MonkeyProgram : INode
{
    private readonly List<IStatement> _statements = new();

    public List<IStatement> Statements() => _statements;

    public void AddStatement(IStatement statement) => _statements.Add(statement);

    public string GetTokenLiteral()
    {
        if (_statements.Count > 0)
        {
            return _statements[0].GetTokenLiteral();
        }

        return string.Empty;
    }
}