namespace Monkey.Interpreter.Evaluation;

public static class Evaluator
{
    private static readonly BooleanObject TRUE = new(true);
    private static readonly BooleanObject FALSE = new (false);
    private static readonly NullObject NULL = new();

    public static IObject? Evaluate(INode node)
    {
        return node switch
        {
            MonkeyProgram p => EvaluateStatements(p.Statements),
            ExpressionStatement s => Evaluate(s.Expression),
            IntegerLiteralExpression e => new IntegerObject(e.Value),
            BooleanExpression e => GetBooleanObjectFromNativeBool(e.Value),
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

    private static BooleanObject GetBooleanObjectFromNativeBool(bool input)
    {
        return input ? TRUE : FALSE;
    }
}