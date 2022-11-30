﻿using Microsoft.VisualStudio.TestPlatform.TestHost;
using Monkey.Interpreter.AbstractSyntaxTree;
using System.Net.Http.Headers;

namespace Monkey.Interpreter.Tests;

public class ParserTests
{
    [Fact]
    public void ShouldParseProgramStatements()
    {
        var lexer = new Lexer(@"
            let x = 5;
            let y = 10;
            let foobar = 838383;");
        var parser = new Parser(lexer);
        var program = parser.ParseProgram();

        Assert.NotNull(program);
        Assert.Equal(3, program.Statements().Count);
    }

    [Theory]
    [InlineData("let x = 5;", "x")]
    [InlineData("let y = 10;", "y")]
    [InlineData("let foobar = 838383;", "foobar")]
    public void ShouldParseLetStatement(string statement, string expectedName)
    {
        var lexer = new Lexer(statement);
        var parser = new Parser(lexer);
        var program = parser.ParseProgram();

        var parsedStatement = program.Statements().Single();
        Assert.Equal("let", parsedStatement.GetTokenLiteral());

        var letStatement = (LetStatement)parsedStatement;
        Assert.Equal(TokenType.LET, letStatement.Token.TokenType);
        Assert.Equal(expectedName, letStatement.Name.Value);
        Assert.Equal(expectedName, letStatement.Name.GetTokenLiteral());
    }

    [Theory]
    [InlineData("return 5;")]
    [InlineData("return 10;")]
    [InlineData("return  993322;")]
    public void ShouldParseReturnStatement(string statement)
    {
        var lexer = new Lexer(statement);
        var parser = new Parser(lexer);
        var program = parser.ParseProgram();

        var parsedStatement = program.Statements().Single();
        Assert.Equal("return", parsedStatement.GetTokenLiteral());

        var returnStatement = (ReturnStatement)parsedStatement;
        Assert.Equal(TokenType.RETURN, returnStatement.Token.TokenType);
    }

    [Fact]
    public void ShouldGetParserErrors()
    {
        var lexer = new Lexer(@"
            let x = 5;
            let = 10;
            let 838383;");

        var parser = new Parser(lexer);
        parser.ParseProgram();

        Assert.Equal(2, parser.Errors.Count);
        Assert.Contains(parser.Errors, x => x.Contains($"Expected next token to be {TokenType.IDENTIFIER}. Got {TokenType.ASSIGNMENT} instead."));
        Assert.Contains(parser.Errors, x => x.Contains($"Expected next token to be {TokenType.IDENTIFIER}. Got {TokenType.INTEGER} instead."));
    }

    [Fact]
    public void ShouldGetCorrectToStringFromMonkeyProgram()
    {
        var program = new MonkeyProgram();

        program.AddStatement(new LetStatement(
            new Token(TokenType.LET, "let"),
            new IdentifierExpression(new Token(TokenType.IDENTIFIER, "myVar"), "myVar"),
            new IdentifierExpression(new Token(TokenType.IDENTIFIER, "anotherVar"), "anotherVar")));

        Assert.Equal("let myVar = anotherVar;", program.ToString());
    }

    [Fact]
    public void ShouldParseIdentifierExpression()
    {
        var lexer = new Lexer("foobar;");
        var parser = new Parser(lexer);
        var program = parser.ParseProgram();

        Assert.Single(program.Statements());

        var expressionStatement = (ExpressionStatement)program.Statements()[0];
        var identifierExpression = (IdentifierExpression)expressionStatement.Expression;
        Assert.Equal("foobar", identifierExpression.Value);
        Assert.Equal("foobar", identifierExpression.GetTokenLiteral());
    }

    [Fact]
    public void ShouldParseIntegerLiteralExpression()
    {
        var lexer = new Lexer("5");
        var parser = new Parser(lexer);
        var program = parser.ParseProgram();

        Assert.Single(program.Statements());

        var expressionStatement = (ExpressionStatement)program.Statements()[0];
        var integerLiteralExpression = (IntegerLiteralExpression)expressionStatement.Expression;
        Assert.True(IsIntegerLiteral(integerLiteralExpression, 5));
    }

    [Theory]
    [InlineData("!5;", "!", 5)]
    [InlineData("-15;", "-", 15)]
    public void ShouldParsePrefixExpression(string input, string prefixOperator, int value)
    {
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);
        var program = parser.ParseProgram();

        Assert.Single(program.Statements());

        var expressionStatement = (ExpressionStatement)program.Statements()[0];
        var prefixExpression = (PrefixExpression)expressionStatement.Expression;
        Assert.Equal(prefixOperator, prefixExpression.Operator);
        Assert.True(IsIntegerLiteral(prefixExpression.Right, value));
    }

    private static bool IsIntegerLiteral(IExpression expression, int value)
    {
        if (expression is not IntegerLiteralExpression)
        {
            return false;
        }

        var integerLiteralExpression = (IntegerLiteralExpression)expression;
        if (integerLiteralExpression.Value != value)
        {
            return false;
        }

        if (integerLiteralExpression.GetTokenLiteral() != value.ToString())
        {
            return false;
        }

        return true;
    }
}