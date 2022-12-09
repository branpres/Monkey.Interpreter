namespace Monkey.Interpreter.Evaluation;

public interface IObject
{
    ObjectType Type();

    string Inspect();
}