namespace Monkey.Interpreter.Evaluation;

public class IntegerObject : IObject
{
    public int Value { get; }

    public IntegerObject(int value)
    {
        Value = value;
    }

    public ObjectType Type() => ObjectType.INTEGER;

    public string Inspect() => Value.ToString();
}