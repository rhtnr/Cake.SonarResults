using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.SonarResults.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Cake.SonarResults
{
    public class SonarTaskClient
    {
        private readonly string _TaskUrl = "{0}/api/ce/task?id={1}";
        private readonly ICakeLog _Logger;
        private readonly IRestClient _Client;
        public SonarTaskClient(ICakeContext context, IRestClient client)
        {
            _Logger.Information($"Initaliazing SonarTaskClient with context and REST client");
            _Logger = context.Log;
            _Client = client;
        }

        public SonarTaskClient(ICakeContext context)
        {
            _Logger.Information($"Initaliazing SonarTaskClient with context");
            _Logger = context.Log;
            _Client = new RestClient();
        }

        public Task GetTaskResults(string sonarUrl, string taskId)
        {
            try
            {
                if (!Utility.ValidateUrl(sonarUrl))
                {
                    throw new ArgumentException("Invalid SonarQube URL");
                }
                string url = String.Format(_TaskUrl, sonarUrl, taskId);
                _Logger.Information($"Initializing request");
                var request = new RestRequest(url, Method.POST);
                _Logger.Information($"Setting Content-Type to application/json");
                request.OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; };
                _Logger.Information($"Retrieving task status from {url} [POST]");
                //Debugger.Launch();
                var queryResult = _Client.Execute<TaskWrapper>(request);
                if (queryResult.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    throw new CakeException("Sonar Task has not been found probably");
                }
                if (queryResult.StatusCode != System.Net.HttpStatusCode.Accepted)
                {
                    throw new CakeException($"Sonar Task cannot be retrieved  - {queryResult.StatusDescription}");
                }
                if (queryResult.Data == null)
                {
                    throw new FormatException($"Could not parse response from API [{queryResult?.StatusCode}] [{queryResult?.StatusDescription}] [{queryResult?.Content}]");
                }
                TaskWrapper x = queryResult.Data;
                return x.Task;
            }
            catch(Exception ex)
            {
                _Logger.Error($"An error occured while retrieving Task Status from SonarQube - {ex.Message}");
                throw;
            }
        }
    }
}
