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
            10 != 9;
            ""foobar""
            ""foo bar""");

        var expectedTokens = new List<Token>
        {
            new (TokenType.LET, "let"),
            new (TokenType.IDENTIFIER, "five"),
            new (TokenType.ASSIGNMENT, "="),
            new (TokenType.INTEGER, "5"),
            new (TokenType.SEMICOLON, ";"),
            new (TokenType.LET, "let"),
            new (TokenType.IDENTIFIER, "ten"),
            new (TokenType.ASSIGNMENT, "="),
            new (TokenType.INTEGER, "10"),
            new (TokenType.SEMICOLON, ";"),
            new (TokenType.LET, "let"),
            new (TokenType.IDENTIFIER, "add"),
            new (TokenType.ASSIGNMENT, "="),
            new (TokenType.FUNCTION, "fn"),
            new (TokenType.LEFT_PARENTHESIS, "("),
            new (TokenType.IDENTIFIER, "x"),
            new (TokenType.COMMA, ","),
            new (TokenType.IDENTIFIER, "y"),
            new (TokenType.RIGHT_PARENTHESIS, ")"),
            new (TokenType.LEFT_BRACE, "{"),
            new (TokenType.IDENTIFIER, "x"),
            new (TokenType.PLUS, "+"),
            new (TokenType.IDENTIFIER, "y"),
            new (TokenType.SEMICOLON, ";"),
            new (TokenType.RIGHT_BRACE, "}"),
            new (TokenType.SEMICOLON, ";"),
            new (TokenType.LET, "let"),
            new (TokenType.IDENTIFIER, "result"),
            new (TokenType.ASSIGNMENT, "="),
            new (TokenType.IDENTIFIER, "add"),
            new (TokenType.LEFT_PARENTHESIS, "("),
            new (TokenType.IDENTIFIER, "five"),
            new (TokenType.COMMA, ","),
            new (TokenType.IDENTIFIER, "ten"),
            new (TokenType.RIGHT_PARENTHESIS, ")"),
            new (TokenType.SEMICOLON, ";"),
            new (TokenType.BANG, "!"),
            new (TokenType.MINUS, "-"),
            new (TokenType.SLASH, "/"),
            new (TokenType.ASTERISK, "*"),
            new (TokenType.INTEGER, "5"),
            new (TokenType.SEMICOLON, ";"),
            new (TokenType.INTEGER, "5"),
            new (TokenType.LESS_THAN, "<"),
            new (TokenType.INTEGER, "10"),
            new (TokenType.GREATER_THAN, ">"),
            new (TokenType.INTEGER, "5"),
            new (TokenType.SEMICOLON, ";"),
            new (TokenType.IF, "if"),
            new (TokenType.LEFT_PARENTHESIS, "("),
            new (TokenType.INTEGER, "5"),
            new (TokenType.LESS_THAN, "<"),
            new (TokenType.INTEGER, "10"),
            new (TokenType.RIGHT_PARENTHESIS, ")"),
            new (TokenType.LEFT_BRACE, "{"),
            new (TokenType.RETURN, "return"),
            new (TokenType.TRUE, "true"),
            new (TokenType.SEMICOLON, ";"),
            new (TokenType.RIGHT_BRACE, "}"),
            new (TokenType.ELSE, "else"),
            new (TokenType.LEFT_BRACE, "{"),
            new (TokenType.RETURN, "return"),
            new (TokenType.FALSE, "false"),
            new (TokenType.SEMICOLON, ";"),
            new (TokenType.RIGHT_BRACE, "}"),
            new (TokenType.INTEGER, "10"),
            new (TokenType.EQUAL, "=="),
            new (TokenType.INTEGER, "10"),
            new (TokenType.SEMICOLON, ";"),
            new (TokenType.INTEGER, "10"),
            new (TokenType.NOT_EQUAL, "!="),
            new (TokenType.INTEGER, "9"),
            new (TokenType.SEMICOLON, ";"),            
            new (TokenType.STRING, "foobar"),
            new (TokenType.STRING, "foo bar"),
            new (TokenType.EOF, "\0")
        };

        var lexedTokens = new List<Token>();
        for (var i = 0; i < expectedTokens.Count; i++)
        {
            lexedTokens.Add(lexer.GetNextToken());            
        }

        Assert.Equal(expectedTokens, lexedTokens);
    }
}