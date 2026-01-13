using TagCloudLibrary.Preprocessor;

namespace TagCloudTests;

public class WordPreprocessorTests
{
	[Test]
	public void Should_ReturnSameWordCountWithoutFiltering()
	{
		var preprocessor = new WordPreprocessor(new([]));
		var words = new List<string> { "мама", "мыла", "раму" };

		var output = preprocessor.Process(words);

		Assert.That(output, Has.Count.EqualTo(words.Count));
	}

	[Test]
	public void Should_FilterWords()
	{
		var preprocessor = new WordPreprocessor(new([PartOfSpeech.Noun]));
		var words = new List<string> { "мама", "мыла", "раму" };

		var output = preprocessor.Process(words);

		Assert.That(output, Has.Count.EqualTo(0));
	}
}