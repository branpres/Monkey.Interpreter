namespace Monkey.Interpreter.Tests;

public class LexerTests
{
    [Fact]
    public void ShouldGetTokens()
    {
        var lexer = new Lexer(@"
            let five = 5;
            let ten = 10;

            let add = fn(x, y) {
              x + y;
            };

            let result = add(five, ten);");

        var expectedTokens = new List<Token>
        {
            new (new TokenType(Constants.LET), "let"),
            new (new TokenType(Constants.IDENTIFIER), "five"),
            new (new TokenType(Constants.ASSIGNMENT), "="),
            new (new TokenType(Constants.INTEGER), "5"),
            new (new TokenType(Constants.SEMICOLON), ";"),
            new (new TokenType(Constants.LET), "let"),
            new (new TokenType(Constants.IDENTIFIER), "ten"),
            new (new TokenType(Constants.ASSIGNMENT), "="),
            new (new TokenType(Constants.INTEGER), "10"),
            new (new TokenType(Constants.SEMICOLON), ";"),
            new (new TokenType(Constants.LET), "let"),
            new (new TokenType(Constants.IDENTIFIER), "add"),
            new (new TokenType(Constants.ASSIGNMENT), "="),
            new (new TokenType(Constants.FUNCTION), "fn"),
            new (new TokenType(Constants.LEFT_PARENTHESIS), "("),
            new (new TokenType(Constants.IDENTIFIER), "x"),
            new (new TokenType(Constants.COMMA), ","),
            new (new TokenType(Constants.IDENTIFIER), "y"),
            new (new TokenType(Constants.RIGHT_PARENTHESIS), ")"),
            new (new TokenType(Constants.LEFT_BRACE), "{"),
            new (new TokenType(Constants.IDENTIFIER), "x"),
            new (new TokenType(Constants.PLUS), "+"),
            new (new TokenType(Constants.IDENTIFIER), "y"),
            new (new TokenType(Constants.SEMICOLON), ";"),
            new (new TokenType(Constants.RIGHT_BRACE), "}"),
            new (new TokenType(Constants.SEMICOLON), ";"),
            new (new TokenType(Constants.LET), "let"),
            new (new TokenType(Constants.IDENTIFIER), "result"),
            new (new TokenType(Constants.ASSIGNMENT), "="),
            new (new TokenType(Constants.IDENTIFIER), "add"),
            new (new TokenType(Constants.LEFT_PARENTHESIS), "("),
            new (new TokenType(Constants.IDENTIFIER), "five"),
            new (new TokenType(Constants.COMMA), ","),
            new (new TokenType(Constants.IDENTIFIER), "ten"),
            new (new TokenType(Constants.RIGHT_PARENTHESIS), ")"),
            new (new TokenType(Constants.SEMICOLON), ";"),
            new (new TokenType(Constants.EOF), "\0"),
        };

        foreach (var expectedToken in expectedTokens)
        {
            var token = lexer.GetNextToken();
            Assert.Equal(expectedToken.TokenType.Value, token.TokenType.Value);
            Assert.Equal(expectedToken.Literal, token.Literal);
        }
    }
}