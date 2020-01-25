using Cake.Core;
using Cake.Core.Diagnostics;
using Moq;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Cake.SonarResults.Test
{
    public static class TestSetup
    {
        public static string TaskResponse = File.ReadAllText("taskResponseMock.json");
        public static string AnalysisResponse = File.ReadAllText("analysisResponseMock.json");
        public static IRestClient MockRestClient<T>(HttpStatusCode httpStatusCode, string json)
    where T : new()
        {
            var data = JsonConvert.DeserializeObject<T>(json);
            var response = new Mock<IRestResponse<T>>();
            response.Setup(_ => _.StatusCode).Returns(httpStatusCode);
            response.Setup(_ => _.Data).Returns(data);

            var mockIRestClient = new Mock<IRestClient>();
            mockIRestClient
              .Setup(x => x.Execute<T>(It.IsAny<IRestRequest>()))
              .Returns(response.Object);
            return mockIRestClient.Object;
        }

        public static ICakeContext MockCakeContext(string workingDirectory)
        {
            var log = Mock.Of<ICakeLog>();

            var envMock = new Mock<ICakeEnvironment>();
            envMock.Setup(x => x.WorkingDirectory).Returns(workingDirectory);
            var env = envMock.Object;
            
            var mockICakeContext = new Mock<ICakeContext>();
            mockICakeContext.Setup(x => x.Log).Returns(log);
            mockICakeContext.Setup(x => x.Environment).Returns(env);

            return mockICakeContext.Object;
        }
    }
}
