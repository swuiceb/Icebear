# yourLogger Logging Library
# What is it
yourLogger Exception Logging Library is an attempt to simplify the day to day workflow of a developer by creating a layer of abstraction that is extensible, with sensible defaults that anyone can start using within minutes.

The goal is that is all a developer needs to do, until they are ready to move on to an observability tool such as Datadog.

# Why
As I was getting close to finishing up a project at my last job, I knew we did not have a good system for identifying exceptions.
Sure we were logging it all, and it was available from the cloud provider, it would just be so much simpler if we could send user an Error number and look it in our storage.

Then I thought about how to do that, sure it wasn't difficult, it just takes a bit of time that you'd prefer that you wouldn't have to spend.

I decided it would be useful to create a minimum implementation of an Exception library, but keep it open such that additional functionalities can either be added easily through custom code, or additional nuget packages (which is still being worked on)
And I think it is a bit more flexible than log4net, and easier to configure.

So now that I'm off work, this is the first project I wanted to work on.

# What does it do?
This is a logging library, so obviously it logs.
The Core project contains the base implementation on how to create the logger, and how the logger translates exceptions into a model that can be consumed
There are several levels (pretty typical) of information
```c#
    public enum LogType
    {
        Trace,
        Info,
        Warning,
        Error,
        Custom,
        Custom1,
        Custom2,
        Custom3
    }
```

# How do I use it?
Really, there are four steps
1. Configure your DB (or not, if you want to log it somewhere else)

```c#
var dbOptions = new DbContextOptionsBuilder()
            .UseSqlServer(
                Configuration.GetConnectionString("ErrorDatabase"))
            .Options;
```
2. Initiate the Builder
```c#
var loggerBuilder = new LoggerBuilder()
            .WithConsoleLevel(LogType.Trace)
            .WithExceptionTextProvider(ExceptionTextProviders.Default);
```
3. Build the logger
```c#
var logger = loggerBuilder.Build(LogType.Info);
```
*The above code builds the default logger which sends the log messages to Console, this isn't at all exciting*

You likely would want to do something like the following to send the logs to DB.

```c#
var logger = loggerBuilder.WithWriter(
        loggerBuilder.BuildInDb(LogContextProvider))
        .Build(LogType.Info);
```
or for a bit more efficiency, a DB implementation that doesn't log every entry until it has enough
```c#
var logger = loggerBuilder.WithWriter(loggerBuilder.BuildRollingDb(
        new Ef5Repository(LogContextProvider), 50))
    .Build(LogType.Warning);
```

4. Add it to service collection
```c#
services.AddSingleton<ILogWriter>(logger.Writer);
services.AddSingleton<ILogReader>(logger.Reader);
```
5. Start Logging
Logging is simple, for ```Warn``` and ```Error``` type of logs, use the LogWarn or LogError Api
```c#
        
        // Warn
        await exceptionLogWriter.LogWarnAsync(new Exception("My bad"));
       
        // Error 
        await exceptionLogWriter.LogErrorAsync(new Exception("My bad"));
 ```
For all custom events/logs, use the LogAsync method
```c#
        await LogAsync<T>(LogType logType, string message, T detail);
```

TODO:
- Test with Dot net core 3.1 (works with .net 5 and .net 6, .Net 6 needs EF Core 6)
- A nosql implementation

# What's in this Repo
In this Repo, there is one Abstraction (core) project, and one Implementation Project.
- yourLogger.Exceptions.Core - Base implementation encompasses the base implementation for each type of logger
  - The core project also includes an InMemory implementation as a sample
- yourLogger.Exceptions.Db.Ef - Implementation that uses a DB as storage mechanism
- yourLogger.Exceptions.Mvc - Sample endpoint implementations for reading and displaying of recorded logs
