namespace App.Common.Logic.Exceptions;

public class InvalidArgument(string message) : AppException(message) { }
