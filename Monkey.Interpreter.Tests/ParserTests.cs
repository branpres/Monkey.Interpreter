using Monkey.Interpreter.AbstractSyntaxTree;
using Monkey.Interpreter.AbstractSyntaxTree.Expressions;
using Monkey.Interpreter.AbstractSyntaxTree.Statements;

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
        Assert.Equal(3, program.Statements.Count);
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

        var parsedStatement = program.Statements.Single();
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

        var parsedStatement = program.Statements.Single();
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

        Assert.Single(program.Statements);

        var expressionStatement = (ExpressionStatement)program.Statements[0];
        var identifierExpression = (IdentifierExpression)expressionStatement.Expression;
        Assert.True(IsIdentifier(identifierExpression, "foobar"));
    }

    [Fact]
    public void ShouldParseIntegerLiteralExpression()
    {
        var lexer = new Lexer("5");
        var parser = new Parser(lexer);
        var program = parser.ParseProgram();

        Assert.Single(program.Statements);

        var expressionStatement = (ExpressionStatement)program.Statements[0];
        var integerLiteralExpression = (IntegerLiteralExpression)expressionStatement.Expression;
        Assert.True(IsIntegerLiteral(integerLiteralExpression, 5));
    }

    [Theory]
    [InlineData("!5;", "!", 5)]
    [InlineData("-15;", "-", 15)]
    [InlineData("!true;", "!", true)]
    [InlineData("!false;", "!", false)]
    public void ShouldParsePrefixExpression(string input, string prefixOperator, object value)
    {
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);
        var program = parser.ParseProgram();

        Assert.Single(program.Statements);

        var expressionStatement = (ExpressionStatement)program.Statements[0];
        var prefixExpression = (PrefixExpression)expressionStatement.Expression;
        Assert.True(IsPrefixExpression(prefixExpression, prefixOperator, value));
    }

    [Theory]
    [InlineData("5 + 5;", 5, "+", 5)]
    [InlineData("5 - 5;", 5, "-", 5)]
    [InlineData("5 * 5;", 5, "*", 5)]
    [InlineData("5 / 5;", 5, "/", 5)]
    [InlineData("5 > 5;", 5, ">", 5)]
    [InlineData("5 < 5;", 5, "<", 5)]
    [InlineData("5 == 5;", 5, "==", 5)]
    [InlineData("5 != 5;", 5, "!=", 5)]
    [InlineData("true == true", true, "==", true)]
    [InlineData("true != false", true, "!=", false)]
    [InlineData("false == false", false, "==", false)]
    public void ShouldParseInfixExpression(string input, object leftValue, string infixOperator, object rightValue)
    {
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);
        var program = parser.ParseProgram();

        Assert.Single(program.Statements);

        var expressionStatement = (ExpressionStatement)program.Statements[0];
        var infixExpression = (InfixExpression)expressionStatement.Expression;
        Assert.True(IsInfixExpression(infixExpression, leftValue, infixOperator, rightValue));
    }

    [Theory]
    [InlineData("-a * b", "((-a) * b)")]
    [InlineData("!-a", "(!(-a))")]
    [InlineData("a + b + c", "((a + b) + c)")]
    [InlineData("a + b - c", "((a + b) - c)")]
    [InlineData("a * b * c", "((a * b) * c)")]
    [InlineData("a * b / c", "((a * b) / c)")]
    [InlineData("a + b / c", "(a + (b / c))")]
    [InlineData("a + b * c + d / e - f", "(((a + (b * c)) + (d / e)) - f)")]
    [InlineData("3 + 4; -5 * 5", "(3 + 4)((-5) * 5)")]
    [InlineData("5 > 4 == 3 < 4", "((5 > 4) == (3 < 4))")]
    [InlineData("5 < 4 != 3 > 4", "((5 < 4) != (3 > 4))")]
    [InlineData("3 + 4 * 5 == 3 * 1 + 4 * 5", "((3 + (4 * 5)) == ((3 * 1) + (4 * 5)))")]
    [InlineData("true", "true")]
    [InlineData("false", "false")]
    [InlineData("3 > 5 == false", "((3 > 5) == false)")]
    [InlineData("3 < 5 == true", "((3 < 5) == true)")]
    [InlineData("1 + (2 + 3) + 4", "((1 + (2 + 3)) + 4)")]
    [InlineData("(5 + 5) * 2", "((5 + 5) * 2)")]
    [InlineData("2 / (5 + 5)", "(2 / (5 + 5))")]
    [InlineData("-(5 + 5)", "(-(5 + 5))")]
    [InlineData("!(true == true)", "(!(true == true))")]
    public void ShouldParsePrefixAndInfixExpressionsByPrecedence(string input, string expected)
    {
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);
        var program = parser.ParseProgram();

        Assert.Equal(expected, program.ToString());
    }

    [Theory]
    [InlineData("true", true)]
    [InlineData("false", false)]
    public void ShouldParseBooleanExpression(string input, bool expected)
    {
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);
        var program = parser.ParseProgram();

        Assert.Single(program.Statements);

        var expressionStatement = (ExpressionStatement)program.Statements[0];
        var booleanExpression = (BooleanExpression)expressionStatement.Expression;
        Assert.True(IsBooleanLiteral(booleanExpression, expected));
    }

    [Fact]
    public void ShouldParseIfExpressionWithoutAlternative()
    {
        var lexer = new Lexer("if (x < y) { x }");
        var parser = new Parser(lexer);
        var program = parser.ParseProgram();

        Assert.Single(program.Statements);

        var expressionStatement = (ExpressionStatement)program.Statements[0];
        var ifExpression = (IfExpression)expressionStatement.Expression;
        Assert.True(IsInfixExpression(ifExpression.Condition, "x", "<", "y"));
        Assert.Single(ifExpression.Consequence.Statements);

        var consequenceExpressionStatement = (ExpressionStatement)ifExpression.Consequence.Statements[0];
        Assert.True(IsIdentifier(consequenceExpressionStatement.Expression, "x"));

        Assert.Null(ifExpression.Alternative);
    }

    [Fact]
    public void ShouldParseIfExpressionWithAlternative()
    {
        var lexer = new Lexer("if (x < y) { x } else { y }");
        var parser = new Parser(lexer);
        var program = parser.ParseProgram();

        Assert.Single(program.Statements);

        var expressionStatement = (ExpressionStatement)program.Statements[0];
        var ifExpression = (IfExpression)expressionStatement.Expression;
        Assert.True(IsInfixExpression(ifExpression.Condition, "x", "<", "y"));
        Assert.Single(ifExpression.Consequence.Statements);

        var consequenceExpressionStatement = (ExpressionStatement)ifExpression.Consequence.Statements[0];
        Assert.True(IsIdentifier(consequenceExpressionStatement.Expression, "x"));

        Assert.NotNull(ifExpression.Alternative);
        var alternativeExpressionStatement = (ExpressionStatement)ifExpression.Alternative.Statements[0];
        Assert.True(IsIdentifier(alternativeExpressionStatement.Expression, "y"));
    }

    [Fact]
    public void ShouldParseFunctionLiteralExpression()
    {
        var lexer = new Lexer("fn(x, y) { x + y; }");
        var parser = new Parser(lexer);
        var program = parser.ParseProgram();

        Assert.Single(program.Statements);

        var expressionStatement = (ExpressionStatement)program.Statements[0];
        var functionLiteralExpression = (FunctionLiteralExpression)expressionStatement.Expression;
        Assert.Equal(2, functionLiteralExpression.Parameters.Count);
        Assert.True(IsLiteralExpression(functionLiteralExpression.Parameters[0], "x"));
        Assert.True(IsLiteralExpression(functionLiteralExpression.Parameters[1], "y"));

        Assert.Single(functionLiteralExpression.Body.Statements);
        var bodyExpressionStatement = (ExpressionStatement)functionLiteralExpression.Body.Statements[0];
        Assert.True(IsInfixExpression(bodyExpressionStatement.Expression, "x", "+", "y"));
    }

    [Theory]
    [InlineData("fn() {};", "")]
    [InlineData("fn(x) {};", "x")]
    [InlineData("fn(x, y, z) {};", "x,y,z")]
    public void ShouldParseFunctionLiteralExpressionParameters(string input, string expected)
    {
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);
        var program = parser.ParseProgram();

        var expectedParameters = expected.Split(",", StringSplitOptions.RemoveEmptyEntries);
        var expressionStatement = (ExpressionStatement)program.Statements[0];
        var functionLiteralExpression = (FunctionLiteralExpression)expressionStatement.Expression;

        Assert.Equal(expectedParameters.Length, functionLiteralExpression.Parameters.Count);

        for (var i = 0; i < expectedParameters.Length; i++)
        {
            Assert.True(IsLiteralExpression(functionLiteralExpression.Parameters[i], expectedParameters[i]));
        }       
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

    private static bool IsIdentifier(IExpression expression, string value)
    {
        if (expression is not IdentifierExpression)
        {
            return false;
        }

        var identifierExpression = (IdentifierExpression)expression;
        if (identifierExpression.Value != value)
        {
            return false;
        }

        if (identifierExpression.GetTokenLiteral() != value)
        {
            return false;
        }

        return true;
    }

    private static bool IsBooleanLiteral(IExpression expression, bool value)
    {
        if (expression is not BooleanExpression)
        {
            return false;
        }

        var booleanExpression = (BooleanExpression)expression;
        if (booleanExpression.Value != value)
        {
            return false;
        }

        if (booleanExpression.GetTokenLiteral() != value.ToString().ToLower())
        {
            return false;
        }

        return true;
    }

    private static bool IsLiteralExpression(IExpression expression, object value)
    {
        return value switch
        {
            int => IsIntegerLiteral(expression, (int)value),
            bool => IsBooleanLiteral(expression, (bool)value),
            _ => IsIdentifier(expression, (string)value),
        };
    }

    private static bool IsPrefixExpression(IExpression expression, string @operator, object right)
    {
        if (expression is not PrefixExpression)
        {
            return false;
        }

        var prefixExpression = (PrefixExpression)expression;
        if (prefixExpression.Operator != @operator)
        {
            return false;
        }

        if (!IsLiteralExpression(prefixExpression.Right, right))
        {
            return false;
        }

        return true;
    }

    private static bool IsInfixExpression(IExpression expression, object left, string @operator, object right)
    {
        if (expression is not InfixExpression)
        {
            return false;
        }

        var infixExpression = (InfixExpression)expression;
        if (!IsLiteralExpression(infixExpression.Left, left))
        {
            return false;
        }

        if (infixExpression.Operator != @operator)
        {
            return false;
        }

        if (!IsLiteralExpression(infixExpression.Right, right))
        {
            return false;
        }

        return true;
    }
}