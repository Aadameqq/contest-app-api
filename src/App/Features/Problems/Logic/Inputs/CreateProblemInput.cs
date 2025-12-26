namespace App.Features.Problems.Logic.Inputs;

public record CreateProblemInput(string Title, List<string> TagSlugs);
