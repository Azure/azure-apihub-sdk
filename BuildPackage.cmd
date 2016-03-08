@echo off

REM nuget restore -NonInteractive

msbuild

md .\.artifacts
nuget pack -NoPackageAnalysis -OutputDirectory .\.artifacts .\Azure-ApiHub-Sdk.nuspec