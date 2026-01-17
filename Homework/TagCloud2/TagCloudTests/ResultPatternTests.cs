using FakeItEasy;
using FluentAssertions;
using SkiaSharp;
using TagCloudLibrary;
using TagCloudLibrary.Layouter;
using TagCloudLibrary.Preprocessor;
using TagCloudLibrary.ResultPattern;
using TagCloudLibrary.Visualizer;

namespace TagCloudTests;

[TestFixture]
internal class ResultPatternTests
{
    [TestCase(0, 1)]
    [TestCase(1, 0)]
    [TestCase(0, 0)]
    public void CloudLayouter_ShouldFail_WhenSizeHasNonPositiveComponent(int width, int height)
    {
        var layouter = new CircularCloudLayouter(new());
        var size = new SKSize(width, height);

        var result = layouter.PutNextRectangle(size);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Recatngle size should be greater then zero.");
    }

    [TestCase(119, 119)]
    [TestCase(0, 0)]
    [TestCase(119, 121)]
    [TestCase(121, 119)]
    public void CloudVisualizer_ShouldFail_WhenTagCloudWithBorderDoNotFitOnImage(int width, int height)
    {
        var options = new TagCloudVisualizerOptions(width, height, 20, SKColor.Parse("#000000"), null);
        var visualizer = new TagCloudVisualizer(options);
        var placedText = new PlacedText(null, new SKFont(), new SKRect(0, 0, 100, 100));

        var result = visualizer.DrawTagCloud([placedText]);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("The tag cloud extends beyond the selected image dimensions.");
    }

    [Test]
    public void TagCloud_Should_PopulatePreprocessorFails()
    {
        var preprocessor = A.Fake<IWordPreprocessor>();
        var words = new[] { "мама" };
        A.CallTo(() => preprocessor.Process(words)).Returns(Result.Fail<List<string>>(""));

        var layouter = A.Fake<ICloudLayouter>();
        var visualizer = A.Fake<ITagCloudVisualizer>();
        var options = new TagCloudOptions(SKTypeface.Default, 20, 100, 12);
        var tagCloud = new TagCloud(preprocessor, layouter, visualizer, options);

        var result = tagCloud.BuildTagTree(words);

        result.IsSuccess.Should().BeFalse();
    }

    [Test]
    public void TagCloud_Should_PopulateLayouterFails()
    {
        var preprocessor = A.Fake<IWordPreprocessor>();
        var words = new[] { "мама" };
        A.CallTo(() => preprocessor.Process(words)).Returns(Result.Ok<List<string>>(["мама"]));

        var layouter = A.Fake<ICloudLayouter>();
        var size = new SKSize(20, 20);
        A.CallTo(() => layouter.PutNextRectangle(size)).WithAnyArguments().Returns(Result.Fail<SKRect>(""));

        var visualizer = A.Fake<ITagCloudVisualizer>();
        var options = new TagCloudOptions(SKTypeface.Default, 20, 100, 12);
        var tagCloud = new TagCloud(preprocessor, layouter, visualizer, options);

        var result = tagCloud.BuildTagTree(words);

        result.IsSuccess.Should().BeFalse();
    }

    [Test]
    public void TagCloud_Should_PopulateVisualizerFails()
    {
        var preprocessor = A.Fake<IWordPreprocessor>();
        var layouter = A.Fake<ICloudLayouter>();

        var visualizer = A.Fake<ITagCloudVisualizer>();
        A.CallTo(() => visualizer.DrawTagCloud(null!)).WithAnyArguments().Returns(Result.Fail<SKImage>(""));

        var options = new TagCloudOptions(SKTypeface.Default, 20, 100, 12);
        var tagCloud = new TagCloud(preprocessor, layouter, visualizer, options);

        var result = tagCloud.CreateImage();

        result.IsSuccess.Should().BeFalse();
    }
}
