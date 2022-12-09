namespace Monkey.Interpreter.Evaluation;

public static class Evaluator
{
    public static IObject? Evaluate(INode node)
    {
        return node switch
        {
            MonkeyProgram p => EvaluateStatements(p.Statements),
            ExpressionStatement s => Evaluate(s.Expression),
            IntegerLiteralExpression e => new Integer(e.Value),
            _ => null,
        };
    }

    private static IObject? EvaluateStatements(List<IStatement> statements)
    {
        IObject? @object = null;

        foreach (var statement in statements)
        {
            @object = Evaluate(statement);
        }

        return @object;
    }
}