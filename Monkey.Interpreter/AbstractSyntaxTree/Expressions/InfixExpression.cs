﻿namespace Monkey.Interpreter.AbstractSyntaxTree.Expressions;

public class InfixExpression : Node, IExpression
{
    public IExpression Left { get; }

    public string Operator { get; }

    public IExpression Right { get; }

    public InfixExpression(Token token, IExpression left, string @operator, IExpression right) : base(token)
    {
        Left = left;
        Operator = @operator;
        Right = right;
    }

    public override string ToString() => $"({Left.ToString()} {Operator} {Right.ToString()})";
}