// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");



using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Layout;
using log4net.Repository;
using ProgAssign1;
using System.Diagnostics;

ILog log = LogManager.GetLogger(typeof(Program));

Stopwatch stopwatch = new Stopwatch();
stopwatch.Start();
ConfigureLogging();

long CountValidRows = 0;
long CountSkippedRows = 0;
DirWalker.walk(@"C:\Users\amit.dey\source\repos\ProgAssign1\ProgAssign1\Sample Data\Sample Data\",ref CountValidRows, ref CountSkippedRows);


log.Info("Total number of valid rows: " + CountValidRows);
log.Info("Total number of skipped rows: " + CountSkippedRows);


stopwatch.Stop();
TimeSpan ts = stopwatch.Elapsed;
log.Info($"Total execution time: {ts.TotalSeconds} s");



static void ConfigureLogging()
{
    // Create a RollingFileAppender
    RollingFileAppender roller = new RollingFileAppender();
    roller.AppendToFile = true;
    roller.File = @"C:\Users\amit.dey\source\repos\ProgAssign1\ProgAssign1\logs\app.log"; // Set your log file path here
    roller.MaxSizeRollBackups = 5;
    roller.MaximumFileSize = "10MB";
    roller.RollingStyle = RollingFileAppender.RollingMode.Size;
    roller.StaticLogFileName = true;

    // Set the layout for the appender
    PatternLayout layout = new PatternLayout();
    layout.ConversionPattern = "%date %-5level - %message%newline";
    layout.ActivateOptions();
    roller.Layout = layout;

    // Activate the appender configuration
    roller.ActivateOptions();

    // Get the logger repository and configure it
    ILoggerRepository repository = LogManager.GetRepository();
    BasicConfigurator.Configure(repository, roller);
}