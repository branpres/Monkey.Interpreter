namespace Monkey.Interpreter.Evaluation;

public static class Evaluator
{
    private static readonly BooleanObject TRUE = new(true);
    private static readonly BooleanObject FALSE = new (false);
    private static readonly NullObject NULL = new();

    public static IObject? Evaluate(INode node, Environment env)
    {
        IObject? left;
        IObject? right;
        switch (node)
        {
            case MonkeyProgram p:
                return EvaluateProgram(p, env);
            case ExpressionStatement s:
                return Evaluate(s.Expression, env);
            case IntegerLiteralExpression e:
                return new IntegerObject(e.Value);
            case BooleanExpression e:
                return GetBooleanObjectFromNativeBool(e.Value);
            case PrefixExpression e:
                right = Evaluate(e.Right, env);
                if (IsError(right))
                {
                    return right;
                }

                return EvaluatePrefixExpression(e.Operator, right);
            case InfixExpression e:
                left = Evaluate(e.Left, env);
                if (IsError(left))
                {
                    return left;
                }

                right = Evaluate(e.Right, env);
                if (IsError(right))
                {
                    return right;
                }

                return EvaluateInfixExpression(e.Operator, left, right);
            case BlockStatement s:
                return EvaluateBlockStatement(s, env);
            case IfExpression e:
                return EvaluateIfExpression(e, env);
            case ReturnStatement s:
                var returnValue = Evaluate(s.ReturnValue, env);
                if (IsError(returnValue))
                {
                    return returnValue;
                }

                return returnValue == null ? NULL : new ReturnValueObject(returnValue);
            case LetStatement s:
                var letStatement = Evaluate(s.Value, env);
                if (IsError(letStatement))
                {
                    return letStatement;
                }

                env.Set(s.Name.Value, letStatement);

                return letStatement;
            case IdentifierExpression e:
                return EvaluateIdentifierExpression(e, env);
            default:
                return NULL;
        }
    }

    private static IObject? EvaluateProgram(MonkeyProgram program, Environment env)
    {
        IObject? @object = null;

        foreach (var statement in program.Statements)
        {
            @object = Evaluate(statement, env);

            if (@object is ReturnValueObject returnValueObject)
            {
                return returnValueObject.Value;
            }
            else if (@object is ErrorObject errorObject)
            {
                return errorObject;
            }
        }

        return @object;
    }

    private static BooleanObject GetBooleanObjectFromNativeBool(bool input)
    {
        return input ? TRUE : FALSE;
    }

    private static IObject? EvaluatePrefixExpression(string @operator, IObject? right)
    {
        switch (@operator)
        {
            case Constants.Operator.BANG:
                return EvaluateBangOperatorExpression(right);
            case Constants.Operator.MINUS:
                return EvaluateMinusPrefixOperatorExpression(right);
            default:
                if (right != null)
                {
                    return CreateError("unknown operator: ", @operator, right.Type().ToString());
                }

                return NULL;
        }
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
        else if (left != null && right != null)
        {
            if (left.Type() != right.Type())
            {
                return CreateError("type mismatch: ", left.Type().ToString(), @operator, right.Type().ToString());
            }

            return CreateError("unknown operator: ", left.Type().ToString(), @operator, right.Type().ToString());
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
            _ => CreateError("unknown operator: ", left.Type().ToString(), @operator, right.Type().ToString()),
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
                return CreateError("unknown operator: -", right.Type().ToString());
            }

            var integer = (IntegerObject)right;

            return new IntegerObject(-integer.Value);
        }

        return NULL;
    }

    private static IObject? EvaluateIfExpression(IfExpression expression, Environment env)
    {
        var condition = Evaluate(expression.Condition, env);
        if (IsError(condition))
        {
            return condition;
        }

        if (IsTruthy(condition))
        {
            return Evaluate(expression.Consequence, env);
        }
        else if (expression.Alternative != null)
        {
            return Evaluate(expression.Alternative, env);
        }
        else
        {
            return NULL;
        }
    }

    private static IObject? EvaluateBlockStatement(BlockStatement blockStatement, Environment env)
    {
        IObject? @object = null;

        foreach (var statement in blockStatement.Statements)
        {
            @object = Evaluate(statement, env);
            if (@object != null && (@object is ReturnValueObject || @object is ErrorObject))
            {
                return @object;
            }
        }

        return @object;
    }

    private static IObject? EvaluateIdentifierExpression(IdentifierExpression identifierExpression, Environment env)
    {
        var value = env.Get(identifierExpression.Value);
        if (value == null)
        {
            return CreateError($"identifier not found: {identifierExpression.Value}");
        }

        return value;
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

    private static bool IsError(IObject? @object)
    {
        if (@object != NULL)
        {
            return @object is ErrorObject;
        }

        return false;
    }

    private static ErrorObject CreateError(string errorMessage, params string[] args)
    {
        return new ErrorObject($"{errorMessage}{string.Join(" ", args)}");
    }
}