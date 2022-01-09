using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Microsoft.Extensions.Options;

namespace BlinkyVision
{
    public class ImageClassifier
    {
        private readonly CustomVisionSettings _settings;
        public ImageClassifier(IOptions<CustomVisionSettings> settings)
        {
            _settings = settings.Value;
            if (_settings.Endpoint == null || _settings.Key == null)
            {
                throw new ApplicationException("Invalid settings");
            }
        }
        public async Task<IEnumerable<FrameInfo>> Classify(string folder)
        {
            var client = AuthenticatePrediction(_settings.Endpoint!, _settings.Key!);
            var result = await ClassifyImagesAsync(client, folder);

            CleanOutputDirectory(folder);
            return result;
        }

        private static void CleanOutputDirectory(string folder)
        {
            foreach(var file in Directory.GetFiles(folder).Where(f => f.EndsWith(".png") || f.EndsWith(".mp4")))
            {
                File.Delete(file);
            }
        }

        private static CustomVisionPredictionClient AuthenticatePrediction(string endpoint, string predictionKey)
        {
            // Create a prediction endpoint, passing in the obtained prediction key
            var predictionApi = new CustomVisionPredictionClient(new ApiKeyServiceClientCredentials(predictionKey))
            {
                Endpoint = endpoint
            };
            return predictionApi;
        }
        private static async Task<IEnumerable<FrameInfo>> ClassifyImagesAsync(CustomVisionPredictionClient client, string folder)
        {
            var rv = new List<FrameInfo>();
            var pngFiles = Directory.GetFiles(folder).Where(f => f.EndsWith(".png"));
            var totalFiles = pngFiles.Count();
            Console.WriteLine($"Start processing {totalFiles} images. This may take several minutes");
            foreach (var file in pngFiles)
            {
                using var stream = File.OpenRead(file);
                var result = await client.ClassifyImageAsync(Guid.Parse("70212ba0-beb0-45d1-970c-b055395b5103"), "Iteration3", stream);
                var number = new string(file.Where(c => char.IsDigit(c)).ToArray());
                var frameNumber = int.Parse(number);
                var tagname = result.Predictions.MaxBy(model => model.Probability)?.TagName;

                if (tagname != null)
                {
                    rv.Add(new FrameInfo(frameNumber, tagname));
                }

                Console.WriteLine($"Progress: {frameNumber} of {totalFiles}");
            }
            return rv;
        }
    }
}
