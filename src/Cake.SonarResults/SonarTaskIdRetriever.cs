using Cake.Core;
using Cake.Core.Diagnostics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Cake.SonarResults
{
    public static class SonarTaskIdRetriever
    {
        private static string _ReportFileName = "report-task.txt";
        public static string GetTaskId(ICakeContext context)
        {

            var logger = context.Log;
            string fullpath = context.Environment.WorkingDirectory.FullPath;

            logger.Information($"Searching current directory for report-task.txt - ${fullpath}");
            FileAttributes attr = File.GetAttributes(fullpath);
            if (!attr.HasFlag(FileAttributes.Directory))
            {
                logger.Error("Working directory is not set to a valid directory!");
                throw new DirectoryNotFoundException($"Working directory not valid");
            }
            List<string> reportFiles = new List<string>(Directory.GetFiles(fullpath, _ReportFileName, SearchOption.AllDirectories));
            if (!reportFiles.Any())
            {
                logger.Error("SonarQube report file not found!");
                throw new FileNotFoundException("SonarQube report file not found");
            }
            string reportFile = reportFiles.Select(x => new FileInfo(x)).OrderByDescending(x => x.LastWriteTime).First().FullName;

            var dic = File.ReadAllLines(reportFile)
              .Select(l => l.Split(new[] { '=' }))
              .ToDictionary(s => s[0].Trim(), s => s[1].Trim());

            return dic["ceTaskId"];

        }
    }
}
