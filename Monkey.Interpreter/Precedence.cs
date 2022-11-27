﻿namespace Monkey.Interpreter;

public enum Precedence
{
    _ = 0,
    LOWEST = 1,
    EQUALS = 2,             // ==     
    LESS_OR_GREATER = 3,    // < or >
    SUM = 4,                // +
    PRODUCT = 5,            // *
    PREFIX = 6,             // -x or !x
    CALL = 7                // myFunction(x)
}