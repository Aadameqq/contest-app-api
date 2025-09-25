using Core.Problems.Entities;

namespace Core.Problems.Application.Ports;

public interface TagsRepository
{
	public Task<bool> Exists(string slug);
	public Task<Tag?> Find(string slug);
	public Task<List<Tag>> ListAll();
	public Task Create(Tag tag);
	public Task Update(Tag tag);
	public Task Delete(Tag tag);
}
