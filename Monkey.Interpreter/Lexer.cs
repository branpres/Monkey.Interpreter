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

    public Token GetNextToken()
    {
        SkipWhitespace();

        var character = _character.ToString();

        Token token;         
        switch(_character)
        {
            case '=':
                if (PeekCharacter() == '=')
                {
                    var tempChar = _character;
                    ReadCharacter();
                    token = new Token(TokenType.EQUAL, $"{tempChar}{_character}");
                }
                else
                {
                    token = new Token(TokenType.ASSIGNMENT, character);
                }
                break;
            case '+':
                token = new Token(TokenType.PLUS, character);
                break;
            case '-':
                token = new Token(TokenType.MINUS, character);
                break;
            case '!':
                if (PeekCharacter() == '=')
                {
                    var tempChar = _character;
                    ReadCharacter();
                    token = new Token(TokenType.NOT_EQUAL, $"{tempChar}{_character}");
                }
                else
                {
                    token = new Token(TokenType.BANG, character);
                }                
                break;
            case '*':
                token = new Token(TokenType.ASTERISK, character);
                break;
            case '/':
                token = new Token(TokenType.SLASH, character);
                break;
            case '<':
                token = new Token(TokenType.LESS_THAN, character);
                break;
            case '>':
                token = new Token(TokenType.GREATER_THAN, character);
                break;
            case '(':
                token = new Token(TokenType.LEFT_PARENTHESIS, character);
                break;
            case ')':
                token = new Token(TokenType.RIGHT_PARENTHESIS, character);
                break;
            case '{':
                token = new Token(TokenType.LEFT_BRACE, character);
                break;
            case '}':
                token = new Token(TokenType.RIGHT_BRACE, character);
                break;
            case ',':
                token = new Token(TokenType.COMMA, character);
                break;
            case ';':
                token = new Token(TokenType.SEMICOLON, character);
                break;
            case '\0':
                token = new Token(TokenType.EOF, character);
                break;
            case '\"':
                token = new Token(TokenType.STRING, ReadString());
                break;
            case '[':
                token = new Token(TokenType.LEFT_BRACKET, character);
                break;
            case ']':
                token = new Token(TokenType.RIGHT_BRACKET, character);
                break;
            case ':':
                token = new Token(TokenType.COLON, character);
                break;
            default:
                if (_character.IsLetter())
                {
                    var identifier = ReadIdentifier();
                    return new Token(identifier.GetIdentifier(), identifier);
                }
                else if (_character.IsDigit())
                {
                    return new Token(TokenType.INTEGER, ReadNumber());
                }
                else
                {
                    token = new Token(TokenType.ILLEGAL, character);
                }
                break;
        }

        ReadCharacter();

        return token;
    }

    private void ReadCharacter()
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

    private string ReadIdentifier()
    {
        var position = _position;
        while (_character.IsLetter())
        {
            ReadCharacter();
        }

        return _input[position.._position];
    }

    private string ReadNumber()
    {
        var position = _position;
        while (_character.IsDigit())
        {
            ReadCharacter();
        }

        return _input[position.._position];
    }

    private string ReadString()
    {
        var position = _position + 1;
        ReadCharacter();
        while(_character != '\"' && _character != '\0')
        {
            ReadCharacter();
        }

        return _input[position.._position];
    }

    private void SkipWhitespace()
    {
        while (_character.IsWhitespace())
        {
            ReadCharacter();
        }
    }

    private char PeekCharacter()
    {
        if (_readPosition >= _input.Length)
        {
            return default;
        }

        return _input[_readPosition];
    }
}