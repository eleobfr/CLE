using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OutilWPF.Configuration;
using OutilWPF.Données;
using OutilWPF.Services;
using System;
using System.Runtime.Versioning;
using System.Windows;

namespace OutilWPF
{
    [SupportedOSPlatform("windows")]
    public partial class App : System.Windows.Application
    {
        private IHost host;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((context, configuration) =>
                {
                    configuration.Sources.Clear();
                    configuration.SetBasePath(AppContext.BaseDirectory);
                    configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    services.Configure<ApplicationOptions>(context.Configuration.GetSection("Application"));
                    services.AddSingleton<IDatabasePathStore, DatabasePathStore>();
                    services.AddSingleton<IUserPreferencesStore, JsonUserPreferencesStore>();
                    services.AddSingleton<IDatabaseSelectionService, DatabaseSelectionService>();
                    services.AddSingleton<IClinicDataService, Access>();
                    services.AddSingleton<ViewModel>();
                    services.AddSingleton<MainWindow>();
                })
                .Build();

            host.Start();

            var mainWindow = host.Services.GetRequiredService<MainWindow>();
            MainWindow = mainWindow;
            mainWindow.Show();
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            if (host != null)
            {
                await host.StopAsync();
                host.Dispose();
            }

            base.OnExit(e);
        }
    }
}
