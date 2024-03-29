﻿namespace Monkey.Interpreter.Evaluation;

public static class Evaluator
{
    private static readonly BooleanObject TRUE = new(true);
    private static readonly BooleanObject FALSE = new(false);
    private static readonly NullObject NULL = new();

    private static readonly Dictionary<string, BuiltInObject> _builtInFunctions = new()
    {
        { "len", new BuiltInObject(LenBuiltInFunction) },
        { "first", new BuiltInObject(FirstBuiltInFunction) },
        { "last", new BuiltInObject(LastBuiltInFunction) },
        { "rest", new BuiltInObject(RestBuiltInFunction) },
        { "push", new BuiltInObject(PushBuiltInFunction) }
    };

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
            case FunctionLiteralExpression e:
                return new FunctionObject(e.Parameters, e.Body, env);
            case CallExpression e:
                var function = Evaluate(e.Function, env);
                if (IsError(function))
                {
                    return function;
                }

                var arguments = EvaluateExpressions(e.Arguments, env);
                if (arguments.Count == 1 && IsError(arguments[0]))
                {
                    return arguments[0];
                }

                return ApplyFunction(function, arguments);
            case StringLiteralExpression e:
                return new StringObject(e.Value);
            case ArrayLiteralExpression e:
                var elements = EvaluateExpressions(e.Elements, env);
                if (elements.Count == 1 && IsError(elements[0]))
                {
                    return elements[0];
                }
                                
                return new ArrayObject(elements.ToArray());
            case IndexExpression e:
                left = Evaluate(e.Left, env);
                if (IsError(left))
                {
                    return left;
                }

                var index = Evaluate(e.Index, env);
                if (IsError(index))
                {
                    return index;
                }

                return EvaluateIndexExpression(left, index);
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
            if (left.Type() == ObjectType.STRING && right.Type() == ObjectType.STRING)
            {
                return EvaluateStringInfixExpression(@operator, left, right);
            }

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
        if (value != null)
        {
            return value;
        }

        if (_builtInFunctions.TryGetValue(identifierExpression.Value, out var builtInFunction))
        {
            return builtInFunction;
        }

