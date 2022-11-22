using Monkey.Interpreter.AbstractSyntaxTree;

namespace Monkey.Interpreter;

public class Parser
{
    public List<string> Errors { get; } = new List<string>();

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

    public MonkeyProgram ParseProgram()
    {
        var program = new MonkeyProgram();

        while(!IsCurrentTokenExpectedType(TokenType.EOF))
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

    private void NextToken()
    {
        _currenToken = _peekToken;
        _peekToken = _lexer.GetNextToken();
    }

    private IStatement? ParseStatement()
    {
        return _currenToken.TokenType switch
        {
            TokenType.LET => ParseLetStatement(),
            _ => null,
        };
    }

    private LetStatement? ParseLetStatement()
    {   
        var token = _currenToken;

        if (!IsExpectedPeekTokenOfType(TokenType.IDENTIFIER))
        {
            return null;
        }

        var name = new IdentifierExpression(_currenToken, _currenToken.Literal);

        if (!IsExpectedPeekTokenOfType(TokenType.ASSIGNMENT))
        {
            return null;
        }

        while (!IsCurrentTokenExpectedType(TokenType.SEMICOLON))
        {
            NextToken();
        }

        return new LetStatement(token, name, null);
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

        Errors.Add(string.Format("Expected next token to be {0}. Got {1} instead.", tokenType, _peekToken.TokenType));
        return false;
    }
}