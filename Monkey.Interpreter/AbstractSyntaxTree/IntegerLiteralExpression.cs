﻿namespace Monkey.Interpreter.AbstractSyntaxTree;

public class IntegerLiteralExpression : Node, IExpression
{
    public int Value { get; }

    public IntegerLiteralExpression(Token token, int value) : base(token)
    {
        Value = value;
    }

    public override string ToString()
    {
        return Token.Literal;
    }
}