using SkiaSharp;
using TagCloudLibrary.Layouter;
using TagCloudLibrary.ResultPattern;

namespace TagCloudLibrary.Visualizer;

public interface ITagCloudVisualizer
{
	Result<SKImage> DrawTagCloud(IEnumerable<PlacedText> tagCloud);
}
