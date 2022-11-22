namespace Monkey.Interpreter.AbstractSyntaxTree;

public class MonkeyProgram : INode
{
    private readonly List<IStatement> _statements;

    public MonkeyProgram(List<IStatement> statements)
    {
        _statements = statements;
    }

    public string GetTokenLiteral()
    {
        if (_statements.Count > 0)
        {
            return _statements[0].GetTokenLiteral();
        }

        return string.Empty;
    }
}