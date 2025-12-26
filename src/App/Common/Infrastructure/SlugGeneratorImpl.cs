using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Core.Problems.Application.Ports;

namespace App.Common.Infrastructure;

public class SlugGeneratorImpl : SlugGenerator
{
	public string Generate(string title)
	{
		var slug = RemoveAccent(title.ToLowerInvariant());
		slug = Regex.Replace(slug, @"\s", "-", RegexOptions.Compiled);
		slug = Regex.Replace(slug, @"[^a-z0-9-_]", "", RegexOptions.Compiled);
		slug = slug.Trim('-', '_');
		return Regex.Replace(slug, @"([-_]){2,}", "$1", RegexOptions.Compiled);
	}

	private static string RemoveAccent(string title)
	{
		if (string.IsNullOrEmpty(title))
			return title;

		var normalized = title.Normalize(NormalizationForm.FormD);
		var sb = new StringBuilder();

		foreach (var c in normalized)
		{
			var category = CharUnicodeInfo.GetUnicodeCategory(c);
			if (category != UnicodeCategory.NonSpacingMark)
			{
				sb.Append(c);
			}
		}

		return sb.ToString().Normalize(NormalizationForm.FormC);
	}
}
