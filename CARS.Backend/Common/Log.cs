using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;

namespace CARS.Backend.Common
{
    public class Log
    {
        private static readonly log4net.ILog InfoLog = log4net.LogManager.GetLogger("InfoLogger");
        private static readonly log4net.ILog WarnLog = log4net.LogManager.GetLogger("WarnLogger");
        private static readonly log4net.ILog FatalLog = log4net.LogManager.GetLogger("FatalLogger");
        private static readonly log4net.ILog DebugLog = log4net.LogManager.GetLogger("DebugLogger");

        public Log() { }

        /// <summary>
        /// Used for logging the normal information.  For example, start applying leave, finish applying leave.
        /// </summary>
        /// <param name="s"></param>
        public static void Info(object s)
        {
            if (InfoLog.IsInfoEnabled) InfoLog.Info(s);
        }

        /// <summary>
        /// Used for logging some abnormal information which you think the application should not be go this rule.
        /// For example, in the ApplyLeave method, the input parameter Employee should not be null.  But it is really null you could have a log here.
        /// </summary>
        /// <param name="s"></param>
        public static void Warn(object s)
        {
            if (WarnLog.IsWarnEnabled) WarnLog.Warn(s);
        }

        /// <summary>
        /// Used for logging the exception message.
        /// </summary>
        /// <param name="s"></param>
        public static void Exception(object s)
        {
            if (FatalLog.IsFatalEnabled) FatalLog.Fatal(s);
        }

        /// <summary>
        /// Used for logging debug information.
        /// </summary>
        /// <param name="s"></param>
        public static void Debug(object s)
        {
            if (DebugLog.IsDebugEnabled) DebugLog.Debug(s);
        }
    }
}