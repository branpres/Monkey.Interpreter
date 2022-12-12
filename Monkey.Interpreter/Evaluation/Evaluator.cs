namespace Monkey.Interpreter.Evaluation;

public static class Evaluator
{
    private static readonly BooleanObject TRUE = new(true);
    private static readonly BooleanObject FALSE = new (false);
    private static readonly NullObject NULL = new();

    public static IObject? Evaluate(INode node)
    {
        IObject? left;
        IObject? right;
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
                right = Evaluate(e.Right);
                return EvaluatePrefixExpression(e.Operator, right);
            case InfixExpression e:
                left = Evaluate(e.Left);
                right = Evaluate(e.Right);
                return EvaluateInfixExpression(e.Operator, left, right);
            case BlockStatement s:
                return EvaluateStatements(s.Statements);
            case IfExpression e:
                return EvaluateIfExpression(e);
            default:
                return NULL;
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
            Constants.Operator.BANG => EvaluateBangOperatorExpression(right),
            Constants.Operator.MINUS => EvaluateMinusPrefixOperatorExpression(right),
            _ => NULL,
        };
    }

    private static IObject? EvaluateInfixExpression(string @operator, IObject? left, IObject? right)
    {
        if (left is IntegerObject && right is IntegerObject)
        {
            return EvaluateIntegerInfixExpression(@operator, left, right);
        }
        else if (@operator == Constants.Operator.EQUAL)
        {
            return GetBooleanObjectFromNativeBool(left == right);
        }
        else if (@operator == Constants.Operator.NOT_EQUAL)
        {
            return GetBooleanObjectFromNativeBool(left != right);
        }

        return NULL;
    }

    private static IObject? EvaluateIntegerInfixExpression(string @operator, IObject left, IObject right)
    {
        var leftValue = ((IntegerObject)left).Value;
        var rightValue = ((IntegerObject)right).Value;

        return @operator switch
        {
            Constants.Operator.PLUS => new IntegerObject(leftValue + rightValue),
            Constants.Operator.MINUS => new IntegerObject(leftValue - rightValue),
            Constants.Operator.ASTERISK => new IntegerObject(leftValue * rightValue),
            Constants.Operator.SLASH => new IntegerObject(leftValue / rightValue),
            Constants.Operator.LESS_THAN => GetBooleanObjectFromNativeBool(leftValue < rightValue),
            Constants.Operator.GREATER_THAN => GetBooleanObjectFromNativeBool(leftValue > rightValue),
            Constants.Operator.EQUAL => GetBooleanObjectFromNativeBool(leftValue == rightValue),
            Constants.Operator.NOT_EQUAL => GetBooleanObjectFromNativeBool(leftValue != rightValue),
            _ => NULL,
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

    private static IObject? EvaluateIfExpression(IfExpression expression)
    {
        var condition = Evaluate(expression.Condition);

        if (IsTruthy(condition))
        {
            return Evaluate(expression.Consequence);
        }
        else if (expression.Alternative != null)
        {
            return Evaluate(expression.Alternative);
        }
        else
        {
            return NULL;
        }
    }

    private static bool IsTruthy(IObject? @object)
    { 
        if (@object == NULL)
        {
            return true;
        }
        else if (@object == TRUE)
        {
            return true;
        }
        else if (@object == FALSE)
        {
            return false;
        }

        return true;
    }
}