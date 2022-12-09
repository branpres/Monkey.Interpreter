namespace Monkey.Interpreter.Evaluation;

public class Integer : IObject
{
    public int Value { get; }

    public Integer(int value)
    {
        Value = value;
    }

    public ObjectType Type() => ObjectType.INTEGER;

    public string Inspect() => Value.ToString();
}