using Monkey.Interpreter.AbstractSyntaxTree;

namespace Monkey.Interpreter;

public class Parser
{
    public List<string> Errors { get; } = new List<string>();

    private readonly Lexer _lexer;

    private readonly Dictionary<TokenType, Func<IExpression?>> _prefixParseFunctions = new();

    private readonly Dictionary<TokenType, Func<IExpression, IExpression?>> _infixParseFunctions = new();

    private readonly Dictionary<TokenType, Precedence> _precedences = new()
    {
        { TokenType.EQUAL, Precedence.EQUALS },
        { TokenType.NOT_EQUAL, Precedence.EQUALS },
        { TokenType.LESS_THAN, Precedence.LESS_OR_GREATER },
        { TokenType.GREATER_THAN, Precedence.LESS_OR_GREATER },
        { TokenType.PLUS, Precedence.SUM },
        { TokenType.MINUS, Precedence.SUM },
        { TokenType.SLASH, Precedence.PRODUCT },
        { TokenType.ASTERISK, Precedence.PRODUCT }
    };

    private Token _currenToken;

    private Token _peekToken;

    public Parser(Lexer lexer)
    {
        _lexer = lexer;

        // initialize _currentToken and _peekToken
        NextToken();
        NextToken();

        _prefixParseFunctions.Add(TokenType.IDENTIFIER, ParseIdentifier);
        _prefixParseFunctions.Add(TokenType.INTEGER, ParseIntegerLiteral);
        _prefixParseFunctions.Add(TokenType.BANG, ParsePrefix);
        _prefixParseFunctions.Add(TokenType.MINUS, ParsePrefix);

        _infixParseFunctions.Add(TokenType.PLUS, ParseInfix);
        _infixParseFunctions.Add(TokenType.MINUS, ParseInfix);
        _infixParseFunctions.Add(TokenType.SLASH, ParseInfix);
        _infixParseFunctions.Add(TokenType.ASTERISK, ParseInfix);
        _infixParseFunctions.Add(TokenType.EQUAL, ParseInfix);
        _infixParseFunctions.Add(TokenType.NOT_EQUAL, ParseInfix);
        _infixParseFunctions.Add(TokenType.LESS_THAN, ParseInfix);
        _infixParseFunctions.Add(TokenType.GREATER_THAN, ParseInfix);
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

    private ExpressionStatement? ParseExpressionStatement()
    {
        var expression = ParseExpression(Precedence.LOWEST);

        ExpressionStatement? expressionStatement = null;
        if (expression != null)
        {
            expressionStatement = new ExpressionStatement(_currenToken, expression);
        }

        if (IsPeekToken(TokenType.SEMICOLON))
        {
            NextToken();
        }

        return expressionStatement;
    }

    private IExpression? ParseExpression(Precedence precedence)
    {
        if (_prefixParseFunctions.TryGetValue(_currenToken.TokenType, out var prefixParseFunction))
        {
            var left = prefixParseFunction.Invoke();

            while(!IsPeekToken(TokenType.SEMICOLON) && precedence < PeekPrecedence())
            {
                if (!_infixParseFunctions.TryGetValue(_peekToken.TokenType, out var infixParseFunction))
                {
                    return left;
                }

                NextToken();

                if (left != null)
                {
                    left = infixParseFunction(left);
                }
            }

            return left;
        }
        
        return null;
    }

    private IExpression ParseIdentifier()
    {
        return new IdentifierExpression(_currenToken, _currenToken.Literal);
    }

    private IExpression? ParseIntegerLiteral()
    {
        if (!int.TryParse(_currenToken.Literal, out var integerLiteral))
        {
            Errors.Add($"Could not parse {_currenToken.Literal} as integer.");
            return null;
        }

        return new IntegerLiteralExpression(_currenToken, integerLiteral);
    }

    private IExpression? ParsePrefix()
    {
        var token = _currenToken;
        var prefixOperator = _currenToken.Literal;

        NextToken();

        var right = ParseExpression(Precedence.PREFIX);

        if (right != null)
        {
            return new PrefixExpression(token, prefixOperator, right);
        }

        return null;
    }

    private IExpression? ParseInfix(IExpression left)
    {
        var token = _currenToken;
        var infixOperator = _currenToken.Literal;

        var precedence = CurrentPrecedence();

        NextToken();

        var right = ParseExpression(precedence);

        if (right != null)
        {
            return new InfixExpression(token, left, infixOperator, right);
        }

        return null;
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

        Errors.Add($"Expected next token to be {tokenType}. Got {_peekToken.TokenType} instead.");
        return false;
    }

    private Precedence CurrentPrecedence()
    {
        return GetPrecedence(_currenToken);
    }

    private Precedence PeekPrecedence()
    {
        return GetPrecedence(_peekToken);
    }

    private Precedence GetPrecedence(Token token)
    {
        if (_precedences.TryGetValue(token.TokenType, out var precedence))
        {
            return precedence;
        }

        return Precedence.LOWEST;
    }
}