namespace Monkey.Interpreter;

public enum Precedence
{
    _ = 0,
    LOWEST = 1,
    EQUALS = 2,             // ==     
    LESS_OR_GREATER = 3,    // < or >
    SUM = 4,                // + or -
    PRODUCT = 5,            // * or /
    PREFIX = 6,             // -x or !x
    CALL = 7,               // myFunction(x)
    INDEX = 8               // array[index]
}

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
        { TokenType.ASTERISK, Precedence.PRODUCT },
        { TokenType.LEFT_PARENTHESIS, Precedence.CALL },
        { TokenType.LEFT_BRACKET, Precedence.INDEX }
    };

    private Token _currenToken;

    private Token _peekToken;

    public Parser(Lexer lexer)
    {
        _lexer = lexer;

        // initialize _currentToken and _peekToken
        _currenToken = _lexer.GetNextToken();
        _peekToken = _lexer.GetNextToken();

        _prefixParseFunctions.Add(TokenType.IDENTIFIER, ParseIdentifier);
        _prefixParseFunctions.Add(TokenType.INTEGER, ParseIntegerLiteral);
        _prefixParseFunctions.Add(TokenType.BANG, ParsePrefix);
        _prefixParseFunctions.Add(TokenType.MINUS, ParsePrefix);
        _prefixParseFunctions.Add(TokenType.TRUE, ParseBoolean);
        _prefixParseFunctions.Add(TokenType.FALSE, ParseBoolean);
        _prefixParseFunctions.Add(TokenType.LEFT_PARENTHESIS, ParseGroup);
        _prefixParseFunctions.Add(TokenType.IF, ParseIf);
        _prefixParseFunctions.Add(TokenType.FUNCTION, ParseFunctionLiteral);
        _prefixParseFunctions.Add(TokenType.STRING, ParseStringLiteral);
        _prefixParseFunctions.Add(TokenType.LEFT_BRACKET, ParseArrayLiteral);
        _prefixParseFunctions.Add(TokenType.LEFT_BRACE, ParseHashLiteral);

        _infixParseFunctions.Add(TokenType.PLUS, ParseInfix);
        _infixParseFunctions.Add(TokenType.MINUS, ParseInfix);
        _infixParseFunctions.Add(TokenType.SLASH, ParseInfix);
        _infixParseFunctions.Add(TokenType.ASTERISK, ParseInfix);
        _infixParseFunctions.Add(TokenType.EQUAL, ParseInfix);
        _infixParseFunctions.Add(TokenType.NOT_EQUAL, ParseInfix);
        _infixParseFunctions.Add(TokenType.LESS_THAN, ParseInfix);
        _infixParseFunctions.Add(TokenType.GREATER_THAN, ParseInfix);
        _infixParseFunctions.Add(TokenType.LEFT_PARENTHESIS, ParseCallExpression);
        _infixParseFunctions.Add(TokenType.LEFT_BRACKET, ParseIndexExpression);
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

        if (!IsExpectedPeekToken(TokenType.IDENTIFIER))
        {
            return null;
        }

        var name = new IdentifierExpression(_currenToken, _currenToken.Literal);

        if (!IsExpectedPeekToken(TokenType.ASSIGNMENT))
        {
            return null;
        }

        NextToken();

        var value = ParseExpression(Precedence.LOWEST);

        if (IsPeekToken(TokenType.SEMICOLON))
        {
            NextToken();
        }

        if (value != null)
        {
            return new LetStatement(token, name, value);
        }

        return null;
    }

    private ReturnStatement? ParseReturnStatement()
    {
        var token = _currenToken;

        NextToken();

        var returnValue = ParseExpression(Precedence.LOWEST);

        if (!IsPeekToken(TokenType.SEMICOLON))
        {
            NextToken();
        }

        if (returnValue != null)
        {
            return new ReturnStatement(token, returnValue);
        }

        return null;
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

    private IExpression? ParseStringLiteral()
    {
        return new StringLiteralExpression(_currenToken, _currenToken.Literal);
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

    private IExpression? ParseBoolean()
    {
        return new BooleanExpression(_currenToken, IsCurrentToken(TokenType.TRUE));
    }

    private IExpression? ParseGroup()
    {
        NextToken();

        var expression = ParseExpression(Precedence.LOWEST);

        if (!IsExpectedPeekToken(TokenType.RIGHT_PARENTHESIS))
        {
            return null;
        }

        return expression;
    }

    private IExpression? ParseIf()
    {
        var token = _currenToken;

        if (!IsExpectedPeekToken(TokenType.LEFT_PARENTHESIS))
        {
            return null;
        }

        NextToken();

        var conditionExpression = ParseExpression(Precedence.LOWEST);
        if (conditionExpression == null)
        {
            return null;
        }

        if (!IsExpectedPeekToken(TokenType.RIGHT_PARENTHESIS) && !IsExpectedPeekToken(TokenType.LEFT_BRACE))
        {
            return null;
        }

        var consequence = ParseBlockStatement();

        BlockStatement? alternative = null;
        if (IsPeekToken(TokenType.ELSE))
        {
            NextToken();

            if (!IsExpectedPeekToken(TokenType.LEFT_BRACE))
            {
                return null;
            }

            alternative = ParseBlockStatement();
        }

        return new IfExpression(token, conditionExpression, consequence, alternative);
    }

    private BlockStatement ParseBlockStatement()
    {
        var blockStatement = new BlockStatement(_currenToken);

        while(!IsCurrentToken(TokenType.RIGHT_BRACE) && !IsCurrentToken(TokenType.EOF))
        {
            var statement = ParseStatement();
            if (statement != null)
            {
                blockStatement.Statements.Add(statement);
            }

            NextToken();
        }

        return blockStatement;
    }

    private IExpression? ParseFunctionLiteral()
    {
        var token = _currenToken;

        if (!IsExpectedPeekToken(TokenType.LEFT_PARENTHESIS))
        {
            return null;
        }

        var parameters = ParseFunctionParameters();

        if (!IsExpectedPeekToken(TokenType.LEFT_BRACE))
        {
            return null;
        }

        var body = ParseBlockStatement();

        var functionLiteralExpression = new FunctionLiteralExpression(token, body);
        functionLiteralExpression.Parameters.AddRange(parameters);

        return functionLiteralExpression;
    }

    private List<IdentifierExpression> ParseFunctionParameters()
    {
        var parameters = new List<IdentifierExpression>();

        if (IsPeekToken(TokenType.RIGHT_PARENTHESIS))
        {
            NextToken();
            return parameters;
        }

        NextToken();

        parameters.Add(new IdentifierExpression(_currenToken, _currenToken.Literal));

        while(IsPeekToken(TokenType.COMMA))
        {
            NextToken();
            NextToken();
            parameters.Add(new IdentifierExpression(_currenToken, _currenToken.Literal));
        }

        if (!IsExpectedPeekToken(TokenType.RIGHT_PARENTHESIS))
        {
            return new List<IdentifierExpression>();
        }

        return parameters;
    }

    private IExpression? ParseCallExpression(IExpression function)
    {
        var token = _currenToken;
        var arguments = ParseExpressions(TokenType.RIGHT_PARENTHESIS);

        return new CallExpression(token, function, arguments);
    }

    private IExpression? ParseArrayLiteral()
    {
        var token = _currenToken;

        return new ArrayLiteralExpression(token, ParseExpressions(TokenType.RIGHT_BRACKET));
    }

    private IExpression? ParseIndexExpression(IExpression left)
    {
        var token = _currenToken;

        NextToken();

        var index = ParseExpression(Precedence.LOWEST);

        if (!IsExpectedPeekToken(TokenType.RIGHT_BRACKET))
        {
            return null;
        }

        if (index != null)
        { 
            return new IndexExpression(token, left, index);
        }

        return null;
    }

    private List<IExpression> ParseExpressions(TokenType end)
    {
        var expressions = new List<IExpression>();

        if (IsPeekToken(end))
        {
            NextToken();

            return expressions;
        }

        NextToken();

        var expression = ParseExpression(Precedence.LOWEST);
        if (expression != null)
        {
            expressions.Add(expression);
        }

        while (IsPeekToken(TokenType.COMMA))
        {
            NextToken();
            NextToken();

            expression = ParseExpression(Precedence.LOWEST);
            if (expression != null)
            {
                expressions.Add(expression);
            }
        }

        if (!IsExpectedPeekToken(end))
        {
            return new List<IExpression>();
        }

        return expressions;
    }

    private IExpression? ParseHashLiteral()
    {
        var hash = new Dictionary<IExpression, IExpression>();

        var token = _currenToken;        

        while(!IsPeekToken(TokenType.RIGHT_BRACE))
        {
            NextToken();

            var key = ParseExpression(Precedence.LOWEST);

            if (!IsExpectedPeekToken(TokenType.COLON))
            {
                return null;
            }

            NextToken();
            var value = ParseExpression(Precedence.LOWEST);

            if (key != null && value != null)
            {
                hash.Add(key, value);
            }
            
            if (!IsPeekToken(TokenType.RIGHT_BRACE) && !IsExpectedPeekToken(TokenType.COMMA))
            {
                return null;
            }
        }

        if (!IsExpectedPeekToken(TokenType.RIGHT_BRACE))
        {
            return null;
        }

        return new HashLiteralExpression(token, hash);
    }

    private bool IsCurrentToken(TokenType expectedTokenType)
    {
        return _currenToken.TokenType == expectedTokenType;
    }

    private bool IsPeekToken(TokenType expectedTokenType)
    {
        return _peekToken.TokenType == expectedTokenType;
    }

    private bool IsExpectedPeekToken(TokenType tokenType)
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