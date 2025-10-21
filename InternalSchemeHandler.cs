using CefSharp;

namespace HTMLMCPSandbox
{
    public class InternalSchemeHandlerFactory : ISchemeHandlerFactory
    {
        public IResourceHandler Create(IBrowser browser, IFrame frame, string schemeName, IRequest request)
        {
            return new InternalSchemeHandler();
        }
    }

    public class InternalSchemeHandler : ResourceHandler
    {
        private static readonly string Root = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "_internal")
        );

        public override CefReturnValue ProcessRequestAsync(IRequest request, ICallback callback)
        {
            Task.Run(() =>
            {
                using (callback)
                {
                    try
                    {
                        var uri = new Uri(request.Url);
                        var relativePath = uri.AbsolutePath.TrimStart('/');
                        var requestedPath = Path.GetFullPath(Path.Combine(Root, relativePath));

                        if (!requestedPath.StartsWith(Root, StringComparison.OrdinalIgnoreCase))
                        {
                            StatusCode = 404;
                            callback.Continue();
                            return;
                        }

                        if (File.Exists(requestedPath))
                        {
                            var bytes = File.ReadAllBytes(requestedPath);
                            Stream = new MemoryStream(bytes);
                            MimeType = GetMimeTypeForExtension(Path.GetExtension(requestedPath));
                            StatusCode = 200;
                        }
                        else
                        {
                            StatusCode = 404;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[internal://] error: {ex.Message}");
                        StatusCode = 404;
                    }
                    finally
                    {
                        callback.Continue();
                    }
                }
            });

            return CefReturnValue.ContinueAsync;
        }

        private static string GetMimeTypeForExtension(string extension) => extension.ToLowerInvariant() switch
        {
            ".html" => "text/html",
            ".htm" => "text/html",
            ".css" => "text/css",
            ".js" => "application/javascript",
            ".json" => "application/json",
            ".png" => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".gif" => "image/gif",
            ".svg" => "image/svg+xml",
            ".woff" => "font/woff",
            ".woff2" => "font/woff2",
            _ => "application/octet-stream"
        };
    }

}
