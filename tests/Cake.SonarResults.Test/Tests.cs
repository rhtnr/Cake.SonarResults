using System;
using Cake.Core;
using Cake.SonarResults.Models;
using Moq;
using RestSharp;
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
            IRestClient client = TestSetup.MockRestClient<TaskWrapper>(HttpStatusCode.OK, TestSetup.TaskResponse);
            SonarTaskClient taskClient = new SonarTaskClient(context, client);
            SonarResultsSettings settings = new SonarResultsSettings("http://someurl");

            //act
            Task taskResult = taskClient.GetTaskResults(settings, "AW_Eo9r1HErFYEHBDpHN");

            //assert
            Assert.NotNull(taskResult);
            Assert.Equal("AW_EpPNpIMlVdJl9IY2G", taskResult.AnalysisId);
        }

        [Fact]
        public void SonarAnalysisClient()
        {
            //arrange
            ICakeContext context = Mock.Of<ICakeContext>();
            IRestClient client = TestSetup.MockRestClient<ProjectStatusWrapper>(HttpStatusCode.OK, TestSetup.AnalysisResponse);
            SonarAnalysisClient analysisClient = new SonarAnalysisClient(context, client);
            SonarResultsSettings settings = new SonarResultsSettings("http://someurl");

            //act
            ProjectStatus projectResult = analysisClient.GetAnalysisResults(settings, "AW_YNmu6IMlVdJl9Ii6g");

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

        [Fact]
        public void AuthSettings()
        {
            //arrange
            SonarResultsSettings settingsWithToken = new SonarResultsSettings("someurl", "abc");
            SonarResultsSettings settingsWithUser = new SonarResultsSettings("someurl", "abc", "abc");
            SonarResultsSettings settingsWithNoCreds = new SonarResultsSettings("someurl");

            //arrange
            bool authEnabledToken = settingsWithToken.IsAuthEnabled;
            bool authEnabledUser = settingsWithUser.IsAuthEnabled;
            bool authEnabledNone = settingsWithNoCreds.IsAuthEnabled;

            //assert
            Assert.True(authEnabledToken);
            Assert.True(authEnabledUser);
            Assert.False(authEnabledNone);
        }

        [Fact]
        void CorrectlyGeneratesAnAuthenticator()
        {
            //arrange
            SonarResultsSettings settingsWithToken = new SonarResultsSettings("someurl", "abc");
            SonarResultsSettings settingsWithUser = new SonarResultsSettings("someurl", "abc", "abc");
            SonarResultsSettings settingsWithNoCreds = new SonarResultsSettings("someurl");

            Assert.Throws<NotSupportedException>(() => settingsWithNoCreds.GetAuthenticator);
            Assert.NotNull(settingsWithUser.GetAuthenticator);
            Assert.NotNull(settingsWithToken.GetAuthenticator);
        }
    }
}
