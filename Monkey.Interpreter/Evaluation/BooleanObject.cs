namespace Monkey.Interpreter.Evaluation;

public class BooleanObject : IObject
{
    public bool Value { get; }

    public BooleanObject(bool value)
    {
        Value = value;
    }

    public ObjectType Type() => ObjectType.BOOLEAN;

    public string Inspect() => Value.ToString().ToLower();
}