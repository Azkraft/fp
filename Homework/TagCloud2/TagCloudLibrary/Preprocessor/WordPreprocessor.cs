using System.Diagnostics;
using System.Text.Json;

namespace TagCloudLibrary.Preprocessor;

public class WordPreprocessor(WordPreprocessorOptions options) : IWordPreprocessor
{
	public List<string> Process(IEnumerable<string> words)
	{
		return GetAnalyzedWords(words)
			.Where(t => options.SelectedPartsOfSpeech.Contains(t.PartOfSpeech))
			.Select(t => t.Word.ToLower())
			.ToList();
	}

	private static List<WordWithPartOfSpeech> GetAnalyzedWords(IEnumerable<string> words)
	{
		var inputFilePath = Path.Combine(AppContext.BaseDirectory, Path.GetRandomFileName());
		var outputFilePath = Path.Combine(AppContext.BaseDirectory, Path.GetRandomFileName());

		File.WriteAllText(inputFilePath, string.Join('\n', words));
		File.Create(outputFilePath)
			.Close();

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

		var jsonOption = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
		var jsonOutput = output
			.Split('\n', StringSplitOptions.RemoveEmptyEntries)
			.Select(t => JsonSerializer.Deserialize<List<WordAnalysis>>(t, jsonOption))
			.Where(t => t.Count > 0)
			.Select(t => t[0].ToWordWithPartOfSpeech())
			.ToList();

		return jsonOutput;
	}
}
