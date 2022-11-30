﻿using Monkey.Interpreter.AbstractSyntaxTree;

namespace Monkey.Interpreter;

public class Parser
{
    public List<string> Errors { get; } = new List<string>();

    private readonly Lexer _lexer;

    private readonly Dictionary<TokenType, Func<IExpression?>> _prefixParseFunctions = new();

    private readonly Dictionary<TokenType, Func<IExpression?>> _infixParseFunctions = new();

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
        if (_prefixParseFunctions.TryGetValue(_currenToken.TokenType, out var value))
        {
            var prefixFunction = value;
            return prefixFunction.Invoke();
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

        return new PrefixExpression(token, prefixOperator, right);
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
}