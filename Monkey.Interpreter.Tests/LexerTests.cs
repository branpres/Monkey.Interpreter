namespace Monkey.Interpreter.Tests;

public class LexerTests
{
    [Fact]
    public void ShouldGetTokens()
    {
        var lexer = new Lexer("=+(){},;");

        var expectedTokens = new List<Token>
        {
            new (new TokenType(Constants.ASSIGNMENT), "="),
            new (new TokenType(Constants.PLUS), "+"),
            new (new TokenType(Constants.LEFT_PARENTHESIS), "("),
            new (new TokenType(Constants.RIGHT_PARENTHESIS), ")"),
            new (new TokenType(Constants.LEFT_BRACE), "{"),
            new (new TokenType(Constants.RIGHT_BRACE), "}"),
            new (new TokenType(Constants.COMMA), ","),
            new (new TokenType(Constants.SEMICOLON), ";"),
            new (new TokenType(Constants.ASSIGNMENT), string.Empty),
        };

        foreach (var expectedToken in expectedTokens)
        {
            var token = lexer.GetNextToken();
            Assert.Equal(expectedToken.TokenType, token.TokenType);
            Assert.Equal(expectedToken.Literal, token.Literal);
        }
    }
}