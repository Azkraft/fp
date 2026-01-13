using SkiaSharp;

namespace TagCloudLibrary;

public record class TagCloudOptions(SKTypeface? Typeface, float MinFontSize, float MaxFontSize, float TextGap);
