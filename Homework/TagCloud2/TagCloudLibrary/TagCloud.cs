using SkiaSharp;
using TagCloudLibrary.Layouter;
using TagCloudLibrary.Preprocessor;
using TagCloudLibrary.ResultPattern;
using TagCloudLibrary.Visualizer;

namespace TagCloudLibrary;

public class TagCloud(IWordPreprocessor preprocessor, ICloudLayouter layouter, ITagCloudVisualizer visualizer, TagCloudOptions options)
{
	private readonly List<PlacedText> cloud = [];

	public Result<None> BuildTagTree(IEnumerable<string> words)
    {
       return preprocessor
            .Process(words)
            .Then(r => r
                .GroupBy(t => t, (key, elements) => (Word: key, Count: elements.Count()))
                .ToList())
            .Then(CreateTagsFromWords);
    }

    private Result<None> CreateTagsFromWords(List<(string Word, int Count)> preparedWords)
    {
        var fontCoeff = (float)(options.MaxFontSize - options.MinFontSize) / (preparedWords.Max(t => t.Count) - 1f);

        foreach (var group in preparedWords)
        {
            var fontSize = options.MinFontSize + fontCoeff * (group.Count - 1);
            var font = new SKFont(options.Typeface ?? SKTypeface.Default, fontSize);
            font.MeasureText(group.Word, out var textMeasurement);
            var rectangle = layouter.PutNextRectangle(textMeasurement.Size + new SKSize(options.TextGap * 2, options.TextGap * 2));
            if (rectangle.IsSuccess)
                cloud.Add(new PlacedText(group.Word, font, rectangle.Value));
            else
                return Result.Fail<None>(rectangle.Error);
        }

        return Result.Ok();
    }

    public Result<SKImage> CreateImage() => visualizer.DrawTagCloud(cloud);
}
