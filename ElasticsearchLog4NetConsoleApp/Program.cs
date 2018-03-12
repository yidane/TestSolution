using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticsearchLog4NetConsoleApp
{
    class Program
    {
        // Create a new logging instance
        private static readonly ILog _log = LogManager.GetLogger(typeof(Program));

        static void Main(string[] args)
        {
            Stopwatch watch = new Stopwatch();
            Console.WriteLine("开始写入日志");
            watch.Start();
            for (int i = 0; i < 100000; i++)
            {
                _log.Warn("1");
                // Most basic logging example.
                WriteLog(string.Format("System Error {0}", "Divide by zero error."));
                WriteLog(string.Format("System Error {0}", "Divide by zero error."));
                WriteLog(string.Format("System Error {0}", "Divide by zero error."));
                WriteLog(string.Format("System Error {0}", "Divide by zero error."));
                WriteLog(string.Format("System Error {0}", "Divide by zero error."));
                WriteLog(string.Format("System Error {0}", "Divide by zero error."));
                WriteLog(string.Format("System Error {0}", "Divide by zero error."));
                WriteLog(string.Format("System Error {0}", "Divide by zero error."));

                // Log a message with an exeption object
                WriteLog("System Error", new Exception("Something terrible happened."));

                // Log an exception object with custom fields in the Data property
                Exception newException = new Exception("There was a system error");
                newException.Data.Add("CustomProperty", "CustomPropertyValue");
                newException.Data.Add("SystemUserID", "User43");

                WriteLog("Something broke.", newException);
            }
            watch.Stop();

            Console.WriteLine(watch.ElapsedMilliseconds);
            Console.WriteLine("结束写入日志");
            Console.ReadKey();
        }

        static void WriteLog(string message, Exception ex)
        {
            Task.Run(() =>
            {
                _log.Error(message, ex);
            });
        }

        static void WriteLog(string message)
        {
            Task.Run(() =>
            {
                _log.Error(message);
            });
        }
    }
}
