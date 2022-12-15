namespace Monkey.Interpreter.Evaluation;

public class ErrorObject : IObject
{
    public string Message { get; }

    public ErrorObject(string message)
    {
        Message = message;
    }

    public ObjectType Type() => ObjectType.ERROR;

    public string Inspect() => $"Error: {Message}";
}
