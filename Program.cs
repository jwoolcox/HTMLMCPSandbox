using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace HTMLMCPSandbox
{
    internal static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            ApplicationConfiguration.Initialize();

            var wdii = new MCPDOMBridgeImpl();

            var form = new frmWebBrowse(wdii);

            Task.Run(() =>
            {
                var builder = WebApplication.CreateBuilder(args);

                builder.Services.AddSingleton<IMCPCommands>(wdii);
                builder.Services.AddSingleton<MCPDOMTools>(); //
                builder.Services.AddMcpServer()
                        .WithTools<MCPDOMTools>()
                        .WithHttpTransport();


                var app = builder.Build();

                app.MapMcp();

                app.Run("http://localhost:8000");
            });

            Application.Run(form);
        }
    }
}
