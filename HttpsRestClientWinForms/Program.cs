using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;

namespace HttpsRestClientWinForms
{
    static class Program
    {
        /// <summary>アプリケーションのメインエントリポイント</summary>
        /// <remarks>Nuget Package : Install-Package Microsoft.Extensions.DependencyInjection -Version 3.1.5</remarks>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ConfigureServices();
            Application.Run((Form1)ServiceProvider.GetService(typeof(Form1)));
        }

        private static void ConfigureServices()
        {
            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            //using Microsoft.Extensions.DependencyInjectionが必要
            services.AddHttpClient();
            services.AddSingleton<Form1>();
            ServiceProvider = services.BuildServiceProvider();
            //System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
        }

        private static System.IServiceProvider ServiceProvider { get; set; }
    }
}