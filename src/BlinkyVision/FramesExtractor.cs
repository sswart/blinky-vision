using FFMpegCore;
using FFMpegCore.Enums;

namespace BlinkyVision
{
    public class FramesExtractor
    {
        public static void ExtractFrames(string fileName, string imageOutputFolder)
        {
            InitializeDirectory(fileName, imageOutputFolder);
            FFOptions options = SetupOptions(imageOutputFolder);
            VideoToImages(fileName, options);
        }

        private static void InitializeDirectory(string videoFile, string outputFolder)
        {
            var folder = outputFolder;

            if (Directory.Exists(folder))
            {
                Directory.Delete(folder, true);
            }

            Directory.CreateDirectory(folder);
            var fileName = Path.GetFileName(videoFile);
            File.Copy(videoFile, $"{folder}/{fileName}");
        }

        private static FFOptions SetupOptions(string outputFolder)
        {
            return new FFOptions { WorkingDirectory = outputFolder };
        }

        private static void VideoToImages(string fileName, FFOptions ffOptions)
        {
            Action<FFMpegArgumentOptions> options = args => args.Resize(1024, 576).WithVideoCodec(VideoCodec.Png).WithFramerate(10);//.ForceFormat("png");
            FFMpegArguments
                .FromFileInput(fileName)
                .OutputToFile(@"%03d.png", false, options)
                .NotifyOnOutput(Log)
                .ProcessSynchronously(true, ffOptions);
        }

        private static void Log(string value, Instances.DataType data)
        {
            Console.WriteLine(value);
        }
    }
}
