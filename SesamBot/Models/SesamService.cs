using Microsoft.Bot.Builder.Dialogs;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SesamBot.Models
{
    [Serializable]
    public class SesamService
    {
        //const string baseUrl = "http://localhost:9042/api";
        const string baseUrl = "http://9af6091e.ngrok.io/api";
        //const string baseUrl = "https://open.sesam.io/api/api";

        public static async Task<string> GetEntities(string entity)
        {
            string jsonResult = string.Empty;
            using (WebClient client = new WebClient())
            {
                jsonResult = await client.DownloadStringTaskAsync($"{baseUrl}/{entity}");
            }
            string result = string.Empty;
            var token = JToken.Parse(jsonResult);
            switch (entity)
            {
                case "license":
                    result = ParseLicense(token);
                    break;
                case "pipes":
                    result = ParsePipes(token);
                    break;
                case "systems":
                    result = ParseSystems(token);
                    break;
                case "config":
                    result = ParseConfig(token);
                    break;
                case "datasets":
                    result = ParseDatasets(token);
                    break;
                default:
                    break;

            }
            return result;
        }

        public static async Task<List<ISesamError>> GetErrors()
        {
            List<string> datasets = await GetSystemPumpDatasets();
            List<ISesamError> errors = new List<ISesamError>();
            string jsonResult;
            using (WebClient client = new WebClient())
            {
                foreach (string dataset in datasets)
                {
                    jsonResult = string.Empty;
                    try
                    {
                        jsonResult = await client.DownloadStringTaskAsync($"{baseUrl}/datasets/{dataset}/entities/pump-failed");
                    }
                    catch (WebException)
                    {
                        continue;
                    }
                    var token = JToken.Parse(jsonResult);
                    if (token is JObject)
                    {
                        SesamPumpError error = new SesamPumpError()
                        {
                            Entity = (string)token["pipe"],
                            EventType = (string)token["event_type"],
                            Reason_why_stopped = (string)token["reason_why_stopped"],
                            Traceback = (string)token["traceback"],
                            Original_traceback = (string)token["original_traceback"],
                            Original_error_message = (string)token["original_error_message"],
                            End_time = (DateTime)token["end_time"],
                            Hash = (string)token["_hash"],
                            Url = $"{baseUrl}/datasets/{dataset}/entities/pump-failed",
                            Updated = (int)token["_updated"]
                        };
                        errors.Add(error);
                    }
                }
            }
            return errors;
        }

        public async void AlertErrors(ResumptionCookie cookie)
        {
            List<string> systemDatasets = await GetSystemPumpDatasets();
            foreach (string dataset in systemDatasets)
            {

            }
        }

        private static string ParseDatasets(JToken token)
        {
            StringBuilder sb = new StringBuilder();
            if (token is JArray)
            {
                sb.AppendLine("The ids of all the datasets are:");
                foreach (JToken t in token)
                {
                    sb.Append(t["_id"] + ", ");
                }
            }
            else if (token is JObject)
            {
                sb.Append("There seems to be a problem with the datasets");
            }
            String result = sb.ToString();
            return result.Substring(0, result.Length - 1);
        }

        private static string ParseConfig(JToken token)
        {
            StringBuilder sb = new StringBuilder();
            if (token is JArray)
            {
                foreach (JToken config in token)
                {
                    sb.AppendLine($"Configuration with id {config["_id"]} has the following endpoints:");
                    foreach (JToken endpoint in config["endpoints"])
                    {
                        sb.AppendLine(endpoint.ToString());
                    }
                }

            }
            return sb.ToString();
        }

        private static string ParseSystems(JToken token)
        {
            StringBuilder sb = new StringBuilder();
            if (token is JArray)
            {
                sb.AppendLine("These systems are configured:");
                foreach (JToken t in token)
                {
                    sb.Append(t["_id"] + ", ");
                }
            }
            else if (token is JObject)
            {
                sb.Append("There seems to be a problem with the systems");
            }
            String result = sb.ToString();
            return result.Substring(0, result.Length - 1);
        }

        private static string ParsePipes(JToken token)
        {
            if (token is JArray)
            {
                return $"Found {token.Count()} pipes";
            }
            else if (token is JObject)
            {
                return "There seems to be a problem with the pipes";
            }
            return "No pipes found";
        }

        private static string ParseLicense(JToken token)
        {
            StringBuilder sb = new StringBuilder();
            if (token is JObject)
            {
                sb.Append($"Found license with subscriber id  {token["_id"]}, expires {token["expires"]}");
                if (token["licensed-to"] != null)
                {
                    sb.Append($", licensed to {token["licensed-to"]["email"]}");
                }
                return sb.ToString();
            }
            else if (token is JArray)
            {
                return "There seems to be a problem with getting the license";
            }
            return "No license found";
        }

        private static async Task<List<string>> GetSystemPumpDatasets()
        {
            List<string> results = new List<string>();

            string jsonResult = string.Empty;
            using (WebClient client = new WebClient())
            {
                jsonResult = await client.DownloadStringTaskAsync($"{baseUrl}/datasets");
            }
            JToken token = JToken.Parse(jsonResult);
            if (token is JArray)
            {
                foreach (JToken dataset in token)
                {
                    if (dataset["_id"] != null && dataset.Value<string>("_id").StartsWith("system:pump"))
                    {
                        results.Add(dataset.Value<string>("_id"));
                    }
                }
            }

            return results;
        }
    }
}