namespace Monkey.Interpreter.Evaluation;

public class Null : IObject
{
    public ObjectType Type() => ObjectType.NULL;

    public string Inspect() => "null";
}