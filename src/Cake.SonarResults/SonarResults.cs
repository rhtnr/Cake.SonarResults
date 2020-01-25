using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.Diagnostics;
using System;
using System.Threading;

namespace Cake.SonarResults
{
    public static class SonarResults
    {
        [CakeMethodAlias]
        public static void SonarQubeResults(this ICakeContext context, SonarResultsSettings settings)
        {
            string taskId = SonarTaskIdRetriever.GetTaskId(context);
            ICakeLog logger = context.Log;

            logger.Information("Creating a Sonar Task Retriever Client");

            SonarTaskClient taskClient = new SonarTaskClient(context);
            SonarAnalysisClient analysisClient = new SonarAnalysisClient(context);
            while (true)
            {
                var taskResults = taskClient.GetTaskResults(settings.Url, taskId);
                //until the status is SUCCESS, CANCELED or FAILED
                if (taskResults.Status != "SUCCESS" && taskResults.Status != "CANCELED" && taskResults.Status != "FAILED")
                {
                    Thread.Sleep(10000);
                }
                else
                {
                    if (taskResults.Status == "SUCCESS")
                    {
                        var analysisStatus = analysisClient.GetAnalysisResults(settings.Url, taskResults.AnalysisId);
                        logger.Information(analysisStatus);
                        if (analysisStatus.Status == "ERROR" || analysisStatus.Status == "FAILED")
                        {
                            logger.Error($"SonarQube analysis returned {analysisStatus.Status}");
                            throw new CakeException("SonarQube analysis Failed. Breaking the build");
                        }
                        logger.Information($"SonarQube analysis returned {analysisStatus.Status}");
                        
                    }
                    else
                    {
                        logger.Error($"SonarQube Task Failed! - STATUS => {taskResults.Status}");
                        throw new CakeException($"SonarQube task returned {taskResults.Status}");
                    }
                    break;
                }
            }

        }
    }
}
