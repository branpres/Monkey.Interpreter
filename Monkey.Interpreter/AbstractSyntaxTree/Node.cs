namespace Monkey.Interpreter.AbstractSyntaxTree;

public abstract class Node : INode
{
    public Token Token { get; }

    public Node(Token token)
    {
        Token = token;
    }

    public string GetTokenLiteral()
    {
        return Token.Literal;
    }

    public abstract override string ToString();
}