using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ScalingTest
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();
        private const string Url = "https://www.saucedemo.com/";

        static async Task Main()
        {
            int[] concurrencies = { 1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024 };

            Console.WriteLine("Concurrency\tAvg Time (ms)\tSuccess Rate");
            Console.WriteLine("--------------------------------------------");

            foreach (int concurrency in concurrencies)
            {
                await RunLoadStep(concurrency);
                await Task.Delay(1000);
            }
        }

        static async Task RunLoadStep(int concurrencyCount)
        {
            var tasks = new Task<bool>[concurrencyCount];
            var sw = Stopwatch.StartNew();

            for (int i = 0; i < concurrencyCount; i++)
            {
                tasks[i] = SendRequest();
            }

            var results = await Task.WhenAll(tasks);
            sw.Stop();

            double avgTime = (double)sw.ElapsedMilliseconds / concurrencyCount;

            int successes = results.Count(x => x);
            Console.WriteLine($"{concurrencyCount}\t\t{sw.ElapsedMilliseconds}\t\t{successes}/{concurrencyCount}");
        }

        static async Task<bool> SendRequest()
        {
            try
            {
                var response = await client.GetAsync(Url);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}