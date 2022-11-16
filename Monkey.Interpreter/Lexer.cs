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
                    token = new Token(new TokenType(Constants.EQUAL), $"{tempChar}{_character}");
                }
                else
                {
                    token = new Token(new TokenType(Constants.ASSIGNMENT), character);
                }
                break;
            case '+':
                token = new Token(new TokenType(Constants.PLUS), character);
                break;
            case '-':
                token = new Token(new TokenType(Constants.MINUS), character);
                break;
            case '!':
                if (PeekCharacter() == '=')
                {
                    var tempChar = _character;
                    ReadCharacter();
                    token = new Token(new TokenType(Constants.NOT_EQUAL), $"{tempChar}{_character}");
                }
                else
                {
                    token = new Token(new TokenType(Constants.BANG), character);
                }                
                break;
            case '*':
                token = new Token(new TokenType(Constants.ASTERISK), character);
                break;
            case '/':
                token = new Token(new TokenType(Constants.SLASH), character);
                break;
            case '<':
                token = new Token(new TokenType(Constants.LESS_THAN), character);
                break;
            case '>':
                token = new Token(new TokenType(Constants.GREATER_THAN), character);
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
            case '\0':
                token = new Token(new TokenType(Constants.EOF), character);
                break;
            default:
                if (_character.IsLetter())
                {
                    var identifier = ReadIdentifier();
                    return new Token(new TokenType(identifier.GetIdentifier()), identifier);
                }
                else if (_character.IsDigit())
                {
                    return new Token(new TokenType(Constants.INTEGER), ReadNumber());
                }
                else
                {
                    token = new Token(new TokenType(Constants.ILLEGAL), character);
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