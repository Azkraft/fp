namespace TagCloudLibrary.Preprocessor;

public interface IWordPreprocessor
{
	List<string> Process(IEnumerable<string> words);
}
