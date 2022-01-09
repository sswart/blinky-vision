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
    FramesExtractor.ExtractFrames(mp4File, config["OutputPath"]);
    var frames = await serviceProvider.GetRequiredService<ImageClassifier>().Classify(config["OutputPath"]);
    var result = MorseParser.Translate(frames);

    var outputFile = $"{config["OutputPath"]}/result.txt";
    await File.WriteAllTextAsync(outputFile, result);
}
else
{
    throw new ApplicationException("Place exactly one video file (mp4) in the folder.");
}