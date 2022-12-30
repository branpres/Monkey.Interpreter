namespace Monkey.Interpreter.Evaluation;

public class Environment
{
    private Dictionary<string, IObject?> Store { get; } = new();

    private Environment? Outer { get; set; }

    public IObject? Get(string name)
    {
        if (Store.TryGetValue(name, out var @object))
        {
            return @object;
        }

        if (Outer != null)
        {
            return Outer.Get(name);
        }

        return null;
    }

    public void Set(string name, IObject? @object) => Store[name] = @object;

    public static Environment NewEnclosedEnvironment(Environment outer)
    {
        return new Environment
        {
            Outer = outer
        };
    }
}