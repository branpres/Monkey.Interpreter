namespace Monkey.Interpreter.Evaluation;

public class BuiltInObject : IObject
{
    public Func<List<IObject?>?, IObject> BuiltInFunction { get; }

    public BuiltInObject(Func<List<IObject?>?, IObject> builtInFunction)
    {
        BuiltInFunction = builtInFunction;
    }

    public ObjectType Type() => ObjectType.BUILTIN;

    public string Inspect() => "builtin function";
}