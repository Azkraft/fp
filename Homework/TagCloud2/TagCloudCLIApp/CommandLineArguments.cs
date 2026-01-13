using CommandLine;
using System.ComponentModel;
using TagCloudLibrary.Preprocessor;

namespace TagCloudCLIApp;

internal class CommandLineArguments
{
	[Option("min-font-size", Default = 20, Required = false, HelpText = "Set minimal font size in tag cloud.")]
	public float MinFontSize { get; set; }

	[Option("max-font-size", Default = 100, Required = false, HelpText = "Set maximal font size in tag cloud.")]
	public float MaxFontSize { get; set; }

	[Option("text-gap", Default = 12, Required = false, HelpText = "Set gap between text.")]
	public float TextGap { get; set; }

	[Option("picture-border-size", Default = 20, Required = false, HelpText = "Set border around picture.")]
	public float PictureBorderSize { get; set; }

	[Option("background-color", Default = "#000000", Required = false, HelpText = "Set backgroud color.")]
	public string BackgroudColor { get; set; }

	[Option("foreground-color", Default = null, Required = false, HelpText = "Set text color.")]
	public string? ForegroundColor { get; set; }

	[Option("font-family-name", Default = null, Required = false, HelpText = "Set font family name.")]
	public string? FontFamilyName { get; set; }

	[Option("image-width", Default = null, Required = false, HelpText = "Set image width.")]
	public int? ImageWidth { get; set; }

	[Option("image-height", Default = null, Required = false, HelpText = "Set image height.")]
	public int? ImageHeight { get; set; }

	[Option("image-format", Default = "Png", Required = false, HelpText = "Set image format.")]
	public string ImageFormat { get; set; }

	[Option(
		"selected-parts-of-speech",
		Default = new string[]
		{
			nameof(PartOfSpeech.Adjective),
			nameof(PartOfSpeech.Adverb),
			nameof(PartOfSpeech.Composite),
			nameof(PartOfSpeech.Noun),
			nameof(PartOfSpeech.Verb)
		},
		Required = false,
		HelpText = "Sets the selected of the following parts of speech: Adjective, Adverb, PronominalAdverb, NumeralAdjective, AdjectivePronoun, Composite, Conjunction, Interjection, Numeral, Particle, Pretext, Noun, PronounNoun, Verb.")]
	public IEnumerable<string> SelectedPartsOfSpeech { get; set; }

	[Value(0, MetaName = nameof(WordsFilePath), Required = true, HelpText = "Specifies a path to file in the .txt format.")]
	public string WordsFilePath { get; set; }

	[Value(1, MetaName = nameof(ImageFilePath), Required = true, HelpText = "Specifies a path to file in the .png format.")]
	public string ImageFilePath { get; set; }
}
