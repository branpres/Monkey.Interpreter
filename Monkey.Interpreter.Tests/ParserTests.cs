using Monkey.Interpreter.AbstractSyntaxTree;

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
}