using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using Newtonsoft.Json.Linq;
namespace PR1
{
    class Program
    {
        public static int isDone = 0;
        public static List<string> finalResult = new List<string>();
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            string access_token = "";
            using (var httpClient = new HttpClient())
            {
                string responseBody = httpClient.GetStringAsync("http://localhost:5000/register").Result;
                JObject json = JObject.Parse(responseBody);
                access_token = json["access_token"].ToString();
                Console.WriteLine("Access token: " + json["access_token"]);
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
                        SendRequestWithNewThred(item.First.ToString(), access_token);
                    }
                }
                if(jsonResponse["data"] != null){
                    finalResult.Add(jsonResponse["data"].ToString());
                }
                
            }
        }

        public static void SendRequestWithNewThred(string route, string access_token)
        {
            var thread = new System.Threading.Thread(() =>
            {
                isDone++;
                SendRequest(route, access_token);
                isDone--;
                if(isDone == 0){
                    printResponse();
                }
            });
            thread.Start();
        }

        public static void printResponse() {
            Console.WriteLine("Done");
            finalResult.ForEach(x => Console.WriteLine(x + "\n--------------------------------"));
        }
    }
}
