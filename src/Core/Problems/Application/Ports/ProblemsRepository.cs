using Core.Problems.Entities;

namespace Core.Problems.Application.Ports;

public interface ProblemsRepository
{
	public Task<bool> Exists(string slug);
	public Task<Problem?> Find(string slug);
	public Task<List<Problem>> ListAll();
	public Task Create(Problem problem);
	public Task Update(Problem problem);
	public Task Delete(Problem problem);
}
