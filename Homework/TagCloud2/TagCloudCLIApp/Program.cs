using Autofac;
using CommandLine;
using SkiaSharp;
using TagCloudLibrary;
using TagCloudLibrary.Layouter;
using TagCloudLibrary.Preprocessor;
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
		SetUpContainer(args);

		var words = GetWordsFromFile(args.WordsFilePath);
		var image = GetTagCloudImage(words);
		SaveTagCloudImage(
			args.ImageFilePath,
			image,
			args.ImageWidth,
			args.ImageHeight,
			Enum.Parse<SKEncodedImageFormat>(args.ImageFormat));
	}

	private static void SetUpContainer(CommandLineArguments args)
	{
		var builder = new ContainerBuilder();

		var wordPreprocessorOptions = new WordPreprocessorOptions(
			GetAndValidatePartsOfSpeech(args.SelectedPartsOfSpeech).ToHashSet());
		builder.RegisterInstance(wordPreprocessorOptions);

		var tagCloudOptions = new TagCloudOptions(
			args.FontFamilyName is null ? null : SKTypeface.FromFamilyName(args.FontFamilyName),
			args.MinFontSize,
			args.MaxFontSize,
			args.TextGap);
		builder.RegisterInstance(tagCloudOptions);

		var tagCloudVisualizerOptions = new TagCloudVisualizerOptions(
			args.PictureBorderSize,
			SKColor.Parse(args.BackgroudColor),
			args.ForegroundColor is null ? null : SKColor.Parse(args.ForegroundColor));
		builder.RegisterInstance(tagCloudVisualizerOptions);

		builder.RegisterType<WordPreprocessor>().As<IWordPreprocessor>();
		builder.Register(c => new CircularCloudLayouter(new())).As<ICloudLayouter>();
		builder.RegisterType<TagCloudVisualizer>().As<ITagCloudVisualizer>();
		builder.RegisterType<TagCloud>().AsSelf();

		Container = builder.Build();
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

	private static IEnumerable<PartOfSpeech> GetAndValidatePartsOfSpeech(IEnumerable<string> partsOfSpeech)
	{
		foreach (var partOfSpeech in partsOfSpeech)
			yield return Enum.TryParse<PartOfSpeech>(partOfSpeech, out var result)
				? result
				: throw new ArgumentOutOfRangeException(nameof(partsOfSpeech),
					$"the part of speech ({partOfSpeech}) is not one of the defined parts of speech");
	}

	private static SKImage GetTagCloudImage(string[] words)
	{
		using var scope = Container.BeginLifetimeScope();
		var cloud = scope.Resolve<TagCloud>();
		cloud.BuildTagTree(words);
		return cloud.CreateImage();
	}

	private static string[] GetWordsFromFile(string path) => File.ReadAllText(path).Split();

	private static void SaveTagCloudImage(string path, SKImage image, int? width, int? height, SKEncodedImageFormat format)
	{
		var imageInfo = new SKImageInfo(
			width: width ?? image.Width,
			height: height ?? image.Height,
			colorType: SKColorType.Rgb888x,
			alphaType: SKAlphaType.Opaque);

		using var bitmap = SKBitmap.FromImage(image).Resize(imageInfo, SKFilterQuality.High);
		using var data = image.Encode(format, 80);
		using var stream = File.OpenWrite(path);
		if (data is null)
			throw new NullReferenceException("Can't encode bitmap");
		data.SaveTo(stream);
	}
}
