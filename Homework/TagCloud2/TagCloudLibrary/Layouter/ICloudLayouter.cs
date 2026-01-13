using SkiaSharp;
using System.Drawing;

namespace TagCloudLibrary.Layouter;

public interface ICloudLayouter
{
	SKRect PutNextRectangle(SKSize rectangleSize);
}
