using App.Common.Logic;
using Shouldly;

namespace Tests.Common.UnitTests;

public class SlugGeneratorTests
{
	[Fact]
	public async Task TestGenerateShouldReturnBaseSlugWhenNoCollision()
	{
		Func<string, Task<bool>> exists = _ => Task.FromResult(false);
		var title = "Test title";

		var result = await Run(exists, title);

		result.ShouldBe("test-title");
	}

	[Fact]
	public async Task TestGenerateShouldReturnSlugWithNumberWhenCollision()
	{
		int calls = 0;
		Func<string, Task<bool>> exists = _ => Task.FromResult(calls++ < 2);
		var title = "Test title";

		var result = await Run(exists, title);

		result.ShouldBe("test-title-2");
	}

	[Theory]
	[InlineData("Test Title", "test-title")]
	[InlineData("Test@ Title!", "test-title")]
	[InlineData("Tęśt TĄg !!! .", "test-tag")]
	[InlineData("   Test   Title   ", "test-title")]
	[InlineData("Test-Title", "test-title")]
	[InlineData("Test_Title", "test_title")]
	[InlineData("Test--Title", "test-title")]
	[InlineData("123 Test Title", "123-test-title")]
	[InlineData("Test  -  _  Title", "test-title")]
	[InlineData("Hello World!", "hello-world")]
	public async Task testGenerateShouldGenerateCorrectSlugs(
		string inputTitle,
		string expectedSlug
	)
	{
		var slug = await Run(title: inputTitle, exists: _ => Task.FromResult(false));

		slug.ShouldBe(expectedSlug);
	}

	private Task<string> Run(
		Func<string, Task<bool>>? exists,
		string title = "Test title"
	)
	{
		if (exists is null)
		{
			exists = _ => Task.FromResult(true);
		}
		var generator = SlugGenerator.CreateNull();
		return generator.Generate(title, exists);
	}
}
