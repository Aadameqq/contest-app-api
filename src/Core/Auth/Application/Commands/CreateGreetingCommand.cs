using Core.Common.Application;

namespace Core.Auth.Application.Commands;

public record CreateGreetingCommand(string Content) : Command;
