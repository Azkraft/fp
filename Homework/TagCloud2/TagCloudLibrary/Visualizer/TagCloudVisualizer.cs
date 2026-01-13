using SkiaSharp;
using TagCloudLibrary.Layouter;

namespace TagCloudLibrary.Visualizer;

public class TagCloudVisualizer(TagCloudVisualizerOptions options) : ITagCloudVisualizer
{
	public SKImage DrawTagCloud(IEnumerable<PlacedText> cloud)
	{
		var tagCloud = cloud.ToList();
		var boundingRectangle = GetBoundingRectangle(tagCloud.Select(t => t.Place));
		var pictureOrigin = boundingRectangle.Location - new SKSize(options.PictureBorderSize, options.PictureBorderSize);

		var imageInfo = new SKImageInfo(
			width: (int)Math.Round(boundingRectangle.Width + 2 * options.PictureBorderSize),
			height: (int)Math.Round(boundingRectangle.Height + 2 * options.PictureBorderSize),
			colorType: SKColorType.Rgb888x,
			alphaType: SKAlphaType.Opaque);
		using var surface = SKSurface.Create(imageInfo);
		var canvas = surface.Canvas;

		canvas.Clear(options.BackgroundColor);
		DrawWords(tagCloud, pictureOrigin, canvas);

		return surface.Snapshot();
	}

	private static SKRect GetBoundingRectangle(IEnumerable<SKRect> rects)
	{
		var result = default(SKRect?);

		foreach (var rectangle in rects)
			result = SKRect.Union(result ?? rectangle, rectangle);

		return result.Value;
	}

	private static SKColor GetRandomColor()
	{
		var rand = new Random();

		var color = new byte[3];
		rand.NextBytes(color);

		var lineColor = new SKColor(
			red: color[0],
			green: color[1],
			blue: color[2]);

		return lineColor;
	}

	private void DrawWords(List<PlacedText> tagCloud, SKPoint pictureOrigin, SKCanvas canvas)
	{
		foreach (var word in tagCloud)
		{
			var paint = new SKPaint
			{
				Color = options.ForegroundColor ?? GetRandomColor(),
				IsAntialias = true,
				Style = SKPaintStyle.Fill,
			};

			word.Font.MeasureText(word.Text, out var textMeasurement, paint);

			canvas.DrawText(
				word.Text,
				word.Place.Location.X + word.Place.Width / 2 - textMeasurement.Width / 2 - pictureOrigin.X,
				word.Place.Location.Y + word.Place.Height / 2 + textMeasurement.Height / 2 - pictureOrigin.Y,
				word.Font,
				paint);
		}
	}
}
