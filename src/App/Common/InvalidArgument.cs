namespace App.Common;

public class InvalidArgument(string message) : AppException(message) { }
