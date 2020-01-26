# Cake.SonarResults
A Cake Addin to break build if SonarQube Analysis fails!

![](https://github.com/rhtnr/Cake.SonarResults/workflows/Test%20Build%20Publish%20Push/badge.svg)

### Usage
The current working directory should be set to the directory where you ran the sonar scan
SonarBegin and SonarEnd should have run successfully.

```powershell
#addin "Cake.SonarResults"
```

```cs

Task("Sonar-AnalysisResults")
    .IsDependentOn("Sonar-End")
    .Does(() => 
    {
      SonarQubeResults(new SonarResultsSettings("http://sonarqube:9000"));
    });
```


[README file made with my favorite MD editor](https://dillinger.io/)
