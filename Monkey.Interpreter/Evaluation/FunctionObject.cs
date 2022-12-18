namespace Monkey.Interpreter.Evaluation;

public class FunctionObject : IObject
{
    public List<IdentifierExpression> Parameters { get; }

    public BlockStatement Body { get; }

    public Environment Environment { get; }

    public FunctionObject (List<IdentifierExpression> parameters, BlockStatement body, Environment environment)
    {
        Parameters = parameters;
        Body = body;
        Environment = environment;
    }

    public ObjectType Type() => ObjectType.FUNCTION;

    public string Inspect()
    {
        var parameters = Parameters.Select(p => p.ToString());

        return $"fn({string.Join(", ", parameters)}) {{\n{Body}\n}}";
    }    
}