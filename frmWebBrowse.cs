using CefSharp;
using CefSharp.WinForms;
using HTMLMCPSandbox.Filtering;
using Microsoft.Extensions.Logging;

namespace HTMLMCPSandbox
{
    public partial class frmWebBrowse : Form
    {
        private IDOMHooks _domInterface;

        private ILoggerFactory? _loggerFactory;

        public frmWebBrowse(IDOMHooks domInterface, ILoggerFactory? loggerFactory = null)
        {
            InitializeComponent();

            _loggerFactory = loggerFactory;

            PrepareBrowserInstance();

            this._domInterface = domInterface;
            _domInterface.OnSetDOMContentAsync += DoSetDOMContentAsync;
            _domInterface.OnGetDOMContentAsync += DoGetDOMContentAsync;
            _domInterface.OnRunJavascriptAsync += DoRunJavascriptAsync;
            _domInterface.OnGetInnerHTMLBySelectorAsync += DoGetInnerHTMLBySelectorAsync;
            _domInterface.OnSetInnerHTMLBySelectorAsync += DoSetInnerHTMLBySelectorAsync;
        }

        private void PrepareBrowserInstance()
        {
            CefSettings browserSettings = new CefSettings();

            browserSettings.UserAgent = $"HTMLMCPSandbox ^_^ CEF[{Cef.CefSharpVersion}]";
            browserSettings.CefCommandLineArgs.Add("no-proxy-server");

            string resourceRoot = Path.Combine(AppContext.BaseDirectory, "_internal");

            browserSettings.RegisterScheme(new CefCustomScheme
            {
                SchemeName = "internal",
                SchemeHandlerFactory = new InternalSchemeHandlerFactory(),
                IsStandard = false,
                IsLocal = false,
                IsSecure = true,
                IsCorsEnabled = true
            });

            Cef.Initialize(browserSettings);

            webBrowser.ConsoleMessage += (s, e) =>
            {
                Console.WriteLine($"Browser console message: {e.Message}");
            };

            webBrowser.IsBrowserInitializedChanged += async (s, e) =>
            {
                await webBrowser.LoadUrlAsync("internal:///appIndex.html");
            };

            FilteringRequestHandler requestHandler = new FilteringRequestHandler();

            if (null != _loggerFactory)
                requestHandler.LoggerFactory = _loggerFactory;

            webBrowser.RequestHandler = requestHandler;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            Cef.Shutdown();

            base.OnFormClosing(e);
        }

        private async Task<string> DoRunJavascriptAsync(string request)
        {
            return await ExecuteInWebViewAsync(request);
        }

        private async Task<string> ExecuteInWebViewAsync(string script)
        {
            if (!webBrowser.IsBrowserInitialized)
                throw new InvalidOperationException("Browser not initialized yet.");

            var frame = webBrowser.GetMainFrame();
            if (frame == null)
                throw new InvalidOperationException("Main frame is not available.");

            var response = await frame.EvaluateScriptAsync(script).ConfigureAwait(false);

            return response.Result?.ToString() ?? string.Empty;
        }

        private async Task DoSetDOMContentAsync(string content)
        {
            await Task.Run(() => webBrowser.LoadHtml(content));
        }

        private async Task<string> DoGetDOMContentAsync()
        {
            string source = await webBrowser.GetMainFrame().GetSourceAsync();

            return source ?? "";
        }

        private async Task DoSetInnerHTMLBySelectorAsync(string selector, string innerHTML)
        {
            string script = $@"
                (function() {{
                    var element = document.querySelector('{selector}');
                    if (element) {{
                        element.innerHTML = `{innerHTML}`;
                        return true;
                    }}
                    return false;
                }})()
            ";

            string result = await DoRunJavascriptAsync(script);
            //return result;
        }

        private async Task<string> DoGetInnerHTMLBySelectorAsync(string selector)
        {
            string script = $@"
                (function() {{
                    var element = document.querySelector('{selector}');
                    return element ? element.innerHTML : null;
                }})()
            ";

            string result = await DoRunJavascriptAsync(script);
            return result;
        }
    }
}

