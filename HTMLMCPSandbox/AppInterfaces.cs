namespace HTMLMCPSandbox
{
    public interface IMCPCommands
    {
        Task SetDOMContentAsync(string HTMLContents);
        Task<string> GetDOMContentAsync();
        Task<string> RunJavascriptAsync(string Javascript);

        Task<string> GetInnerHTMLBySelectorAsync(string selector);

        Task SetInnerHTMLBySelectorAsync(string selector, string innerHTML);
    }

    public interface IDOMHooks
    {
        Func<string, Task>? OnSetDOMContentAsync { get; set; }
        Func<Task<string>>? OnGetDOMContentAsync { get; set; }
        Func<string, Task<string>>? OnRunJavascriptAsync { get; set; }
        Func<string, Task<string>>? OnGetInnerHTMLBySelectorAsync { get; set; }
        Func<string, string, Task>? OnSetInnerHTMLBySelectorAsync { get; set; }
    }
}
