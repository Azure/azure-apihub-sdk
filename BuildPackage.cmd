@echo off

REM nuget restore -NonInteractive

msbuild /p:Configuration=Release

md .\.artifacts
nuget pack -NoPackageAnalysis -OutputDirectory .\.artifacts .\.nuget\Azure.ApiHub.Sdk.nuspec