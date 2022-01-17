namespace BlinkyVision
{
    public class CustomVisionSettings
    {
        public const string SettingsKey = "AzureCustomVision";
        public string? Endpoint { get; set; }
        public string? Key { get; set; }
        public string? Iteration { get; set; }
        public Guid? ProjectId { get; set; }
    }
}
