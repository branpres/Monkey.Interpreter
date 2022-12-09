namespace Monkey.Interpreter.Evaluator;

public interface IObject
{
    ObjectType Type();

    string Inspect();
}