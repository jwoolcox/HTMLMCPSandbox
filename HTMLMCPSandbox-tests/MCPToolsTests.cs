using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using HTMLMCPSandbox;
using Moq;
using Xunit;

namespace HTMLMCPSandbox_tests
{
    // Lightweight fake implementation of IMCPCommands for unit testing MCPTools.
    class FakeMCPCommands : IMCPCommands
    {
        public string? LastSetDOMContent;
        private readonly Dictionary<string, string> _selectorMap = new();

        public Task SetDOMContentAsync(string HTMLContents)
        {
            LastSetDOMContent = HTMLContents;
            return Task.CompletedTask;
        }

        public Task<string> GetDOMContentAsync()
        {
            return Task.FromResult(LastSetDOMContent ?? string.Empty);
        }

        public Task<string> RunJavascriptAsync(string Javascript)
        {
            return Task.FromResult("JS_RESULT:" + Javascript);
        }

        public Task<string> GetInnerHTMLBySelectorAsync(string selector)
        {
            return Task.FromResult(_selectorMap.TryGetValue(selector, out var v) ? v : string.Empty);
        }

        public Task SetInnerHTMLBySelectorAsync(string selector, string innerHTML)
        {
            _selectorMap[selector] = innerHTML;
            return Task.CompletedTask;
        }
    }

    public class MCPToolsTests
    {
        [Fact]
        public async Task RunJavascriptAsync_ForwardsCallAndReturnsValue()
        {
            var fake = new FakeMCPCommands();
            var sut = new MCPTools(fake);

            var result = await sut.RunJavascriptAsync("1+1");

            Assert.Equal("JS_RESULT:1+1", result);
        }

        [Fact]
        public async Task SetDOMContent_ForwardsCallToBridge()
        {
            var fake = new FakeMCPCommands();
            var sut = new MCPTools(fake);

            await sut.SetDOMContent("<p>hello</p>");

            Assert.Equal("<p>hello</p>", fake.LastSetDOMContent);
        }

        [Fact]
        public async Task GetDOMContent_ReturnsBridgeValue()
        {
            var fake = new FakeMCPCommands { LastSetDOMContent = "<div>content</div>" };
            var sut = new MCPTools(fake);

            var content = await sut.GetDOMContent();

            Assert.Equal("<div>content</div>", content);
        }

        [Fact]
        public async Task SetAndGetInnerHTMLBySelector_WorkAsExpected()
        {
            var fake = new FakeMCPCommands();
            var sut = new MCPTools(fake);

            await sut.SetInnerHTMLBySelectorAsync("#myId", "<span>ok</span>");
            var inner = await sut.GetInnerHTMLBySelectorAsync("#myId");

            Assert.Equal("<span>ok</span>", inner);
        }

        [Fact]
        public async Task GetInnerHTMLBySelector_ReturnsEmptyWhenMissing()
        {
            var fake = new FakeMCPCommands();
            var sut = new MCPTools(fake);

            var inner = await sut.GetInnerHTMLBySelectorAsync("#doesNotExist");

            Assert.Equal(string.Empty, inner);
        }
    }
}