        return CreateError($"identifier not found: {identifierExpression.Value}");
    }

    private static IObject? EvaluateStringInfixExpression(string @operator, IObject left, IObject right)
    {
        if (@operator != "+")
        {
            return CreateError($"unknown operator: {left.Type()} {@operator} {right.Type()}");
        }

        return new StringObject(((StringObject)left).Value + ((StringObject)right).Value);
    }

    private static List<IObject> EvaluateExpressions(List<IExpression> expressions, Environment env)
    {
        var objects = new List<IObject>();

        foreach (var expression in expressions)
        {
            var evaluated = Evaluate(expression, env);
            if (evaluated != null)
            {
                objects.Add(evaluated);
            }
            
            if (IsError(evaluated))
            {                
                break;
            }
        }

        return objects;
    }

    private static IObject? EvaluateIndexExpression(IObject? left, IObject? index)
    {
        if (left != null && index != null)
        {
            if (left.Type() == ObjectType.ARRAY && index.Type() == ObjectType.INTEGER)
            {
                return EvaluateArrayIndexExpression(left, index);
            }

            return CreateError($"index operator not supported: {left.Type()}");
        }

        return NULL;
    }

    private static IObject EvaluateArrayIndexExpression(IObject array, IObject index)
    {
        var arrayObject = (ArrayObject)array;
        var arrayIndex = ((IntegerObject)index).Value;
        var max = arrayObject.Elements.Length - 1;

        if (arrayIndex < 0 || arrayIndex > max)
        {
            return NULL;
        }

        return arrayObject.Elements[arrayIndex];
    }

    private static IObject? ApplyFunction(IObject? functionObject, List<IObject>? arguments)
    {
        if (functionObject == null)
        {
            return NULL;
        }

        if (functionObject is FunctionObject function)
        {
            var extendedEnv = ExtendFunctionEnvironment(function, arguments);
            var evaluated = Evaluate(function.Body, extendedEnv);
            return UnwrapReturnValue(evaluated);
        }
        
        if (functionObject is BuiltInObject builtInObject)
        {
            return builtInObject.BuiltInFunction(arguments);
        }

        return CreateError($"not a function {functionObject.Type()}");
    }

    private static Environment ExtendFunctionEnvironment(FunctionObject function, List<IObject>? arguments)
    {
        var env = Environment.EnclosedEnvironment(function.Environment);

        if (arguments != null)
        {
            for (var i = 0; i < function.Parameters.Count; i++)
            {
                env.Set(function.Parameters[i].Value, arguments[i]);
            }
        }

        return env;
    }

    private static IObject? UnwrapReturnValue(IObject? @object)
    {
        if (@object == null)
        {
            return NULL;
        }

        if (@object is ReturnValueObject returnValue)
        {
            return returnValue.Value;
        }

        return @object;
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

    private static IObject LenBuiltInFunction(List<IObject>? objects)
    {
        if (objects == null)
        {
            return CreateError($"wrong number of arguments. got=0, want=1");
        }

        if (objects.Count > 1)
        {
            return CreateError($"wrong number of arguments. got={objects.Count}, want=1");
        }

        if (objects[0] != null)
        {
            if (objects[0] is StringObject stringObject)
            {
                return new IntegerObject(stringObject.Value.Length);
            }

            if (objects[0] is ArrayObject arrayObject)
            {
                return new IntegerObject(arrayObject.Elements.Length);
            }

            return CreateError($"argument to `len` not supported, got {objects[0].Type()}");
        }

        return NULL;
    }

    private static IObject FirstBuiltInFunction(List<IObject>? objects)
    {
        if (objects == null)
        {
            return CreateError($"wrong number of arguments. got=0, want=1");
        }

        if (objects.Count != 1)
        {
            return CreateError($"wrong number of arguments. got={objects.Count}, want=1");
        }

        if (objects[0].Type() != ObjectType.ARRAY)
        {
            return CreateError($"argument to 'first' must be ARRAY, got {objects[0].Type()}");
        }

        var array = (ArrayObject)objects[0];
        if (array.Elements.Length > 0)
        {
            return array.Elements[0];
        }

        return NULL;
    }

    private static IObject LastBuiltInFunction(List<IObject>? objects)
    {
        if (objects == null)
        {
            return CreateError($"wrong number of arguments. got=0, want=1");
        }

        if (objects.Count != 1)
        {
            return CreateError($"wrong number of arguments. got={objects.Count}, want=1");
        }

        if (objects[0].Type() != ObjectType.ARRAY)
        {
            return CreateError($"argument to 'last' must be ARRAY, got {objects[0].Type()}");
        }

        var array = (ArrayObject)objects[0];
        if (array.Elements.Length > 0)
        {
            return array.Elements[^1];
        }

        return NULL;
    }

    private static IObject RestBuiltInFunction(List<IObject>? objects)
    {
        if (objects == null)
        {
            return CreateError($"wrong number of arguments. got=0, want=1");
        }

        if (objects.Count != 1)
        {
            return CreateError($"wrong number of arguments. got={objects.Count}, want=1");
        }

        if (objects[0].Type() != ObjectType.ARRAY)
        {
            return CreateError($"argument to 'rest' must be ARRAY, got {objects[0].Type()}");
        }

        var array = (ArrayObject)objects[0];
        if (array.Elements.Length > 0)
        {
            return new ArrayObject(array.Elements.Skip(1).ToArray());
        }

        return NULL;
    }

    private static IObject PushBuiltInFunction(List<IObject>? objects)
    {
        if (objects == null)
        {
            return CreateError($"wrong number of arguments. got=0, want=1");
        }

        if (objects.Count != 2)
        {
            return CreateError($"wrong number of arguments. got={objects.Count}, want=2");
        }

        if (objects[0].Type() != ObjectType.ARRAY)
        {
            return CreateError($"argument to 'push' must be ARRAY, got {objects[0].Type()}");
        }

        var array = (ArrayObject)objects[0];
        var elements = array.Elements.ToList();
        elements.Add(objects[1]);

        return new ArrayObject(elements.ToArray());
    }
}