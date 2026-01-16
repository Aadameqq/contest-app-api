using App.Common.Logic;
using App.Features.Problems.Domain;

namespace App.Features.Problems.Logic.Ports;

public interface ProblemsRepository
{
	public Task<bool> Exists(string slug);
	public Task<Problem?> Find(string slug);
	public Task<List<Problem>> ListAll();
	public Task<Paginated<Problem>> Search(Pagination pagination);
	public Task Create(Problem problem);
	public Task Update(Problem problem);
	public Task Delete(Problem problem);
}
