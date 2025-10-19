using CefSharp;
using CefSharp.WinForms;
using HTMLMCPSandbox.Filtering;
using Microsoft.Extensions.Logging;

namespace HTMLMCPSandbox
{
    public partial class frmWebBrowse : Form
    {
        private IDOMHooks _domInterface;

        public frmWebBrowse(IDOMHooks domInterface, ILoggerFactory? loggerFactory = null)
        {
            InitializeComponent();

            Cef.Initialize(new CefSettings()
            {
                UserAgent = $"HTMLMCPSandbox ^_^ CEF[{Cef.CefSharpVersion}]",
                CefCommandLineArgs = {
                    ["disable-gcm"] = "1",
                        }
            });

            webBrowser.IsBrowserInitializedChanged += async (s, e) =>
            {
                string ic = GetInitialContent();
                await DoSetDOMContentAsync(ic);
            };

            FilteringRequestHandler requestHandler = new FilteringRequestHandler();

            requestHandler.LoggerFactory = loggerFactory;

            webBrowser.RequestHandler = requestHandler;

            this._domInterface = domInterface;
            _domInterface.OnSetDOMContentAsync += DoSetDOMContentAsync;
            _domInterface.OnGetDOMContentAsync += DoGetDOMContentAsync;
            _domInterface.OnRunJavascriptAsync += DoRunJavascriptAsync;
            _domInterface.OnGetInnerHTMLBySelectorAsync += DoGetInnerHTMLBySelectorAsync;
            _domInterface.OnSetInnerHTMLBySelectorAsync += DoSetInnerHTMLBySelectorAsync;
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

            if (!response.Success)
                throw new Exception($"Script execution failed: {response.Message}");

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

        private string GetInitialContent()
        {
            return @"
<!DOCTYPE html>
<html lang='en'>
<head>
<meta charset='UTF-8'>
<title>HTMLMCPSandbox</title>
<style>
    /* Material-ish style reset */
    * {
        box-sizing: border-box;
        margin: 0;
        padding: 0;
        font-family: 'Roboto', sans-serif;
    }

    body {
        background-color: #f5f5f5;
        color: #212121;
        height: 100vh;
        display: flex;
        justify-content: center;
        align-items: center;
        flex-direction: column;
        text-align: center;
    }

    .card {
        background-color: #ffffff;
        padding: 2rem;
        border-radius: 12px;
        box-shadow: 0 4px 8px rgba(0,0,0,0.1);
        max-width: 400px;
        width: 90%;
        transition: box-shadow 0.2s ease;
    }

    .card:hover {
        box-shadow: 0 8px 16px rgba(0,0,0,0.2);
    }

    h1 {
        font-size: 1.8rem;
        color: #6200ee;
        margin-bottom: 1rem;
    }

    p {
        font-size: 1rem;
        color: #424242;
    }

    button {
        margin-top: 1.5rem;
        padding: 0.6rem 1.2rem;
        font-size: 1rem;
        border: none;
        border-radius: 6px;
        background-color: #6200ee;
        color: #ffffff;
        cursor: pointer;
        transition: background-color 0.2s ease;
    }

    button:hover {
        background-color: #3700b3;
    }
</style>
<script>
    // Simple interactive example
    function showAlert() {
        alert('HTMLMCPSandbox is ready!');
    }

    // Optional: Log a message on load
    window.addEventListener('DOMContentLoaded', () => {
        console.log('HTMLMCPSandbox READY!');
    });
</script>
</head>
<body>
    <div class='card'>
        <h1>HTMLMCPSandbox READY!</h1>
        <p>Welcome to your WebDOM sandbox. You can dynamically inject HTML, CSS, and JavaScript here using your LLM tool.</p>
        <button onclick='showAlert()'>Click Me!</button>
    </div>
</body>
</html>
            ";
        }
    }
}

