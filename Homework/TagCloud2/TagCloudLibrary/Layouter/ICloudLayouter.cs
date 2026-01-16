using SkiaSharp;
using TagCloudLibrary.ResultPattern;

namespace TagCloudLibrary.Layouter;

public interface ICloudLayouter
{
    Result<SKRect> PutNextRectangle(SKSize rectangleSize);
}
