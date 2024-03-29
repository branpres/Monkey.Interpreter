﻿namespace Monkey.Interpreter.AbstractSyntaxTree.Statements;

public class ExpressionStatement : Node, IStatement
{
    public IExpression Expression { get; }

    public ExpressionStatement(Token token, IExpression expression) : base(token)
    {
        Expression = expression;
    }

    public override string ToString()
    {
        if (Expression != null)
        {
            return Expression.ToString();
        }

        return string.Empty;
    }
}