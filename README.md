# Icebear Logging Project
# What is it
Icebear Exception Logging Library is an attempt to simplify the day to day workflow of a developer by creating a layer of abstraction that is extensible, with sensible defaults that anyone can start using within minutes.

The goal is that is all a developer needs to do, until they are ready to move on to an observability tool such as Datadog.

# Why
As I was getting close to finishing up a project at my last job, I knew we did not have a good system for identifying exceptions.
Sure we were logging it all, and it was available from the cloud provider, it would just be so much simpler if we could send user an Error number and look it in our storage.

Then I thought about how to do that, sure it wasn't difficult, it just takes a bit of time that you'd prefer that you wouldn't have to spend.

I decided it would be useful to create a minimum implementation of an Exception library, but keep it open such that additional functionalities can either be added easily through custom code, or additional nuget packages (which is still being worked on)
And I think it is a bit more flexible than log4net, and easier to configure.

So now that I'm off work, this is the first project I wanted to work on.

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
// TODO: See DB options from Icebear.Exceptions.Db.Ef
```c#
var logger = loggerBuilder.WithWriter(
        loggerBuilder.BuildInDb(LogContextProvider))
        .Build(LogType.Info);
```
4. Add it to service collection
```c#
services.AddSingleton<ILogWriter>(logger);
```
# What's in this Repo?
