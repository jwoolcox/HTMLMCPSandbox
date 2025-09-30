using ModelContextProtocol.Server;

namespace HTMLMCPSandbox
{
    public class MCPDOMBridgeImpl : IDOMHooks, IMCPCommands
    {
        public async Task SetDOMContentAsync(string HTMLContents)
        {
            await Task.Run(() => OnSetDOMContentAsync?.Invoke(HTMLContents));
        }

        public async Task<string> GetDOMContentAsync()
        {
            string domResult = string.Empty;

            if (OnGetDOMContentAsync != null)
                domResult = await OnGetDOMContentAsync();

            return domResult;
        }

        public async Task<string> RunJavascriptAsync(string Javascript)
        {
            string result = string.Empty;

            if (OnRunJavascriptAsync != null)
                result = await OnRunJavascriptAsync(Javascript);

            return result;
        }

        public async Task<string> GetInnerHTMLBySelectorAsync(string selector)
        {
            string result = string.Empty;

            if (OnGetInnerHTMLBySelectorAsync != null)
                result = await OnGetInnerHTMLBySelectorAsync(selector);

            return result;
        }

        public async Task SetInnerHTMLBySelectorAsync(string selector, string innerHTML)
        {
            string result = string.Empty;

            if (OnSetInnerHTMLBySelectorAsync != null)
                await OnSetInnerHTMLBySelectorAsync(selector, innerHTML);

            //return result;
        }

        public Func<string, Task>? OnSetDOMContentAsync { get; set; }

        public Func<Task<string>>? OnGetDOMContentAsync { get; set; }

        public Func<string, Task<string>>? OnRunJavascriptAsync { get; set; }

        public Func<string, Task<string>>? OnGetInnerHTMLBySelectorAsync { get; set; }

        public Func<string, string, Task>? OnSetInnerHTMLBySelectorAsync { get; set; }
    }
}
