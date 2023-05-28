using backend_squad1.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;


namespace backend_squad1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var hostBuilder = createHost(args);
            hostBuilder.Run();
        }

        public static IHost createHost(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(configure => {
                    configure.UseStartup<Startup>();
                })
                .Build();
    }
}
