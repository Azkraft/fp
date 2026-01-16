using System.Diagnostics;
using System.Text.Json;
using TagCloudLibrary.ResultPattern;

namespace TagCloudLibrary.Preprocessor;

public class WordPreprocessor(WordPreprocessorOptions options) : IWordPreprocessor
{
	public Result<List<string>> Process(IEnumerable<string> words)
	{
		return GetAnalyzedWords(words)
            .Then(r => r
                .Where(t => options.SelectedPartsOfSpeech.Contains(t.PartOfSpeech))
                .Select(t => t.Word.ToLower())
                .ToList());
	}

	private static Result<List<WordWithPartOfSpeech>> GetAnalyzedWords(IEnumerable<string> words)
    {
        var inputFilePath = Path.Combine(AppContext.BaseDirectory, Path.GetRandomFileName());
        var outputFilePath = Path.Combine(AppContext.BaseDirectory, Path.GetRandomFileName());

        return Result
            .OfAction(
                () => PrepareInputFile(inputFilePath, words),
                $"Can't prepare input file {inputFilePath} for mystem.exe")
            .Then(_ => Result.OfAction(
                () => PrepareOutputFile(outputFilePath),
                $"Can't prepare output file {outputFilePath} for mystem.exe"))
            .Then(_ => Result.Of(() => ProcessWords(inputFilePath, outputFilePath)))
            .Then(r => Result.Of(() => ParseProcessingResult(r)));
    }

    private static List<WordWithPartOfSpeech> ParseProcessingResult(string output)
    {
        var jsonOption = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var jsonOutput = output
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(t => JsonSerializer.Deserialize<List<WordAnalysis>>(t, jsonOption))
            .Where(t => t.Count > 0)
            .Select(t => t[0].ToWordWithPartOfSpeech())
            .ToList();
        return jsonOutput;
    }

    private static string ProcessWords(string inputFilePath, string outputFilePath)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = Path.Combine(AppContext.BaseDirectory, "mystem.exe"),
            Arguments = $"-i --format json \"{inputFilePath}\" \"{outputFilePath}\"",
            UseShellExecute = false,
            CreateNoWindow = true
        };

        string output;
        try
        {
            using var myStemProcess = new Process { StartInfo = startInfo };
            myStemProcess.Start();
            myStemProcess.WaitForExit();
            output = File.ReadAllText(outputFilePath);
        }
        finally
        {
            File.Delete(inputFilePath);
            File.Delete(outputFilePath);
        }

        return output;
    }

    private static void PrepareInputFile(string path, IEnumerable<string> words)
    {
        File.WriteAllText(path, string.Join('\n', words));
    }

    private static void PrepareOutputFile(string path)
    {
        File.Create(path).Close();
    }
}
