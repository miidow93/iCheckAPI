using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace iCheckAPI
{
    public class Program
    {
        static string[] urls = new string[]
        {
            "http://localhost:2229",
            "http://*:2229"
        };
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseUrls(urls)
                .UseKestrel()
                .UseIISIntegration()
                .UseStartup<Startup>();
    }
}
