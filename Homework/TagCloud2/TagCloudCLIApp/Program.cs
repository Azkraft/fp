using Autofac;
using CommandLine;
using SkiaSharp;
using TagCloudLibrary;
using TagCloudLibrary.Layouter;
using TagCloudLibrary.Preprocessor;
using TagCloudLibrary.ResultPattern;
using TagCloudLibrary.Visualizer;

namespace TagCloudCLIApp;

internal class Program
{
	private static IContainer Container { get; set; }

	static void Main(string[] args)
	{
		Parser.Default.ParseArguments<CommandLineArguments>(args)
			.WithParsed(Run)
			.WithNotParsed(HandleParseError);
	}

	private static void Run(CommandLineArguments args)
	{
		SetUpContainer(args)
            .Then(_ => Result.Of(() => GetWordsFromFile(args.WordsFilePath)))
            .Then(GetTagCloudImage)
            .Then(r =>
		        SaveTagCloudImage(
			        args.ImageFilePath,
			        r,
			        args.ImageFormat))
            .OnFail(e => Console.Error.WriteLine($"Critical Error: {e}"));
	}

	private static Result<None> SetUpContainer(CommandLineArguments args)
    {
        var builder = new ContainerBuilder();
        return RegisterWordPreprocessorOptions(args, builder)
            .Then(_ => Result.OfAction(() => RegisterTagCloudOptions(args, builder)))
            .Then(_ => RegisterTagCloudVisualizerOptions(args, builder))
            .Then(_ =>
            {

                builder.RegisterType<WordPreprocessor>().As<IWordPreprocessor>();
                builder.Register(c => new CircularCloudLayouter(new())).As<ICloudLayouter>();
                builder.RegisterType<TagCloudVisualizer>().As<ITagCloudVisualizer>();
                builder.RegisterType<TagCloud>().AsSelf();

                Container = builder.Build();
            });
    }

    private static Result<None> RegisterTagCloudVisualizerOptions(CommandLineArguments args, ContainerBuilder builder)
    {
        if (!SKColor.TryParse(args.BackgroudColor, out var backgroundColor))
            return Result.Fail<None>("Incorrect background color.");

        if (!SKColor.TryParse(args.ForegroundColor, out var foregroundColor))
            return Result.Fail<None>("Incorrect foreground color.");

        var tagCloudVisualizerOptions = new TagCloudVisualizerOptions(
                    args.ImageWidth,
                    args.ImageHeight,
                    args.PictureBorderSize,
                    backgroundColor,
                    args.ForegroundColor is null ? null : foregroundColor);
        builder.RegisterInstance(tagCloudVisualizerOptions);

        return Result.Ok();
    }

    private static void RegisterTagCloudOptions(CommandLineArguments args, ContainerBuilder builder)
    {
        var tagCloudOptions = new TagCloudOptions(
                    args.FontFamilyName is null ? null : SKTypeface.FromFamilyName(args.FontFamilyName),
                    args.MinFontSize,
                    args.MaxFontSize,
                    args.TextGap);
        builder.RegisterInstance(tagCloudOptions);
    }

    private static Result<None> RegisterWordPreprocessorOptions(CommandLineArguments args, ContainerBuilder builder)
    {
        var partsOfSpeech = GetAndValidatePartsOfSpeech(args.SelectedPartsOfSpeech);
        var uniquePartsOfSpeech = new HashSet<PartOfSpeech>();
        foreach (var partOfSpeech in partsOfSpeech)
        {
            if (partOfSpeech.IsSuccess)
                uniquePartsOfSpeech.Add(partOfSpeech.GetValueOrThrow());
            else
                Console.WriteLine($"Warn: {partOfSpeech.Error}.");
        }

        var wordPreprocessorOptions = new WordPreprocessorOptions(uniquePartsOfSpeech);
        builder.RegisterInstance(wordPreprocessorOptions);

        return Result.Ok();
    }

    private static void HandleParseError(IEnumerable<Error> errs)
	{
		if (errs.IsVersion())
		{
			Console.WriteLine("Version Request");
			return;
		}

		if (errs.IsHelp())
		{
			Console.WriteLine("Help Request");
			return;
		}
		Console.WriteLine("Parser Fail");
	}

	private static IEnumerable<Result<PartOfSpeech>> GetAndValidatePartsOfSpeech(IEnumerable<string> partsOfSpeech)
	{
		foreach (var partOfSpeech in partsOfSpeech)
			yield return Enum.TryParse<PartOfSpeech>(partOfSpeech, out var result)
				? result
				: Result.Fail<PartOfSpeech>(
					$"The part of speech ({partOfSpeech}) is not one of the defined parts of speech.");
	}

	private static Result<SKImage> GetTagCloudImage(string[] words)
	{
		using var scope = Container.BeginLifetimeScope();
		var cloud = scope.Resolve<TagCloud>();

		return cloud
            .BuildTagTree(words)
            .Then(_ => cloud.CreateImage());
	}

	private static string[] GetWordsFromFile(string path) => File.ReadAllText(path).Split();

	private static Result<None> SaveTagCloudImage(string path, SKImage image, string format)
	{
        if (!Enum.TryParse<SKEncodedImageFormat>(format, out var imageFormat))
            return Result.Fail<None>("Incorrect image encode format.");

		using var data = image.Encode(imageFormat, 100);
		using var stream = File.OpenWrite(path);
        if (data is null)
            return Result.Fail<None>($"Can't encode bitmap into format {format}.");

		data.SaveTo(stream);
        return Result.Ok();
	}
}
