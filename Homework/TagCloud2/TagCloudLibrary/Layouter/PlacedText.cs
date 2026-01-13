using SkiaSharp;

namespace TagCloudLibrary.Layouter;

public record class PlacedText(string Text, SKFont Font, SKRect Place);
