using System;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace PR1
{
    class Program
    {
        public static List<FetchedDataType> fetchedResult = new List<FetchedDataType>();
        public static int isDone = 0;
        public static List<string> finalResult = new List<string>();
        public static Stopwatch watch = new Stopwatch();
        private static ServerSocket socket = new ServerSocket();
        static void Main(string[] args)
        {
            string access_token = "";
            using (var httpClient = new HttpClient())
            {
                string responseBody = httpClient.GetStringAsync("http://localhost:5000/register").Result;
                JObject json = JObject.Parse(responseBody);
                access_token = json["access_token"].ToString();
                Console.WriteLine("Access token: " + json["access_token"]);
                watch.Start();
                SendRequest("/home", access_token);
            }
            Console.ReadLine();
        }

        public static void SendRequest(string route, string access_token)
        {
            using (var httpClient = new HttpClient())
            {
                Console.WriteLine("Sending request to " + route);
                httpClient.DefaultRequestHeaders.Add("X-Access-Token", access_token);
                string responseBody = httpClient.GetStringAsync("http://localhost:5000" + route).Result;
                JObject jsonResponse = JObject.Parse(responseBody);
                if (jsonResponse["link"] != null)
                {
                    JObject array = (JObject)jsonResponse["link"];
                    foreach (var item in array.Children())
                    {
                        SendRequestWithNewThread(item.First.ToString(), access_token);
                    }
                }
                if (jsonResponse["data"] != null)
                {
                    fetchedResult.Add(Helpers.GetFetchedDataType(jsonResponse));
                    finalResult.Add(jsonResponse["data"].ToString());
                }
            }
        }

        public static void SendRequestWithNewThread(string route, string access_token)
        {
            var thread = new System.Threading.Thread(() =>
            {
                isDone++;
                SendRequest(route, access_token);
                isDone--;
                if (isDone == 0)
                {
                    watch.Stop();
                    printResponse();
                    startServer();
                }
            });
            thread.Start();

            // Using ThreadPool takes longer
            // ThreadPool.QueueUserWorkItem(delegate (object state)
            // {
            //     isDone++;
            //     SendRequest(route, access_token);
            //     isDone--;
            //     if (isDone == 0)
            //     {
            //         watch.Stop();
            //         printResponse();
            //     }
            // });
        }

        public static void printResponse()
        {
            Console.WriteLine("Done");
            finalResult.ForEach(x => Console.WriteLine(x + "\n--------------------------------"));
            Console.Write("Process done in " + watch.Elapsed.Seconds + " seconds.");
        }

        public static void startServer()
        {
            socket.Bind("127.0.0.1", 9000);
            socket.Listen();
            socket.Accept();
            Console.WriteLine("\n\nServer started and listening");
            Console.ReadLine();
        }
    }
}
