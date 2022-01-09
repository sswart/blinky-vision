using BlinkyVision;
using FFMpegCore;
using FFMpegCore.Enums;
using FFMpegCore.Pipes;
using FluentAssertions;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class UnitTest1
    {
        private const string ImageOutputFolder = "ffoutput";
        

        [Fact]
        public void VideoToImagesTest()
        {
            var fileName = "./test.mp4";
            FramesExtractor.ExtractFrames(fileName, ImageOutputFolder);
            
            var files = Directory.GetFiles(ImageOutputFolder);
            files.Should().HaveCountGreaterThan(5);
        }

       

        [Fact]
        public async Task ClassifyImages()
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
            var imageCount = Directory.GetFiles(ImageOutputFolder).Where(f => f.EndsWith(".png")).Count();
            var settings = AppSettings.GetConfiguration().GetSection(CustomVisionSettings.SettingsKey);
            var customVisionSettings = new CustomVisionSettings { Key = settings["Key"], Endpoint = settings["Endpoint"] };
            var classifier = new ImageClassifier(Options.Create(customVisionSettings));
            var results = await classifier.Classify(ImageOutputFolder);

            results.Should().HaveCount(imageCount);

            var json = JsonSerializer.Serialize(results);

            if (File.Exists("result.json"))
            {
                File.Delete("result.json");
            }
            await File.WriteAllTextAsync("result.json", json);
        }

        [Fact]
        public async Task MorseFromFrameInfo()
        {
            var result = await JsonSerializer.DeserializeAsync<FrameInfo[]>(File.OpenRead("result.json"));

            var messageString = MorseParser.Translate(result!);

            messageString.ToLower().Should().Contain("takemeplaces");
        }
    }
}