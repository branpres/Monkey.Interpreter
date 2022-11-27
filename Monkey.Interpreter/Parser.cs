using Monkey.Interpreter.AbstractSyntaxTree;

namespace Monkey.Interpreter;

public class Parser
{
    public List<string> Errors { get; } = new List<string>();

    private readonly Lexer _lexer;

    private readonly Dictionary<TokenType, Func<IExpression>> _prefixParseFunctions = new();

    private readonly Dictionary<TokenType, Func<IExpression>> _infixParseFunctions = new();

    private Token _currenToken;

    private Token _peekToken;

    public Parser(Lexer lexer)
    {
        _lexer = lexer;

        // initialize _currentToken and _peekToken
        NextToken();
        NextToken();

        _prefixParseFunctions.Add(TokenType.IDENTIFIER, ParseIdentifier);
    }

    public MonkeyProgram ParseProgram()
    {
        var program = new MonkeyProgram();

        while(!IsCurrentToken(TokenType.EOF))
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
            TokenType.RETURN => ParseReturnStatement(),
            _ => ParseExpressionStatement(),
        };
    }

    private LetStatement? ParseLetStatement()
    {   
        var token = _currenToken;

        if (!IsExpectedPeekTokenOf(TokenType.IDENTIFIER))
        {
            return null;
        }

        var name = new IdentifierExpression(_currenToken, _currenToken.Literal);

        if (!IsExpectedPeekTokenOf(TokenType.ASSIGNMENT))
        {
            return null;
        }

        while (!IsCurrentToken(TokenType.SEMICOLON))
        {
            NextToken();
        }

        return new LetStatement(token, name, null);
    }

    private ReturnStatement? ParseReturnStatement()
    {
        var token = _currenToken;

        while (!IsCurrentToken(TokenType.SEMICOLON))
        {
            NextToken();
        }

        return new ReturnStatement(token, null);
    }

    private ExpressionStatement ParseExpressionStatement()
    {
        var expression = ParseExpression(Precedence.LOWEST);

        var expressionStatement = new ExpressionStatement(_currenToken, expression);

        if (IsPeekToken(TokenType.SEMICOLON))
        {
            NextToken();
        }

        return expressionStatement;
    }

    private IExpression? ParseExpression(Precedence precedence)
    {
        if (_prefixParseFunctions.ContainsKey(_currenToken.TokenType))
        {
            var prefixFunction = _prefixParseFunctions[_currenToken.TokenType];
            return prefixFunction.Invoke();
        }
        
        return null;
    }

    private IExpression ParseIdentifier()
    {
        return new IdentifierExpression(_currenToken, _currenToken.Literal);
    }

    private bool IsCurrentToken(TokenType expectedTokenType)
    {
        return _currenToken.TokenType == expectedTokenType;
    }

    private bool IsPeekToken(TokenType expectedTokenType)
    {
        return _peekToken.TokenType == expectedTokenType;
    }

    private bool IsExpectedPeekTokenOf(TokenType tokenType)
    {
        if (IsPeekToken(tokenType))
        {
            NextToken();
            return true;
        }

        Errors.Add(string.Format("Expected next token to be {0}. Got {1} instead.", tokenType, _peekToken.TokenType));
        return false;
    }
}