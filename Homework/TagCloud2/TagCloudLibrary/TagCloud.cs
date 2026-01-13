using SkiaSharp;
using TagCloudLibrary.Layouter;
using TagCloudLibrary.Preprocessor;
using TagCloudLibrary.Visualizer;

namespace TagCloudLibrary;

public class TagCloud(IWordPreprocessor preprocessor, ICloudLayouter layouter, ITagCloudVisualizer visualizer, TagCloudOptions options)
{
	private readonly List<PlacedText> cloud = [];

	public void BuildTagTree(IEnumerable<string> words)
	{
		var preparedWords = preprocessor
			.Process(words)
			.GroupBy(t => t, (key, elements) => new { Word = key, Count = elements.Count() })
			.ToList();

		var fontCoeff = (float)(options.MaxFontSize - options.MinFontSize) / (preparedWords.Max(t => t.Count) - 1f);

		foreach (var group in preparedWords)
		{
			var fontSize = options.MinFontSize + fontCoeff * (group.Count - 1);
			var font = new SKFont(options.Typeface ?? SKTypeface.Default, fontSize);
			font.MeasureText(group.Word, out var textMeasurement);
			var rectangle = layouter.PutNextRectangle(textMeasurement.Size + new SKSize(options.TextGap * 2, options.TextGap * 2));
			cloud.Add(new PlacedText(group.Word, font, rectangle));
		}
	}

	public SKImage CreateImage() => visualizer.DrawTagCloud(cloud);
}
