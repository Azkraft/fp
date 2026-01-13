using System.Text.Json.Serialization;

namespace TagCloudLibrary.Preprocessor;

public class AnalysisInfo
{
	[JsonPropertyName("lex")]
	public string Lexeme { get; set; }

	[JsonPropertyName("gr")]
	public string Gramme { get; set; }
}
