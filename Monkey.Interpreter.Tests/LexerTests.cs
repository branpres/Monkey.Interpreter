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

            let result = add(five, ten);
            !-/*5;
            5 < 10 > 5;
            
            if (5 < 10) {
                return true;
            } else {
                return false;
            }

            10 == 10;
            10 != 9;");

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
            new (new TokenType(Constants.BANG), "!"),
            new (new TokenType(Constants.MINUS), "-"),
            new (new TokenType(Constants.SLASH), "/"),
            new (new TokenType(Constants.ASTERISK), "*"),
            new (new TokenType(Constants.INTEGER), "5"),
            new (new TokenType(Constants.SEMICOLON), ";"),
            new (new TokenType(Constants.INTEGER), "5"),
            new (new TokenType(Constants.LESS_THAN), "<"),
            new (new TokenType(Constants.INTEGER), "10"),
            new (new TokenType(Constants.GREATER_THAN), ">"),
            new (new TokenType(Constants.INTEGER), "5"),
            new (new TokenType(Constants.SEMICOLON), ";"),
            new (new TokenType(Constants.IF), "if"),
            new (new TokenType(Constants.LEFT_PARENTHESIS), "("),
            new (new TokenType(Constants.INTEGER), "5"),
            new (new TokenType(Constants.LESS_THAN), "<"),
            new (new TokenType(Constants.INTEGER), "10"),
            new (new TokenType(Constants.RIGHT_PARENTHESIS), ")"),
            new (new TokenType(Constants.LEFT_BRACE), "{"),
            new (new TokenType(Constants.RETURN), "return"),
            new (new TokenType(Constants.TRUE), "true"),
            new (new TokenType(Constants.SEMICOLON), ";"),
            new (new TokenType(Constants.RIGHT_BRACE), "}"),
            new (new TokenType(Constants.ELSE), "else"),
            new (new TokenType(Constants.LEFT_BRACE), "{"),
            new (new TokenType(Constants.RETURN), "return"),
            new (new TokenType(Constants.FALSE), "false"),
            new (new TokenType(Constants.SEMICOLON), ";"),
            new (new TokenType(Constants.RIGHT_BRACE), "}"),
            new (new TokenType(Constants.INTEGER), "10"),
            new (new TokenType(Constants.EQUAL), "=="),
            new (new TokenType(Constants.INTEGER), "10"),
            new (new TokenType(Constants.SEMICOLON), ";"),
            new (new TokenType(Constants.INTEGER), "10"),
            new (new TokenType(Constants.NOT_EQUAL), "!="),
            new (new TokenType(Constants.INTEGER), "9"),
            new (new TokenType(Constants.SEMICOLON), ";"),
            new (new TokenType(Constants.EOF), "\0"),
        };

        var lexedTokens = new List<Token>();
        for (var i = 0; i < expectedTokens.Count; i++)
        {
            lexedTokens.Add(lexer.GetNextToken());            
        }

        Assert.Equal(expectedTokens, lexedTokens);
    }
}