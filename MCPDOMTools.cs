using HTMLMCPSandbox;
using ModelContextProtocol.Server;
using System.ComponentModel;

[McpServerToolType]
public class MCPDOMTools
{
    private readonly IMCPCommands _bridge;

    public MCPDOMTools(IMCPCommands bridge)
    {
        _bridge = bridge;
    }

    [McpServerTool, Description("Runs Javascript in the tool web browser")]
    public async Task<string> RunJavascriptAsync(string javascript)
    {
        return await _bridge.RunJavascriptAsync(javascript);
    }

    [McpServerTool, Description("Sets the immediate tool web browser DOM contents")]
    public async Task SetDOMContent(string html)
    {
        await _bridge.SetDOMContentAsync(html);
    }

    [McpServerTool, Description("Retrieves the current tool web browser DOM contents")]
    public async Task<string> GetDOMContent()
    {
        return await _bridge.GetDOMContentAsync();
    }

    [McpServerTool, Description("Retrieves the innerHTML of the DOM element specified by query selector")]
    public async Task<string> GetInnerHTMLBySelectorAsync(string selector)
    {
        return await _bridge.GetInnerHTMLBySelectorAsync(selector);
    }

    [McpServerTool, Description("Sets the innerHTML of the DOM element specified by query selector")]
    public async Task SetInnerHTMLBySelectorAsync(string selector, string innerHTML)
    {
        await _bridge.SetInnerHTMLBySelectorAsync(selector, innerHTML);
    }
}
