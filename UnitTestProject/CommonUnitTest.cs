using System;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace UnitTestProject
{
    [TestClass]
    public class CommonUnitTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var total = 1000;
            var result = new Dictionary<string, long>();

            Console.WriteLine(DateTime.Now);
            HashSet<Task> taskHashSet = new HashSet<Task>();
            Task.Run(() =>
            {
                Thread.Sleep(1000);
                Console.WriteLine("0");
            }).Wait();
            taskHashSet.Add(Task.Run(() =>
            {
                Thread.Sleep(3000);
                Console.WriteLine("1");
            }));
            taskHashSet.Add(Task.Run(() =>
            {
                Thread.Sleep(2000);
                Console.WriteLine("2");
            }));
            taskHashSet.Add(Task.Run(() =>
            {
                Thread.Sleep(2999);
                Console.WriteLine("3");
            }));
            taskHashSet.Add(Task.Run(() =>
            {
                Thread.Sleep(4000);
                Console.WriteLine("4");
            }));
            foreach (var item in taskHashSet)
            {
                item.Wait();
            }
            Console.WriteLine(DateTime.Now);

            var task1 = Task.Run(() =>
             {
                 var configHelper = new ConfigHelper();
                 Stopwatch watch = new Stopwatch();
                 watch.Start();
                 for (int i = 0; i < total; i++)
                 {
                     var s = configHelper.Config;
                 }
                 watch.Stop();
                 result.Add("Config", watch.ElapsedMilliseconds);
             });

            var task2 = Task.Run(() =>
             {
                 var configHelper = new ConfigHelper();
                 Stopwatch watch = new Stopwatch();
                 watch.Start();
                 for (int i = 0; i < total; i++)
                 {
                     var s = configHelper.ReadConfig();
                 }
                 watch.Stop();
                 result.Add("ReadConfig", watch.ElapsedMilliseconds);
             });

            var task3 = Task.Run(() =>
             {
                 var configHelper = new ConfigHelper();
                 Stopwatch watch = new Stopwatch();
                 watch.Restart();
                 for (int i = 0; i < total; i++)
                 {
                     var s = ConfigHelper.StaticConfig;
                 }
                 watch.Stop();
                 result.Add("StaticConfig", watch.ElapsedMilliseconds);
             });

            var task4 = Task.Run(() =>
             {
                 var configHelper = new ConfigHelper();
                 Stopwatch watch = new Stopwatch();
                 watch.Start();
                 for (int i = 0; i < total; i++)
                 {
                     var s = ConfigHelper.StaticReadonlyConfig;
                 }
                 watch.Stop();
                 result.Add("StaticReadonlyConfig", watch.ElapsedMilliseconds);
             });

            task1.Wait();
            task2.Wait();
            task3.Wait();
            task4.Wait();

            foreach (var item in result)
            {
                Console.WriteLine(string.Format("{0}:{1}", item.Key, item.Value));
            }
        }
    }

    class ConfigHelper
    {
        //结论
        //读取配置文件，最好使用静态变量储存起来。静态变量在应用程序启动时候会初始化
        private static string Read()
        {
            return ConfigurationManager.AppSettings["TestConfig"];
        }

        public string ReadConfig()
        {
            return ConfigurationManager.AppSettings["TestConfig"];
        }

        private string config;
        public string Config
        {
            get
            {
                if (string.IsNullOrEmpty(config))
                    config = ConfigurationManager.AppSettings["TestConfig"];

                return config;
            }
        }

        public static readonly string StaticReadonlyConfig = ConfigurationManager.AppSettings["TestConfig"];
        public static string StaticConfig = Read();
    }
}
