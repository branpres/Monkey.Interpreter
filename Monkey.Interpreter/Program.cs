using Monkey.Interpreter;

Console.WriteLine("Welcome to the Monkey programming language!");
Console.WriteLine("Type any comand:\n");

string? line;
do
{
    Console.Write(">> ");
    line = Console.ReadLine();
    if (line != null)
    {
        var lexer = new Lexer(line);
        Token token;
        do
        {
            token = lexer.GetNextToken();
            Console.WriteLine(token.ToString());
        }
        while (token.TokenType.Value != Constants.EOF);
    }
}
while (line != "exit");