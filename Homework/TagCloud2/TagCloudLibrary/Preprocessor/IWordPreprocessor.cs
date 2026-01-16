using TagCloudLibrary.ResultPattern;

namespace TagCloudLibrary.Preprocessor;

public interface IWordPreprocessor
{
	Result<List<string>> Process(IEnumerable<string> words);
}
