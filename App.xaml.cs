using BankApp.Repos;
using BankApp.ViewModel;
using BankApp.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace BankApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IHost host;
        public IServiceProvider ServiceProvider { get; private set; }

        public IConfiguration Configuration { get; private set; }

        public App()
        {
            host = Host.CreateDefaultBuilder()
                    .ConfigureAppConfiguration((context, builder) =>
                    {
                        builder.AddJsonFile("appsettings.json", optional: true);
                    })
                    .ConfigureServices((services) =>
                    {
                        ConfigureServices(services);
                    })
                    .Build();

            ServiceProvider = host.Services;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
        }
        /// <summary>
        /// ConfigureServices
        /// </summary>
        /// <param name="services"></param>
        private void ConfigureServices(IServiceCollection services)
        {
            var connectionString = "Server=localhost,1433;Initial Catalog=bank; User ID =sa; Password=Passwordxd!23";

            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString), ServiceLifetime.Transient);

            services.AddScoped<UserRepository>();
            services.AddScoped<BankAccountRepository>();
            services.AddScoped<LoanRepository>();
            services.AddScoped<InvestmentRepository>();

            services.AddSingleton(typeof(TransferBroker));

            services.AddTransient(typeof(MainWindow));
            services.AddTransient(typeof(AccountWindow));
            services.AddTransient(typeof(AccountWindowViewModel));
            services.AddSingleton(typeof(MainWindowViewModel));

        }

    }
}

