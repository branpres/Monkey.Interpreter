namespace Monkey.Interpreter.Evaluation;

public class Environment
{
    private Dictionary<string, IObject?> Store { get; } = new();

    public IObject? Get(string name)
    {
        if (Store.TryGetValue(name, out var @object))
        {
            return @object;
        }

        return null;
    }

    public void Set(string name, IObject? @object) => Store[name] = @object;
}