using Monkey.Interpreter.AbstractSyntaxTree;

namespace Monkey.Interpreter;

public class Parser
{
    private readonly Lexer _lexer;

    private Token _currenToken;

    private Token _peekToken;

    public Parser(Lexer lexer)
    {
        _lexer = lexer;

        // initialize _currentToken and _peekToken
        NextToken();
        NextToken();
    }

    public void NextToken()
    {
        _currenToken = _peekToken;
        _peekToken = _lexer.GetNextToken();
    }

    public MonkeyProgram ParseProgram()
    {
        return null;
    }
}