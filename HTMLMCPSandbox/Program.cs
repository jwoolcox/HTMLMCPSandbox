using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HTMLMCPSandbox
{
    internal static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            ApplicationConfiguration.Initialize();

            var wdii = new MCPDOMBridgeImpl();

            var builder = WebApplication.CreateBuilder(args);

            Task.Run(() =>
            {
                builder.Services.AddSingleton<IMCPCommands>(wdii);
                builder.Services.AddSingleton<MCPTools>(); //
                builder.Services.AddMcpServer((mcpopts) => 
                                {

                                })
                                .WithTools<MCPTools>()
                                .WithHttpTransport((opt) =>
                                {
                            
                                });


                var app = builder.Build();

                app.MapMcp();

                app.Run("http://localhost:8000");

            });

            ILoggerFactory loggerFactory = LoggerFactory.Create(loggingBuilder =>
            {
                loggingBuilder.AddConsole();
                loggingBuilder.SetMinimumLevel(LogLevel.Debug);
            });

            var form = new frmWebBrowse(wdii, loggerFactory);

            Application.Run(form);
        }
    }
}
