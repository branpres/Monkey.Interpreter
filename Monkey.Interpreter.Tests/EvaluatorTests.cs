namespace Monkey.Interpreter.Tests;

public class EvaluatorTests
{
    [Theory]
    [InlineData("5", 5)]
    [InlineData("10", 10)]
    public void ShouldEvaluateIntegerLiteralExpressionToIntegerObject(string input, int expected)
    {
        var evaluated = GetEvaluatedObject(input);
        Assert.True(IsIntegerObject(evaluated, expected));
    }

    private static IObject? GetEvaluatedObject(string input)
    {
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);
        var program = parser.ParseProgram();

        return Evaluator.Evaluate(program);
    }

    private static bool IsIntegerObject(IObject? @object, int expected)
    {
        return @object is Integer integer && integer.Value == expected;
    }
}