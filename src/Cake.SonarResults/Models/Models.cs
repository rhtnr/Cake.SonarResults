using System;
using System.Collections.Generic;
using System.Text;

namespace Cake.SonarResults.Models
{
    public class Task
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string ComponentId { get; set; }
        public string ComponentKey { get; set; }
        public string ComponentName { get; set; }
        public string ComponentQualifier { get; set; }
        public string AnalysisId { get; set; }
        public string Status { get; set; }
        public DateTime SubmittedAt { get; set; }
        public string SubmitterLogin { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime ExecutedAt { get; set; }
        public int ExecutionTimeMs { get; set; }
        public bool Logs { get; set; }
        public bool HasScannerContext { get; set; }
        public string Organization { get; set; }
        public int WarningCount { get; set; }
        public List<object> Warnings { get; set; }
    }

    public class TaskWrapper
    {
        public Task Task { get; set; }
    }

    public class ProjectStatusWrapper
    {
        public ProjectStatus ProjectStatus { get; set; }
    }

    public class ProjectStatus
    {
        public string Status { get; set; }
        public List<Condition> Conditions { get; set; }
        public List<Period> Periods { get; set; }
        public bool IgnoredConditions { get; set; }
    }
    public class Condition
    {
        public string Status { get; set; }
        public string MetricKey { get; set; }
        public string Comparator { get; set; }
        public int PeriodIndex { get; set; }
        public string ErrorThreshold { get; set; }
        public string ActualValue { get; set; }
    }

    public class Period
    {
        public int Index { get; set; }
        public string Mode { get; set; }
        public DateTime Date { get; set; }
    }


}
