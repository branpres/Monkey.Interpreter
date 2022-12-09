namespace Monkey.Interpreter.Evaluation;

public class NullObject : IObject
{
    public ObjectType Type() => ObjectType.NULL;

    public string Inspect() => "null";
}