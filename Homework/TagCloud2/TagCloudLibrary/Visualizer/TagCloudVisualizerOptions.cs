using SkiaSharp;

namespace TagCloudLibrary.Visualizer;

public record class TagCloudVisualizerOptions(int? Width, int? Height, int PictureBorderSize, SKColor BackgroundColor, SKColor? ForegroundColor);
