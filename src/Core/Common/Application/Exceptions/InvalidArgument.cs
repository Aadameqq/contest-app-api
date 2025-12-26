namespace Core.Common.Application.Exceptions;

public class InvalidArgument(string message) : AppException(message) { }
