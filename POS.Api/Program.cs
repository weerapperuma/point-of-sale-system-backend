using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace POS.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Minimal placeholder setup
            builder.Services.AddControllers();

            var app = builder.Build();

            app.MapControllers();

            app.Run();
        }
    }
}
