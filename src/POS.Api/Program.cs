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

            // Register application services and in-memory infra for demo
            builder.Services.AddScoped<POS.Domain.Interfaces.IProductRepository, POS.Infrastructure.Repositories.InMemoryProductRepository>();
            builder.Services.AddScoped<POS.Domain.Interfaces.IInvoiceRepository, POS.Infrastructure.Repositories.InMemoryInvoiceRepository>();
            builder.Services.AddScoped<POS.Domain.Interfaces.IUnitOfWork, POS.Infrastructure.UnitOfWork.InMemoryUnitOfWork>();
            builder.Services.AddScoped<POS.Application.Handlers.CreateInvoiceHandler>();

            var app = builder.Build();

            app.MapControllers();

            app.Run();
        }
    }
}
