namespace Monkey.Interpreter.Evaluator;

public class Null : IObject
{
    public ObjectType Type() => ObjectType.NULL;

    public string Inspect() => "null";
}