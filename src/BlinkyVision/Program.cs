using BlinkyVision;

var services = new ServiceCollection();

var config = AppSettings.GetConfiguration();

services.AddSingleton<IConfigurationRoot>(config);
services.Configure<CustomVisionSettings>(config.GetSection(CustomVisionSettings.SettingsKey));
services.AddSingleton<ImageClassifier>();

#pragma warning disable ASP0000 // Do not call 'IServiceCollection.BuildServiceProvider' in 'ConfigureServices'
var serviceProvider = services.BuildServiceProvider();
#pragma warning restore ASP0000 // Do not call 'IServiceCollection.BuildServiceProvider' in 'ConfigureServices'

var mp4File = Directory.GetFiles(AppContext.BaseDirectory).SingleOrDefault(file => file.EndsWith(".mp4"));
if (mp4File != null)
{
    var outputFolder = Path.Combine(AppContext.BaseDirectory, config["OutputPath"]);
    FramesExtractor.ExtractFrames(mp4File, outputFolder);
    var frames = await serviceProvider.GetRequiredService<ImageClassifier>().Classify(outputFolder);
    var result = MorseParser.Translate(frames);

    var outputFile = $"{outputFolder}/result.txt";
    await File.WriteAllTextAsync(outputFile, result);
}
else
{
    throw new ApplicationException("Place exactly one video file (mp4) in the folder.");
}