namespace Monkey.Interpreter.Evaluation;

public class ReturnValueObject : IObject
{
    public IObject Value { get; }

    public ReturnValueObject(IObject value)
    {
        Value = value;
    }

    public ObjectType Type()
    {
        return ObjectType.RETURN_VALUE;
    }

    public string Inspect()
    {
        return Value.Inspect();
    }
}