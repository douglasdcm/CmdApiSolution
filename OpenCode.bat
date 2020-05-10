"C:\Users\Douglas\.nuget\packages\opencover\4.7.922\tools\OpenCover.Console.exe" -register:user -target:runtests.bat   -output:C:\Reports\MSTest\projectCoverageReport.xml -oldstyle
"C:\Users\Douglas\.nuget\packages\reportgenerator\4.5.8\tools\net47\ReportGenerator.exe" -reports:"C:\Reports\MSTest\projectCoverageReport.xml" -targetdir:"C:\Reports\MSTest\"
pause