using System;
using System.Text.Json;

namespace HTMLMCPSandbox.Filtering;

public class FilteringRequestOptions
{
    public List<string> AllowedDomains { get; set; } = new List<string>();
    public List<string> BlockedDomains { get; set; } = new List<string>();
    public List<string> AllowedSchemes { get; set; } = new List<string>();
    public List<string> BlockedSchemes { get; set; } = new List<string>();
    public string LocalRootFolder { get; set; } = "_internal";
}

public static class FilteringRequestOptionsLoader
{
    private const string FileName = "requestfiltering.options.json";

    public static FilteringRequestOptions Load(bool useDefaults)
    {
        // Fallback to defaults and save them
        var result = useDefaults ? CreateDefault() : new FilteringRequestOptions();

        // If file exists, try to load it
        if (File.Exists(FileName))
        {
            try
            {
                var json = File.ReadAllText(FileName);
                var options = JsonSerializer.Deserialize<FilteringRequestOptions>(json);
                if (options != null)
                    return options;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WARN] Failed to load {FileName}: {ex.Message}");
            }
        }
        else
        {
            if (useDefaults)
                Save(result);
        }

        return result;
    }

    public static void Save(FilteringRequestOptions options)
    {
        var json = JsonSerializer.Serialize(options, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(FileName, json);
    }

    private static FilteringRequestOptions CreateDefault() => new()
    {
        AllowedDomains =
        {
                "",
                "cdnjs.cloudflare.com",
                "fonts.googleapis.com",
                "fonts.gstatic.com"
            },
        BlockedSchemes =
            {
                "file",
                "ftp",
                "chrome",
                "chrome-extension",
                "about",
                "blob",
                "http"
            },
        LocalRootFolder = "_internal"
    };
}