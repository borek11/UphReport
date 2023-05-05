using System;

namespace UphReport.Exceptions;

public class BadRequestException : Exception 
{
    public BadRequestException(string message) : base(message)
    {

    }
}
