namespace BlinkyVision
{
    public static class AppSettings
    {
        public static IConfigurationRoot GetConfiguration()
        {
            var configurationBuilder = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetParent(AppContext.BaseDirectory)!.FullName)
                        .AddJsonFile("appsettings.json", false);

            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                configurationBuilder.AddJsonFile("appsettings.Development.json");
            }
            return configurationBuilder.Build();
        }
    }
}
