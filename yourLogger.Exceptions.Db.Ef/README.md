### yourLogger Exception EF Core Logger
This is the EF Core implementation of the [yourLogger Logging Library](https://github.com/swuiceb/yourLogger/blob/main/README.md)

### Why?
The intention of the library was to simplify something all developers have to implement many times over their career.
**Logging**

Logging is a very common and important task. Therefore it should be super simple to implement.

For example, with lombok annotations. All you need to start logging is use the @Sfj4 annotation on the class.

For DotNet Core, it can be as simple as passing in ILogger to the constructor thanks to the magic of dependency injection.

But it only prints to console, and we need more than that.
We need to store the logs and inspect them later.

When an exception occurs, it may even be helpful to present a page to the user with a reference number.

yourLogger is a library that looks to abstract it all away.
The Ef Core Logger implementation is the Ef core implementation of that abstraction.

###
How do I use this?

1. Install the [nuget package](https://www.nuget.org/packages/yourLogger.Exceptions.Db.Ef/).

2. Instantiate the Builder using ```c# DbContextOptionsBuilder```
*Make sure the appropriate EF Core package is installed*

For example, the below snippet creates a Database Option using the "ErrorDatabase" Section in AppSettings.Json
```c#
var dbOptions = new DbContextOptionsBuilder()
            .UseSqlServer(
                Configuration.GetConnectionString("ErrorDatabase"))
            .Options;
```

3. Initiate a LogWriterBuilder to build the logger
```c#
var loggerBuilder = new LoggerBuilder()
            .WithConsoleLevel(LogType.Warn);
```

4. Pass in the DB writer to the log builder.
```c#
loggerBuilder = loggerBuilder.WithWriter(
        loggerBuilder.BuildInDb(LogContextProvider));
```

5. Build the Logger
```c#
var logger = loggerBuilder.Build(LogType.Info);
```

Alternatively, or more simply, we can combine steps 4 and 5 together
```c#
var logger = loggerBuilder.WithWriter(
        loggerBuilder.BuildInDb(LogContextProvider))
        .Build(LogType.Info);
```

6. As the core package, use ```c#logger.Writer``` and ```c#logger.Reader``` for all your logging needs.

### Customization
This library provides two different implementations:
1. Simple DB built with ```BuildInDb``` or
This logger stores each log to the DB as they come in
2. Rolling DB built with ```BuildWithRolling```
The rolling DB implementation collects the logs in memory, and commits the entire batch to the database when there are more than > ```batchSize``` number of entries in memory.

### Changing Table Name
Although most of the EF Core implementation is internal to the nuget package, it is still possible to change the table name. Create a new Context, inherit LogDbContext, and have the LogContextProvider return your new Context.
And you can change the default table settings via the OnConfiguring hook.


