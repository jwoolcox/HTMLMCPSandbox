using CefSharp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace HTMLMCPSandbox.Filtering
{
    public class FilteringResourceRequestHandler : CefSharp.Handler.ResourceRequestHandler
    {
        private FilteringRequestOptions _options;
        private ILogger<FilteringResourceRequestHandler>? _logger;

        public FilteringResourceRequestHandler(FilteringRequestOptions options, ILoggerFactory loggerFactory) : base()
        {
            _options = options;
            _logger = loggerFactory.CreateLogger<FilteringResourceRequestHandler>();
        }

        protected override IResourceHandler? GetResourceHandler(
            IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request)
        {
            LogRequest(request);

            IResourceHandler denied = ResourceHandler.ForErrorMessage(
                "Request blocked by default", System.Net.HttpStatusCode.Forbidden);

            if (!Uri.TryCreate(request.Url, UriKind.RelativeOrAbsolute, out var requestUri))
                return ResourceHandler.ForErrorMessage("Invalid request URI.", System.Net.HttpStatusCode.BadRequest);

            if (_options.BlockedSchemes.Any(s => requestUri.Scheme.Equals(s, StringComparison.OrdinalIgnoreCase)))
                return ResourceHandler.ForErrorMessage($"Blocked scheme: {requestUri.Scheme}", System.Net.HttpStatusCode.Forbidden);

            if (requestUri.IsAbsoluteUri &&
                _options.BlockedDomains.Any(d => requestUri.Host.EndsWith(d, StringComparison.OrdinalIgnoreCase)))
                return ResourceHandler.ForErrorMessage($"Blocked domain: {requestUri.Host}", System.Net.HttpStatusCode.Forbidden);

            if (requestUri.Scheme != "file" && requestUri.IsAbsoluteUri && _options.AllowedDomains.Count > 0)
            {
                if (!_options.AllowedDomains.Any(d => requestUri.Host.EndsWith(d, StringComparison.OrdinalIgnoreCase)))
                    return ResourceHandler.ForErrorMessage($"Domain not whitelisted: {requestUri.Host}", System.Net.HttpStatusCode.Forbidden);

                return null;
            }

            if (requestUri.Scheme == "file")
            {
                var localPath = Path.Combine(_options.LocalRootFolder, Path.GetFileName(requestUri.LocalPath));
                return File.Exists(localPath) ? ResourceHandler.FromFilePath(localPath)
                                            : ResourceHandler.ForErrorMessage("File not found in sandbox", System.Net.HttpStatusCode.NotFound);
            }

            if (!requestUri.IsAbsoluteUri)
            {
                var relativePath = Path.Combine(_options.LocalRootFolder, requestUri.ToString().TrimStart('/'));
                return File.Exists(relativePath) ? ResourceHandler.FromFilePath(relativePath)
                                                : ResourceHandler.ForErrorMessage("Relative resource not found in sandbox", System.Net.HttpStatusCode.NotFound);
            }

            return denied;
        }

        private void LogRequest(IRequest request)
        {
            var msg = $"Request: {request.Method} {request.Url}";
            _logger?.LogInformation(msg);
        }
    }

    public class FilteringRequestHandler : CefSharp.Handler.RequestHandler
    {
        public FilteringRequestOptions Options { get; set; } = FilteringRequestOptionsLoader.Load(true);
        public ILoggerFactory? LoggerFactory { get; set; }

        protected override IResourceRequestHandler GetResourceRequestHandler(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool isNavigation, bool isDownload, string requestInitiator, ref bool disableDefaultHandling)
        {
            //always hand off to the request handler to respond to resource requests?
            return new FilteringResourceRequestHandler(Options, LoggerFactory ?? NullLoggerFactory.Instance);
        }
    }
}
