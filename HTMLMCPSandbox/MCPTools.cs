using ModelContextProtocol.Server;
using System.ComponentModel;

namespace HTMLMCPSandbox;

[McpServerToolType]
public class MCPTools
{
    private readonly IMCPCommands _bridge;

    public MCPTools(IMCPCommands bridge)
    {
        _bridge = bridge;
    }

    [McpServerTool, Description("Runs Javascript in the tool web browser")]
    public async Task<string> RunJavascriptAsync([Description("Javascript to execute")]string javascript)
    {
        return await _bridge.RunJavascriptAsync(javascript);
    }

    [McpServerTool, Description("Sets the contents of the browser DOM")]
    public async Task SetDOMContent([Description("Html content to write to the DOM")]string html)
    {
        await _bridge.SetDOMContentAsync(html);
    }

    [McpServerTool, Description("Gets the contents of the browser DOM")]
    public async Task<string> GetDOMContent()
    {
        return await _bridge.GetDOMContentAsync();
    }

    [McpServerTool, Description("Gets the innerHTML content of the element specified by selector")]
    public async Task<string> GetInnerHTMLBySelectorAsync([Description("Element selector")]string selector)
    {
        return await _bridge.GetInnerHTMLBySelectorAsync(selector);
    }

    [McpServerTool, Description("Sets the innerHTML content of the element specified by selector")]
    public async Task SetInnerHTMLBySelectorAsync([Description("Element selector")]string selector, [Description("innerHTML content")]string innerHTML)
    {
        await _bridge.SetInnerHTMLBySelectorAsync(selector, innerHTML);
    }
}
