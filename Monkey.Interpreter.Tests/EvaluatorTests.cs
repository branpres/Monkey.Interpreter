namespace Monkey.Interpreter.Tests;

public class EvaluatorTests
{
    [Theory]
    [InlineData("5", 5)]
    [InlineData("10", 10)]
    [InlineData("-5", -5)]
    [InlineData("-10", -10)]
    [InlineData("5 + 5 + 5 + 5 - 10", 10)]
    [InlineData("2 * 2 * 2 * 2 * 2", 32)]
    [InlineData("-50 + 100 + -50", 0)]
    [InlineData("5 * 2 + 10", 20)]
    [InlineData("5 + 2 * 10", 25)]
    [InlineData("20 + 2 * -10", 0)]
    [InlineData("50 / 2 * 2 + 10", 60)]
    [InlineData("2 * (5 + 10)", 30)]
    [InlineData("3 * 3 * 3 + 10", 37)]
    [InlineData("3 * (3 * 3) + 10", 37)]
    [InlineData("(5 + 10 * 2 + 15 / 3) * 2 + -10", 50)]
    public void ShouldEvaluateIntegerLiteralExpressionToIntegerObject(string input, int expected)
    {
        var evaluated = GetEvaluatedObject(input);
        Assert.True(IsIntegerObject(evaluated, expected));
    }

    [Theory]
    [InlineData("true", true)]
    [InlineData("false", false)]
    [InlineData("1 < 2", true)]
    [InlineData("1 > 2", false)]
    [InlineData("1 < 1", false)]
    [InlineData("1 > 1", false)]
    [InlineData("1 == 1", true)]
    [InlineData("1 != 1", false)]
    [InlineData("1 == 2", false)]
    [InlineData("1 != 2", true)]
    [InlineData("true == true", true)]
    [InlineData("false == false", true)]
    [InlineData("true == false", false)]
    [InlineData("true != false", true)]
    [InlineData("false != true", true)]
    [InlineData("(1 < 2) == true", true)]
    [InlineData("(1 < 2) == false", false)]
    [InlineData("(1 > 2) == true", false)]
    [InlineData("(1 > 2) == false", true)]
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

    [Theory]
    [InlineData("if (true) { 10 }", 10)]
    [InlineData("if (false) { 10 }", null)]
    [InlineData("if (1) { 10 }", 10)]
    [InlineData("if (1 < 2) { 10 }", 10)]
    [InlineData("if (1 > 2) { 10 }", null)]
    [InlineData("if (1 > 2) { 10 } else { 20 }", 20)]
    [InlineData("if (1 < 2) { 10 } else { 20 }", 10)]
    public void ShouldEvaluateIfElseExpression(string input, int? expected)
    {
        var evaluated = GetEvaluatedObject(input);
        if (expected is int @int)
        {
            Assert.True(IsIntegerObject(evaluated, @int));
        }
        else
        {
            Assert.True(IsNullObject(evaluated));
        }
    }

    [Theory]
    [InlineData("return 10;", 10)]
    [InlineData("return 10; 9;", 10)]
    [InlineData("return 2 * 5; 9;", 10)]
    [InlineData("9; return 2 * 5; 9;", 10)]
    [InlineData("if (10 > 1) { if (10 > 1) { return 10; } return 1; }", 10)]
    public void ShouldEvaluateReturnStatement(string input, int expected)
    {
        var evaluated = GetEvaluatedObject(input);
        Assert.True(IsIntegerObject(evaluated, expected));
    }

    [Theory]
    [InlineData("5 + true;", "type mismatch: INTEGER + BOOLEAN")]
    [InlineData("5 + true; 5;", "type mismatch: INTEGER + BOOLEAN")]
    [InlineData("-true", "unknown operator: -BOOLEAN")]
    [InlineData("true + false;", "unknown operator: BOOLEAN + BOOLEAN")]
    [InlineData("5; true + false; 5", "unknown operator: BOOLEAN + BOOLEAN")]
    [InlineData("if (10 > 1) { true + false; }", "unknown operator: BOOLEAN + BOOLEAN")]
    [InlineData(@"if (10 > 1) {
        if (10 > 1) {
            return true + false;
        }

        return 1;
        }", "unknown operator: BOOLEAN + BOOLEAN")]
    [InlineData("foobar", "identifier not found: foobar")]
    public void ShouldHandleErrors(string input, string expected)
    {
        var evaluated = GetEvaluatedObject(input);
        var errorObject = evaluated as ErrorObject;
        Assert.NotNull(errorObject);
        Assert.Equal(expected, errorObject.Message);
    }

    [Theory]
    [InlineData("let a = 5; a;", 5)]
    [InlineData("let a = 5 * 5; a;", 25)]
    [InlineData("let a = 5; let b = a; b;", 5)]
    [InlineData("let a = 5; let b = a; let c = a + b + 5; c;", 15)]
    public void ShouldEvaluateLetStatement(string input, int expected)
    {
        var evaluated = GetEvaluatedObject(input);
        Assert.True(IsIntegerObject(evaluated, expected));
    }

    [Fact]
    public void ShouldEvaluateFunction()
    {
        var input = "fn(x) { x + 2; };";

        var evaluated = GetEvaluatedObject(input);
        var function = evaluated as FunctionObject;

        Assert.NotNull(function);
        Assert.Single(function.Parameters);
        Assert.Equal("x", function.Parameters[0].ToString());
        Assert.Equal("(x + 2)", function.Body.ToString());
    }

    private static IObject? GetEvaluatedObject(string input)
    {
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);
        var program = parser.ParseProgram();

        return Evaluator.Evaluate(program, new Evaluation.Environment());
    }

    private static bool IsIntegerObject(IObject? @object, int expected)
    {
        return @object is IntegerObject integerObject && integerObject.Value == expected;
    }

    private static bool IsBooleanObject(IObject? @object, bool expected)
    {
        return @object is BooleanObject booleanObject && booleanObject.Value == expected;
    }

    private static bool IsNullObject(IObject? @object)
    {
        return @object is NullObject;
    }
}