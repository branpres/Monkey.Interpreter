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
        var program = new MonkeyProgram();

        while(_currenToken.TokenType != TokenType.EOF)
        {
            var statement = ParseStatement();
            if (statement != null)
            {
                program.AddStatement(statement);
            }

            NextToken();
        }

        return program;
    }

    private IStatement? ParseStatement()
    {
        switch (_currenToken.TokenType)
        {
            case TokenType.LET:
                return ParseLetStatement();
            default:
                return null;
        }
    }

    private LetStatement? ParseLetStatement()
    {   
        var token = _currenToken;

        if (!IsExpectedPeekTokenOfType(TokenType.IDENTIFIER))
        {
            return null;
        }

        var statementName = new IdentifierExpression(_currenToken, _currenToken.Literal);

        if (!IsExpectedPeekTokenOfType(TokenType.ASSIGNMENT))
        {
            return null;
        }

        while (!IsCurrentTokenExpectedType(TokenType.SEMICOLON))
        {
            NextToken();
        }

        return new LetStatement(token, statementName, null);
    }

    private bool IsCurrentTokenExpectedType(TokenType tokenType)
    {
        return _currenToken.TokenType == tokenType;
    }

    private bool IsPeekTokenExpectedType(TokenType tokenType)
    {
        return _peekToken.TokenType == tokenType;
    }

    private bool IsExpectedPeekTokenOfType(TokenType tokenType)
    {
        if (IsPeekTokenExpectedType(tokenType))
        {
            NextToken();
            return true;
        }

        return false;
    }
}