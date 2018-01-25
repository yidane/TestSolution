using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UnitTestProject
{
    [TestClass]
    public class ConcurrentDictionaryTest
    {
        ConcurrentDictionary<int, Tuple<int, DateTime>> ConcurrentDictionary = new ConcurrentDictionary<int, Tuple<int, DateTime>>();
        int total = 100;
        [TestMethod]
        public void TestAddKey()
        {
            var task1 = Task.Run(() =>
            {
                Thread.Sleep(1000);
                for (int i = 0; i < total; i++)
                {
                    Thread.Sleep(10);
                    var item = new Tuple<int, DateTime>(1, DateTime.Now);
                    ConcurrentDictionary.AddOrUpdate(i, item, (i1, item1) => { return item; });
                }
            });
            var task2 = Task.Run(() =>
            {
                Thread.Sleep(1000);
                for (int i = 0; i < total; i++)
                {
                    Thread.Sleep(10);
                    var item = new Tuple<int, DateTime>(2, DateTime.Now);
                    ConcurrentDictionary.AddOrUpdate(i, item, (i1, item1) => { return item; });
                }
            });
            var task3 = Task.Run(() =>
            {
                Thread.Sleep(1000);
                for (int i = 0; i < total; i++)
                {
                    Thread.Sleep(10);
                    var item = new Tuple<int, DateTime>(3, DateTime.Now);
                    ConcurrentDictionary.AddOrUpdate(i, item, (i1, item1) => { return item; });
                }
            });
            var task4 = Task.Run(() =>
            {
                Thread.Sleep(1000);
                for (int i = 0; i < total; i++)
                {
                    Thread.Sleep(10);
                    var item = new Tuple<int, DateTime>(4, DateTime.Now);
                    ConcurrentDictionary.AddOrUpdate(i, item, (i1, item1) => { return item; });
                }
            });

            Task.WaitAll(task1, task2, task3, task4);

            foreach (KeyValuePair<int, Tuple<int, DateTime>> pair in ConcurrentDictionary)
            {
                Console.WriteLine($"{pair.Key}-{pair.Value.Item1}-{pair.Value.Item2}");
            }
        }
    }
}
