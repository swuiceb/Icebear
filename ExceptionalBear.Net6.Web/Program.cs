using System.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using yourLogs.Exceptions.Core.LogReaders;
using yourLogs.Exceptions.Core.LogWriters;
using yourLogs.Exceptions.Core.LogWriters.Providers;
using yourLogs.Exceptions.Core.Models;
using yourLogs.Exceptions.Db.Ef;
using yourLogs.Exceptions.Db.Ef.LogWriters;
using yourLogs.Exceptions.Db.Ef.Repository;
using yourLogs.Exceptions.Mvc.Controllers;

var builder = WebApplication.CreateBuilder(args);
var dbOptions = new DbContextOptionsBuilder()
    .UseSqlServer(
        builder.Configuration.GetConnectionString("ErrorDatabase"))
    .EnableDetailedErrors()
    .EnableSensitiveDataLogging()
    .Options;

var loggerBuilder = new LoggerBuilder()
    .WithConsoleLevel(LogType.Trace)
    .WithExceptionTextProvider(ExceptionTextProviders.Default);

var logger = loggerBuilder.WithWriter(loggerBuilder.BuildRollingDb(
        new EfCoreRepository(LogContextProvider), 100))
    .Build(LogType.Warning);

builder.Services.AddControllers();
builder.Services.AddSingleton<ILogWriter>(logger.Writer);
builder.Services.AddSingleton<ILogReader>(logger.Reader);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMvc(options => options.EnableEndpointRouting = false);

var app = builder.Build();
// app.MapGet("/", () => "Hello World!");
// app.MapControllers();
var assembly = typeof(LogEntryController).Assembly;
builder.Services.AddControllers()
    .PartManager.ApplicationParts.Add(new AssemblyPart(assembly));

app.UseSwagger();
app.UseSwaggerUI();
app.UseMvc();

app.Run();

ErrorDbContext LogContextProvider()
{
    return new ErrorDbContext(dbOptions);
}