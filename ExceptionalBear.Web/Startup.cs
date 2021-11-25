using Icebear.Exceptions.Core.LogReaders;
using Icebear.Exceptions.Core.LogWriters;
using Icebear.Exceptions.Core.LogWriters.Providers;
using Icebear.Exceptions.Core.Models;
using Icebear.Exceptions.Db.Ef;
using Icebear.Exceptions.Db.Ef.LogWriters;
using Icebear.Exceptions.Db.Ef.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class Startup
{
    private IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        InitializeLogger(services);
    }

    private void InitializeLogger(IServiceCollection services)
    {
        var dbOptions = new DbContextOptionsBuilder()
            .UseSqlServer(
                Configuration.GetConnectionString("ErrorDatabase"))
            .EnableDetailedErrors()
            .EnableSensitiveDataLogging()
            .Options;

        var loggerBuilder = new LoggerBuilder()
            .WithConsoleLevel(LogType.Trace)
            .WithExceptionTextProvider(ExceptionTextProviders.Default);

        var logger = loggerBuilder.WithWriter(loggerBuilder.BuildInDb(
                new Ef5Repository(LogContextProvider)))
            .Build(LogType.Warning);
        
        services.AddSingleton<ILogWriter>(logger.Writer);
        services.AddSingleton<ILogReader>(logger.Reader);

        ErrorDbContext LogContextProvider()
        {
            return new ErrorDbContext(dbOptions);
        }
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseExceptionHandler("/Error");

        app.UseStatusCodePages();
        app.UseRouting();
        app.UseStaticFiles();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGet("/", async context => { await context.Response.WriteAsync("Hello World!"); });
            endpoints.MapControllers();
        });
    }
}