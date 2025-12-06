using System;

namespace EngLang.Vm;

public class EngLangRuntimeException : Exception
{
    public EngLangRuntimeException()
    {
    }

    public EngLangRuntimeException(string? message) : base(message)
    {
    }

    public EngLangRuntimeException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
