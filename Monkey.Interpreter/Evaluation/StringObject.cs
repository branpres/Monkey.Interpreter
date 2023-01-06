namespace Monkey.Interpreter.Evaluation;

public class StringObject : IObject
{
    public string Value { get; }

    public StringObject(string value)
    {
        Value = value;
    }

    public ObjectType Type() => ObjectType.STRING;

    public string Inspect() => Value;
}