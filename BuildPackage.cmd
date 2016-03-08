@echo off

REM nuget restore -NonInteractive

msbuild

md .\.artifacts
nuget pack -NoPackageAnalysis -OutputDirectory .\.artifacts .\.nuget\Azure-ApiHub-Sdk.nuspec