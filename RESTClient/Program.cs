using System;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Linq;

namespace RESTClient
{


    abstract class BasicHttpRequest
    {
        public abstract Task<string> MakeRequestAsync();

        protected BasicHttpRequest(HttpClient client, string url)
        {
            Client = client;
            Uri = url;
        } 

        protected HttpClient Client;
        protected string Uri;
        protected bool isInvalidRequest = false;
    }

    class HttpRequestGet : BasicHttpRequest
    {
        public HttpRequestGet(HttpClient client, string url) :
            base(client, url)
        {}

        public HttpRequestGet(HttpClient client, string url, string argument) :
            this(client, url)
        {
            if (argument == null || argument.Trim() == "")
            {
                return;
            }

            string[] userInputArray = argument.Split(' ');
            if (userInputArray.Length > 1 && userInputArray[0] == "--id" && userInputArray[1] != "" && userInputArray[1] != null)
            {
                Uri += $"/{userInputArray[1]}"; // TODO Figure out difference between uri and url once again
            }
            else
            {
                Console.WriteLine("Error. Example get usage: \"get --id abc\"");
                isInvalidRequest = true;
                return;
            } 
        }
        public override async Task<string> MakeRequestAsync()
        {
            if (isInvalidRequest)
            {
                return null;
            }
            HttpResponseMessage response = await Client.GetAsync(Uri);
            Console.WriteLine(response.StatusCode);
            var resp = await response.Content.ReadAsStringAsync();
            return resp;
        }
    }

    class HttpRequestPut : BasicHttpRequest
    {
        public HttpRequestPut(HttpClient client, string url, string argument) :
            base(client, url)
        {
            if (argument == null || argument.Trim() == "")
            {
                Console.WriteLine("Error. Please provide data to put.");
                isInvalidRequest = true;
                return;
            }

            string[] userInputArray = argument.Split(' ');
            if (userInputArray.Length == 2)
            {
                Uri += $"/{userInputArray[0]}";
                HttpData = new StringContent(userInputArray[1], Encoding.UTF8, "application/json");
            }
            else
            {
                Console.WriteLine("Error. Example get usage: \"put {id} {data}\"");
                isInvalidRequest = true;
                return;
            }
        }
        public override async Task<string> MakeRequestAsync()
        {
            if (isInvalidRequest)
            {
                return null;
            }

            HttpResponseMessage response = await Client.PutAsync(Uri, HttpData);
            Console.WriteLine(response.StatusCode);
            var resp = await response.Content.ReadAsStringAsync();
            return resp;
        }

        StringContent HttpData;
    }

    class HttpRequestPost : BasicHttpRequest
    {
        private bool IsJson(string jsonString)
        {
            string strInput = jsonString.Trim();
            if (!(strInput.StartsWith("{") && strInput.EndsWith("}")))
            {
                return false;
            }
            try
            {
                JToken.Parse(strInput);
                return true;
            }
            catch (JsonReaderException e)
            {
                Console.WriteLine($"Can't parse JSON for POST request. Reason:{e.Message}");
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Can't parse JSON for POST request. Reason:{e.Message}");
                return false;
            }
        }
        public HttpRequestPost(HttpClient client, string url, string argument) :
            base(client, url)
        {
            if (argument == "" || argument == null)
            {
                Console.WriteLine("Error. Please provide data to post.");
                isInvalidRequest = true;
                return;
            }
            if (!IsJson(argument))
            {
                Console.WriteLine("Error. Please provide JSON data to post.");
                isInvalidRequest = true;
                return;
            }
            HttpData = new StringContent(argument, Encoding.UTF8, "application/json");
        }
        public override async Task<string> MakeRequestAsync()
        {
            if (isInvalidRequest)
            {
                return null;
            }

            HttpResponseMessage response = await Client.PostAsync(Uri, HttpData);
            Console.WriteLine(response.StatusCode);
            var resp = await response.Content.ReadAsStringAsync();
            return resp;
        }

        StringContent HttpData;
    }

    class HttpRequestDelete : BasicHttpRequest
    {
        public HttpRequestDelete(HttpClient client, string url, string keyToDelete) :
            base(client, url)
        {
            Uri += $"/{keyToDelete}";
        }
        public override async Task<string> MakeRequestAsync()
        {
            HttpResponseMessage response = await Client.DeleteAsync(Uri);
            Console.WriteLine(response.StatusCode);
            var resp = await response.Content.ReadAsStringAsync();
            return resp;
        }    
    }

    class Program
    {
        static string[] availableRequestTypes = {"get", "delete", "post", "put"};

        static async Task RunRESTClient()
        {
            string userInput = Console.ReadLine();

            if (userInput == "" || userInput == null)
            {
                Console.WriteLine("Please specify request type.\nExample usage: \"get --id asd\"");
                return;
            }

            string[] userInputArray = userInput.Split(' ');

            string requestType = userInputArray[0];
            if (!availableRequestTypes.Contains(requestType))
            {
                Console.WriteLine($"Supported types of requests are {availableRequestTypes.ToString()}"); // TODO: Check
                return;
            }

            using var client = new HttpClient();

            client.BaseAddress = new Uri("http://localhost:5000");

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("appId", "campus-task");

            var url = "somedata";

            BasicHttpRequest request = null;

            if (requestType == "get")
            {
                request = new HttpRequestGet(client, url, userInput.Replace("get", "").Trim());
            }
            else if (requestType == "post")
            {
                request = new HttpRequestPost(client, url, userInput.Replace("post", "").Trim());
            }
            else if (requestType == "delete")
            {
                request = new HttpRequestDelete(client, url, userInput.Replace("delete", "").Trim());
            }
            else if (requestType == "put")
            {
                request = new HttpRequestPut(client, url, userInput.Replace("put", "").Trim());
            }

            if (request == null)
            {
                Console.WriteLine($"Bad requst - {userInput}. Try again.");
                return;
            }

            var result = await request.MakeRequestAsync();
            Console.WriteLine(result);
        }

        static async Task Main(string[] args)
        {
            while(true)
            {
                await RunRESTClient();
            }
        }
    }
}
