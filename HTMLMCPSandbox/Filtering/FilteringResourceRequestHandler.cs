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

            if (requestUri.Scheme != "internal" && !_options.AllowedSchemes.Any(d => requestUri.Scheme.EndsWith(d, StringComparison.OrdinalIgnoreCase)))
                return ResourceHandler.ForErrorMessage("Request scheme not whitelisted", System.Net.HttpStatusCode.Forbidden);

            if (requestUri.IsAbsoluteUri)
            {
                if (requestUri.Scheme != "internal" && requestUri.Scheme != "data" && !_options.AllowedDomains.Any(d => requestUri.Host.EndsWith(d, StringComparison.OrdinalIgnoreCase)))
                    return ResourceHandler.ForErrorMessage($"Domain not whitelisted: {requestUri.Host}", System.Net.HttpStatusCode.Forbidden);

                return null;
            }

            return denied;
        }

        private void LogRequest(IRequest request)
        {
            var msg = $"** [{DateTime.UtcNow}] Request: {request.Method} {request.Url}";
            _logger?.LogInformation(msg);
        }
    }

    public class FilteringRequestHandler : CefSharp.Handler.RequestHandler
    {
        public FilteringRequestOptions Options { get; set; } = FilteringRequestOptionsLoader.Load(true);
        public ILoggerFactory? LoggerFactory { get; set; }

        protected override IResourceRequestHandler GetResourceRequestHandler(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool isNavigation, bool isDownload, string requestInitiator, ref bool disableDefaultHandling)
        {
            return new FilteringResourceRequestHandler(Options, LoggerFactory ?? NullLoggerFactory.Instance);
        }
    }
}
