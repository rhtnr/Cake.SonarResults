using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.SonarResults.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cake.SonarResults
{
    public class SonarAnalysisClient
    {
        private readonly string _AnalysisUrl = "{0}/api/qualitygates/project_status?analysisId={1}";
        private readonly ICakeLog _Logger;
        private readonly IRestClient _Client;
        public SonarAnalysisClient(ICakeContext context, IRestClient client)
        {
            _Logger = context.Log;
            _Client = client;
        }

        public SonarAnalysisClient(ICakeContext context)
        {
            _Logger = context.Log;
            _Client = new RestClient();
        }

        public ProjectStatus GetAnalysisResults(SonarResultsSettings settings, string analysisId)
        {
            try
            {
                String sonarUrl = settings.Url;
                if (!Utility.ValidateUrl(sonarUrl))
                {
                    throw new ArgumentException("Invalid SonarQube URL");
                }
                if (settings.IsAuthEnabled)
                {
                    _Client.Authenticator = settings.GetAuthenticator;
                }
                _Logger.Information($"Initializing analysis request");
                string url = String.Format(_AnalysisUrl, sonarUrl, analysisId);
                var request = new RestRequest(url, Method.POST);
                _Logger.Information($"Setting API endpoint to {url} [POST]");
                request.OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; };
                var queryResult = _Client.Execute<ProjectStatusWrapper>(request);
                if(queryResult.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    throw new CakeException("Sonar Analysis has not been found probably because it has already been deleted");
                }
                if (queryResult.StatusCode != System.Net.HttpStatusCode.Accepted)
                {
                    throw new CakeException($"Sonar Analysis cannot be retrieved  - {queryResult.StatusDescription}");
                }
                if (queryResult.Data == null)
                {
                    throw new FormatException($"Could not parse response from API [{queryResult?.StatusCode}] [{queryResult?.StatusDescription}] [{queryResult?.Content}]");
                }
                ProjectStatusWrapper x = queryResult.Data;
                return x.ProjectStatus;
            }
            catch (Exception ex)
            {
                _Logger.Error($"An error occured while retrieving Analysis Status from SonarQube - {ex.Message}");
                throw;
            }
        }
    }
}
