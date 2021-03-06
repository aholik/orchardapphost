﻿using System.IO;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Filter;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using Lombiq.OrchardAppHost.Configuration;
using Orchard.Logging;

namespace Lombiq.OrchardAppHost
{
    /// <summary>
    /// Provides the default Orchard log4net configuration but not from an XML config file but from code. The point is that so there is no need for
    /// thinking about config files and everything can be changed just from code. A config file can still be used.
    /// </summary>
    internal static class DefaultLog4NetConfigurator
    {
        public static void Configure(string logFilesFolderPath, Log4NetConfigurator configurator)
        {
            if (logFilesFolderPath.StartsWith("~")) logFilesFolderPath = logFilesFolderPath.Substring(1);
            if (logFilesFolderPath.StartsWith("/")) logFilesFolderPath = logFilesFolderPath.Substring(1);

            var hierarchy = (Hierarchy)LogManager.GetRepository();


            hierarchy.Root.Level = Level.Warn;
            hierarchy.Configured = true;

            var patternLayout = new PatternLayout
            {
                ConversionPattern = "%date [%thread] %-5level %logger - %P{Tenant} - %message%newline"
            };
            patternLayout.ActivateOptions();

            var debugFileAppender = BaseFileAppender(patternLayout);
            debugFileAppender.Name = "debug-file";
            debugFileAppender.File = Path.Combine(logFilesFolderPath, "orchard-debug");
            debugFileAppender.ActivateOptions();
            hierarchy.Root.AddAppender(debugFileAppender);

            var errorFileAppender = BaseFileAppender(patternLayout);
            errorFileAppender.Name = "error-file";
            errorFileAppender.File = Path.Combine(logFilesFolderPath, "orchard-error");
            var levelFilter = new LevelRangeFilter { LevelMin = Level.Error };
            levelFilter.ActivateOptions();
            errorFileAppender.AddFilter(levelFilter);
            errorFileAppender.ActivateOptions();
            hierarchy.Root.AddAppender(errorFileAppender);

            var simpleLayout = new SimpleLayout();
            simpleLayout.ActivateOptions();
            var debuggerAppender = new DebugAppender();
            debuggerAppender.Name = "debugger";
            debuggerAppender.ImmediateFlush = true;
            debuggerAppender.Layout = simpleLayout;
            debuggerAppender.ActivateOptions();
            hierarchy.Root.AddAppender(debuggerAppender);

            GetLogger("Orchard").AddAppender(debuggerAppender);
            GetLogger("Orchard.Localization").Level = Level.Warn;
            GetLogger("NHibernate.Cache").Level = Level.Error;
            GetLogger("NHibernate.AdoNet.AbstractBatcher").Level = Level.Off;
            GetLogger("NHibernate.Util.ADOExceptionReporter").Level = Level.Off;

            configurator(hierarchy);
        }


        private static OrchardFileAppender BaseFileAppender(PatternLayout patternLayout)
        {
            var appender = new OrchardFileAppender
            {
                AppendToFile = true,
                ImmediateFlush = true,
                StaticLogFileName = false,
                RollingStyle = log4net.Appender.RollingFileAppender.RollingMode.Date,
                DatePattern = "-yyyy.MM.dd'.log'",
                LockingModel = new FileAppender.MinimalLock(),
                Layout = patternLayout
            };

            return appender;
        }

        private static Logger GetLogger(string name)
        {
            return (Logger)LogManager.GetRepository().GetLogger(name);
        }
    }
}
