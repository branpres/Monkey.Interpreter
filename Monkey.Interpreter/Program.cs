Console.WriteLine("Welcome to the Monkey programming language!");
Console.WriteLine("Type any comand:\n");

const string MONKEY_FACE = """
            __,__
   .--.  .-"     "-.  .--.
  / .. \/  .-. .-.  \/ .. \
 | |  '|  /   Y   \  |'  | |
 | \   \  \ 0 | 0 /  /   / |
  \ '- ,\.-'''''''-./, -' /
   ''-' /_   ^ ^   _\ '-''
       |  \._   _./  |
       \   \ '~' /   /
        '._ '-=-' _.'
           '-----'
 """;

string? line;
do
{
    Console.Write(">> ");
    line = Console.ReadLine();
    if (line != null && line != "exit")
    {
        var lexer = new Lexer(line);
        var parser = new Parser(lexer);
        var program = parser.ParseProgram();

        if (parser.Errors.Any())
        {
            Console.WriteLine(MONKEY_FACE);
            Console.WriteLine("Oops! We ran into some monkey business here!");
            Console.WriteLine("\tParser errors:");
            parser.Errors.ForEach(x => Console.WriteLine($"\t{x}"));
            Console.WriteLine();
            continue;
        }

        var evaluated = Evaluator.Evaluate(program);
        if (evaluated != null)
        {
            Console.WriteLine($"{evaluated.Inspect()}\n");
        }
    }
}
while (line != "exit");