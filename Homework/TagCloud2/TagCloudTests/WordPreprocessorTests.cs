using FluentAssertions;
using TagCloudLibrary.Preprocessor;

namespace TagCloudTests;

public class WordPreprocessorTests
{
	[Test]
	public void Should_ReturnSameWordCountWithoutFiltering()
	{
		var preprocessor = new WordPreprocessor(new(Enum.GetValues<PartOfSpeech>().ToHashSet()));
		var words = new List<string> { "мама", "мыла", "раму" };

		var output = preprocessor.Process(words).GetValueOrThrow();

		output.Should().HaveSameCount(words);
	}

	[Test]
	public void Should_FilterWords()
	{
		var preprocessor = new WordPreprocessor(new([]));
		var words = new List<string> { "мама", "мыла", "раму" };

		var output = preprocessor.Process(words).GetValueOrThrow();

		output.Should().HaveCount(0);
	}
}
