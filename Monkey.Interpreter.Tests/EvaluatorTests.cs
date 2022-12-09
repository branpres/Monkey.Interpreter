namespace Monkey.Interpreter.Tests;

public class EvaluatorTests
{
    [Theory]
    [InlineData("5", 5)]
    [InlineData("10", 10)]
    [InlineData("-5", -5)]
    [InlineData("-10", -10)]
    public void ShouldEvaluateIntegerLiteralExpressionToIntegerObject(string input, int expected)
    {
        var evaluated = GetEvaluatedObject(input);
        Assert.True(IsIntegerObject(evaluated, expected));
    }

    [Theory]
    [InlineData("true", true)]
    [InlineData("false", false)]
    public void ShouldEvaluateBooleanExpressionToBooleanObject(string input, bool expected)
    {
        var evaluated = GetEvaluatedObject(input);
        Assert.True(IsBooleanObject(evaluated, expected));
    }

    [Theory]
    [InlineData("!true", false)]
    [InlineData("!false", true)]
    [InlineData("!5", false)]
    [InlineData("!!true", true)]
    [InlineData("!!false", false)]
    [InlineData("!!5", true)]
    public void ShouldEvaluateBangOperator(string input, bool expected)
    {
        var evaluated = GetEvaluatedObject(input);
        Assert.True(IsBooleanObject(evaluated, expected));
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
        return @object is IntegerObject integerObject && integerObject.Value == expected;
    }

    private static bool IsBooleanObject(IObject? @object, bool expected)
    {
        return @object is BooleanObject booleanObject && booleanObject.Value == expected;
    }
}