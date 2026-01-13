using SkiaSharp;
using TagCloudLibrary.Layouter;

namespace TagCloudLibrary.Visualizer;

public interface ITagCloudVisualizer
{
	SKImage DrawTagCloud(IEnumerable<PlacedText> tagCloud);
}
