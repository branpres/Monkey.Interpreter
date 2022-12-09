namespace Monkey.Interpreter.Evaluation;

public static class Evaluator
{
    private static readonly BooleanObject TRUE = new(true);
    private static readonly BooleanObject FALSE = new (false);
    private static readonly NullObject NULL = new();

    public static IObject? Evaluate(INode node)
    {
        switch (node)
        {
            case MonkeyProgram p:
                return EvaluateStatements(p.Statements);
            case ExpressionStatement s:
                return Evaluate(s.Expression);
            case IntegerLiteralExpression e:
                return new IntegerObject(e.Value);
            case BooleanExpression e:
                return GetBooleanObjectFromNativeBool(e.Value);
            case PrefixExpression e:
                var right = Evaluate(e.Right);
                return EvaluatePrefixExpression(e.Operator, right);
            default:
                return null;
        }
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

    private static IObject? EvaluatePrefixExpression(string @operator, IObject? right)
    {
        return @operator switch
        {
            "!" => EvaluateBangOperatorExpression(right),
            "-" => EvaluateMinusPrefixOperatorExpression(right),
            _ => null,
        };
    }

    private static IObject? EvaluateBangOperatorExpression(IObject? right)
    {
        if (right != null)
        {
            if (right is not BooleanObject)
            {
                return FALSE;
            }

            if (right is NullObject)
            {
                return NULL;
            }

            var boolean = (BooleanObject)right;
            if (boolean.Value)
            {
                return FALSE;
            }

            return TRUE;
        }

        return FALSE;
    }

    private static IObject? EvaluateMinusPrefixOperatorExpression(IObject? right)
    {
        if (right != null)
        {
            if (right is not IntegerObject)
            {
                return NULL;
            }

            var integer = (IntegerObject)right;

            return new IntegerObject(-integer.Value);
        }

        return NULL;
    }
}