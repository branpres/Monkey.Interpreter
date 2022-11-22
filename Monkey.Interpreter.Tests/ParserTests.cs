namespace Monkey.Interpreter.Tests;

public class ParserTests
{
    [Fact]
    public void ShouldParseLetStatements()
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
}