namespace Monkey.Interpreter.Evaluation;

public class ArrayObject : IObject
{
    public IObject[] Elements { get; }

    public ArrayObject(IObject[] elements)
    {
        Elements = elements;
    }

    public ObjectType Type() => ObjectType.ARRAY;

    public string Inspect()
    {
        return $"[{string.Join(", ", Elements.Select(x => x.Inspect()))}]";
    }
}