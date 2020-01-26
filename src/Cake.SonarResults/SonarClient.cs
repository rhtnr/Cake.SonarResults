using Cake.Core;
using Cake.SonarResults.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cake.SonarResults
{
    public abstract class SonarClient 
    {
        protected abstract string ClientName { get; }

        public void ValidateResult(IRestResponse queryResult)
        {
            if (queryResult.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedAccessException("SonarQube returned an unauthorized exception");
            }
            if (queryResult.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new CakeException($"{ClientName} has not been found probably because it has already been deleted");
            }
            if (queryResult.StatusCode != System.Net.HttpStatusCode.Accepted)
            {
                throw new CakeException($"{ClientName} cannot be retrieved  - {queryResult.StatusDescription}");
            }
        }
    }
}
