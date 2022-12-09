namespace Monkey.Interpreter.Evaluator;

public class Boolean : IObject
{
    public bool Value { get; }

    public Boolean(bool value)
    {
        Value = value;
    }

    public ObjectType Type() => ObjectType.BOOLEAN;

    public string Inspect() => Value.ToString();
}