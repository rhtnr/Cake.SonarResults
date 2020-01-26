using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.SonarResults.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cake.SonarResults
{
    public class SonarAnalysisClient : SonarClient
    {
        private readonly string _AnalysisUrl = "{0}/api/qualitygates/project_status?analysisId={1}";
        private readonly ICakeLog _Logger;
        private readonly IRestClient _Client;
        private readonly string clientName = "SonarQube Analysis";

        protected override string ClientName
        {
            get
            {
                return clientName;
            }
        }
        public SonarAnalysisClient(ICakeContext context, IRestClient client)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            _Logger = context.Log;
            _Client = client;
        }

        public SonarAnalysisClient(ICakeContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            _Logger = context.Log;
            _Client = new RestClient();
        }

        public ProjectStatus GetAnalysisResults(SonarResultsSettings settings, string analysisId)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));
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
                ValidateResult(queryResult);
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
