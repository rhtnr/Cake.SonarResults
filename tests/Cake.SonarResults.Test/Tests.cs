using Cake.Core;
using Cake.SonarResults.Models;
using Moq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Xunit;

namespace Cake.SonarResults.Test
{
    public class Tests
    {
        [Fact]
        public void ValidateUrl()
        {
            //arrange
            string url = "http://something.com";
            string url2 = "https://something.com";
            string url3 = "http://something:9000/sonar";
            string url4 = "https://something.com:90/sonar";


            //act
            bool result = Utility.ValidateUrl(url);
            bool result2 = Utility.ValidateUrl(url2);
            bool result3 = Utility.ValidateUrl(url3);
            bool result4 = Utility.ValidateUrl(url4);

            //assert
            Assert.True(result && result2 && result3 && result4);
        }

        [Fact]
        public void ValidateInvalidUrl()
        {
            //arrange
            string url = "ftp://something.com";
            string url2 = "https://someth&ing:***";
            string url3 = "http://som\\ething:9000/sonar";
            string url4 = "ssl://something.com:90/sonar";


            //act
            bool result = Utility.ValidateUrl(url);
            bool result2 = Utility.ValidateUrl(url2);
            bool result3 = Utility.ValidateUrl(url3);
            bool result4 = Utility.ValidateUrl(url4);

            //assert
            Assert.False(result && result2 && result3 && result4);
        }

        [Fact]
        public void SonarTaskClient()
        {
            //arrange
            ICakeContext context = Mock.Of<ICakeContext>();
            IRestClient client = TestSetup.MockRestClient<TaskWrapper>(HttpStatusCode.Accepted, TestSetup.TaskResponse);
            SonarTaskClient taskClient = new SonarTaskClient(context, client);

            //act
            Task taskResult = taskClient.GetTaskResults("http://someurl", "AW_Eo9r1HErFYEHBDpHN");

            //assert
            Assert.NotNull(taskResult);
            Assert.Equal("AW_EpPNpIMlVdJl9IY2G", taskResult.AnalysisId);
        }

        [Fact]
        public void SonarAnalysisClient()
        {
            //arrange
            ICakeContext context = Mock.Of<ICakeContext>();
            IRestClient client = TestSetup.MockRestClient<ProjectStatusWrapper>(HttpStatusCode.Accepted, TestSetup.AnalysisResponse);
            SonarAnalysisClient analysisClient = new SonarAnalysisClient(context, client);

            //act
            ProjectStatus projectResult = analysisClient.GetAnalysisResults("http://someurl", "AW_YNmu6IMlVdJl9Ii6g");

            //assert
            Assert.NotNull(projectResult);
            Assert.Equal("ERROR", projectResult.Status);
        }

        [Fact]
        public void TaskIdRetrieverTests()
        {
            //arrange
            var context = TestSetup.MockCakeContext(Directory.GetCurrentDirectory());

            //act
            string id = SonarTaskIdRetriever.GetTaskId(context);

            //assert
            Assert.Equal("AW_KP4VKHErFYEHBDpHY", id);
        }
    }
}
