namespace Monkey.Interpreter;

public class Lexer
{
    private readonly string _input;

    //  current position in input (points to current char)
    private int _position;

    // current reading position in input (after current char)
    private int _readPosition;

    // current character under examination
    private char _character;

    public Lexer(string input)
    {
        _input = input;

        ReadCharacter();
    }

    public void ReadCharacter()
    {
        if (_readPosition >= _input.Length)
        {
            _character = default;
        }
        else
        {
            _character = _input[_readPosition];
        }

        _position = _readPosition;
        _readPosition++;
    }

    public Token GetNextToken()
    {
        var character = _character.ToString();

        Token token;         
        switch(_character)
        {
            case '=':
                token = new Token(new TokenType(Constants.ASSIGNMENT), character);
                break;
            case '+':
                token = new Token(new TokenType(Constants.PLUS), character);
                break;
            case '(':
                token = new Token(new TokenType(Constants.LEFT_PARENTHESIS), character);
                break;
            case ')':
                token = new Token(new TokenType(Constants.RIGHT_PARENTHESIS), character);
                break;
            case '{':
                token = new Token(new TokenType(Constants.LEFT_BRACE), character);
                break;
            case '}':
                token = new Token(new TokenType(Constants.RIGHT_BRACE), character);
                break;
            case ',':
                token = new Token(new TokenType(Constants.COMMA), character);
                break;
            case ';':
                token = new Token(new TokenType(Constants.SEMICOLON), character);
                break;
            default:
                token = new Token(new TokenType(Constants.EOF), character);
                break;
        }

        ReadCharacter();

        return token;
    }
}