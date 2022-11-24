namespace Monkey.Interpreter.AbstractSyntaxTree;

public interface INode
{
    string GetTokenLiteral();

    string ToString();
}