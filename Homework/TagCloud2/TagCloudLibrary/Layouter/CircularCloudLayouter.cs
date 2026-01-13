using SkiaSharp;
using static System.Math;

namespace TagCloudLibrary.Layouter;

public class CircularCloudLayouter(SKPoint center) : ICloudLayouter
{
    private const double radiusStep = 1;
    private const double angleStep = .01;
    private double radius = 0;
    private double angle = 0;
    private readonly List<SKRect> placedRectangles = [];

    public SKRect PutNextRectangle(SKSize rectangleSize)
    {
        if (rectangleSize.Width <= 0 || rectangleSize.Height <= 0)
            throw new ArgumentException("Recatngle size should be greater then zero", nameof(rectangleSize));

        var rectangle = FindRectangleWithCorrectPosition(rectangleSize);
        placedRectangles.Add(rectangle);
        return rectangle;
    }

    private SKRect FindRectangleWithCorrectPosition(SKSize size)
    {
        if (radius == 0)
        {
            radius += radiusStep;
            return SKRect.Create(center - new SKSize(size.Width / 2, size.Height / 2), size);
        }

        while (!CanPlaceRectangle(CreateRectangleAwayFromCenter(center, angle, radius, size)))
        {
            angle += angleStep;

            if (angle > 2 * PI)
            {
                angle = 0;
                radius += radiusStep;
            }
        }

        return PullRectangleToCenter(size);
    }

    private bool CanPlaceRectangle(SKRect rectangle)
    {
        foreach (var placedRectangle in placedRectangles)
            if (rectangle.IntersectsWith(placedRectangle))
                return false;

        return true;
    }

    /// <summary>
    /// Pulls a rectangle toward the center of the cloud until it is centered (radius + circumscribingCircleRadius = 0) or intersects with another rectangle.
    /// </summary>
    private SKRect PullRectangleToCenter(SKSize size)
    {
        var currentRadius = radius;
        var circumscribingCircleRadius = GetCircumscribingCircleRadius(size);

        while (currentRadius > -circumscribingCircleRadius
            && CanPlaceRectangle(CreateRectangleAwayFromCenter(center, angle, currentRadius, size)))
            currentRadius -= radiusStep;

        currentRadius += radiusStep;
        return CreateRectangleAwayFromCenter(center, angle, currentRadius, size);
    }

    /// <summary>
    /// The method creates a rectangle at a distance from the cloud center to the circumscribed circle of a rectangle.
    /// </summary>
    private static SKRect CreateRectangleAwayFromCenter(SKPoint center, double angle, double distance, SKSize size)
    {
        var circumscribingCircleRadius = GetCircumscribingCircleRadius(size);
        var location = CreatePointAwayFromCenter(center, angle, distance + circumscribingCircleRadius) - new SKSize(size.Width / 2, size.Height / 2);

        return SKRect.Create(location, size);
    }

    private static double GetCircumscribingCircleRadius(SKSize size)
        => Sqrt(
            size.Width / 2 * (size.Width / 2)
            + size.Height / 2 * (size.Height / 2));

    private static SKPoint CreatePointAwayFromCenter(SKPoint center, double angle, double distance)
        => new(
            center.X + (float)(distance * Cos(angle)),
            center.Y + (float)(distance * Sin(angle)));
}
