C:\Windows\Microsoft.NET\Framework64\v4.0.30319\msbuild ../NRest.sln /p:Configuration=Release
nuget pack ../NRest/NRest.csproj -Properties Configuration=Release
nuget push *.nupkg
del *.nupkg